using BalikoBot.BO;
using BalikoBot.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace BalikoBot
{
	/// <summary>
	/// klinet BalikoBot (obecny)
	/// </summary>
	public class BalikoBotClient
	{
		public static readonly string API_SCHEMA = "https://";
		public static readonly string API_URL_V1 = "api.balikobot.cz";
		public static readonly string API_URL_V2 = "api.balikobot.cz/v2";

		#region Constructor

		private readonly Carriers _carrier;
		private readonly string _username;
		private readonly string _password;

		internal BalikoBotClient(Carriers carrier, IBalikoBotConfiguration config)
		{
			_carrier = carrier;
			_username = config.Username;
			_password = config.Password;
		}

		#endregion

		/// <summary>
		/// sluzby podle dopravce
		/// </summary>
		public async Task<IEnumerable<BalikoBotService>> Services()
		{
			var json = await GetAsync($"{API_SCHEMA}{API_URL_V1}/{_carrier}/services");

			var services = (JObject)json["service_types"];
			if (services == null)
				throw new InvalidOperationException("JSON: service_types");

			return services.Properties()
				.Select(x => new BalikoBotService()
				{
					ServiceType = x.Name,
					Description = x.Value.ToString()
				})
				.ToArray();
		}

		/// <summary>
		/// Seznam států, do kterých lze zasílat skrze jednotlivé služby přepravce
		/// </summary>
		public async Task<IEnumerable<string>> Countries4service(string serviceType)
		{
			if (string.IsNullOrEmpty(serviceType))
				throw new ArgumentException(nameof(serviceType));

			var json = await GetAsync($"{API_SCHEMA}{API_URL_V1}/{_carrier}/countries4service");

			var services = (JObject)json["service_types"];
			if (services == null)
				throw new InvalidOperationException("JSON: service_types");

			var service = services.ObjectValuesOfProperties().FirstOrDefault(x => x.Value<string>("service_type") == serviceType);
			if (service == null)
				throw new InvalidOperationException($"JSON: service_types == '{serviceType}'");

			var countries = (JObject)service["countries"];
			if (countries == null)
				throw new InvalidOperationException("JSON: countries");

			return countries.StringValuesOfProperties().ToArray();
		}

		/// <summary>
		/// přidání balíku/balíků		/// </summary>
		/// <remarks>
		/// Přidává balík/balíky, které se odešlou ke svozu.
		/// </remarks>
		public async Task<IEnumerable<BalikoBotPackage>> Add(params BalikoBotData[] datas)
		{
			if (datas == null || datas.Length == 0)
				throw new ArgumentException(nameof(datas));

			var data = datas.FromArrayToJArray((x, arr) =>
			{
				arr.Add(JObject.FromObject(x));
			});

			var json = await PostAsync($"{API_SCHEMA}{API_URL_V1}/{_carrier}/add", data);

			int status = (int)json["status"];
			// OK (200) nebo OK, uz drive ulozeno (208)
			if (status == 200 || status == 208)
			{
				return json.ForEachValuesOfProperties((o, x) => new BalikoBotPackage()
				{
					EshopId = (string)datas[x][BalikoBotData.EID],

					CarrierId = (string)o["carrier_id"],
					PackageId = (int)o["package_id"],
					LabelUrl = (string)o["label_url"],
					Status = (int)o["status"],
				});
			}
			else
			{
				// chyba
				throw new BalikoBotAddException(datas, json);
			}
		}

		/// <summary>
		/// K (obdoba metody ADD jen se data neuloží do systému, ale zkontrolují se a API vrátí zda jsou v pořádku, případně seznam chyb)		/// </summary>
		public async Task<IEnumerable<BalikoBotPackage>> Check(params BalikoBotData[] datas)
		{
			if (datas == null || datas.Length == 0)
				throw new ArgumentException(nameof(datas));

			var data = datas.FromArrayToJArray((x, arr) =>
			{
				arr.Add(JObject.FromObject(x));
			});

			var json = await PostAsync($"{API_SCHEMA}{API_URL_V1}/{_carrier}/check", data);

			int status = (int)json["status"];
			// OK (200) nebo OK, uz drive ulozeno (208)
			if (status == 200 || status == 208)
			{
				return json.ForEachValuesOfProperties((o, x) => new BalikoBotPackage()
				{
					EshopId = (string)datas[x][BalikoBotData.EID],

					CarrierId = (string)o["carrier_id"],
					PackageId = (int)o["package_id"],
					LabelUrl = (string)o["label_url"],
					Status = (int)o["status"],
				});
			}
			else
			{
				// chyba
				throw new BalikoBotAddException(datas, json);
			}
		}

		/// <summary>
		/// odstranění balíku
		/// </summary>
		/// <remarks>
		/// Odstranit lze pouze zásilky, které ještě nebyly odeslány ke „svozu“ metodou ORDER.
		/// </remarks>
		public async Task<IEnumerable<BalikoBotResult>> Drop(params int[] packageIds)
		{
			if (packageIds == null || packageIds.Length == 0)
				throw new ArgumentException(nameof(packageIds));

			var data = packageIds.FromArrayToJArray((x, arr) =>
			{
				arr.Add(JObject.FromObject(new { id = x }));
			});

			var json = await PostAsync($"{API_SCHEMA}{API_URL_V1}/{_carrier}/drop", data);

			return json.ForEachValuesOfProperties((o, x) => new BalikoBotResult()
			{
				Status = (int)o["status"]
			});
		}

		/// <summary>
		/// stav balíku
		/// </summary>
		/// <remarks>
		/// Vrací všechny stavy balíku/balíků, ve kterých se dosud ocitl s textovým popisem.
		/// </remarks>
		public async Task<IEnumerable<BalikoBotTrack>> Track(params string[] carrierIds)
		{
			if (carrierIds == null || carrierIds.Length == 0)
				throw new ArgumentException(nameof(carrierIds));

			var data = carrierIds.FromArrayToJArray((x, arr) =>
			{
				arr.Add(JObject.FromObject(new { id = x }));
			});

			var json = await PostAsync($"{API_SCHEMA}{API_URL_V2}/{_carrier}/track", data);

			return json.ForEachValuesOfProperties((o, x) => new BalikoBotTrack()
			{
				CarrierId = carrierIds[x],
				Items = o.ForEachValuesOfProperties((oo, xx) => new BalikoBotTrackItem()
				{
					Date = (DateTime)oo["date"],
					Name = (string)oo["name"],
				})
			});
		}

		/// <summary>
		/// posledni stav balíku
		/// </summary>
		/// <remarks>
		/// Vrací poslední stav balíku/balíků ve formě čísla a textové prezentace.
		/// </remarks>
		public async Task<IEnumerable<BalikoBotTrackStatus>> TrackStatus(params string[] carrierIds)
		{
			if (carrierIds == null || carrierIds.Length == 0 || carrierIds.Length > 4)
				throw new ArgumentException(nameof(carrierIds));

			var data = carrierIds.FromArrayToJArray((x, arr) =>
			{
				arr.Add(JObject.FromObject(new { id = x }));
			});

			var json = await PostAsync($"{API_SCHEMA}{API_URL_V1}/{_carrier}/trackstatus", data);

			return json.ForEachValuesOfProperties((o, x) => new BalikoBotTrackStatus()
			{
				CarrierId = carrierIds[x],
				Status = (BalikoBotTrackStatuses)(int)o["status_id"],
				StatusId = (int)o["status_id"],
				Text = (string)o["status_text"],
			});
		}

		/// <summary>
		/// informace k poslednímu/konkrétnímu svozu
		/// </summary>
		/// <remarks>
		/// Soupis dosud neodeslaných balíků se základními informacemi.
		/// </remarks>
		public async Task<IEnumerable<BalikoBotPackage>> Overview()
		{
			var json = await PostAsync($"{API_SCHEMA}{API_URL_V1}/{_carrier}/overview");

			return json.ForEachValuesOfProperties((o, x) => new BalikoBotPackage()
			{
				EshopId = (string)o["eshop_id"],
				CarrierId = (string)o["carrier_id"],
				PackageId = (int)o["package_id"],
				LabelUrl = (string)o["label_url"],
			});
		}

		/// <summary>
		/// zaslané údaje o konkrétním balíku a odkaz na štítek pro tisk
		/// </summary>
		/// <remarks>
		/// Vrací poslední stav balíku/balíků ve formě čísla a textové prezentace.
		/// </remarks>
		public async Task<BalikoBotData> Package(int packageId)
		{
			if (packageId == 0)
				throw new ArgumentException(nameof(packageId));

			var json = await PostAsync($"{API_SCHEMA}{API_URL_V1}/{_carrier}/package/{packageId}");

			return json.ToObject<BalikoBotData>();
		}

		/// <summary>
		/// vrací hromadné PDF se štítky pro vybrané balíky u konkrétního dopravce
		/// </summary>
		/// <remarks>
		/// Metoda vracející hromadné PDF se štítky pro vyžádané balíčky (package_ids) u vybraného dopravce.
		/// </remarks>
		public async Task<BalikoBotLabel> Labels(params int[] packageIds)
		{
			if (packageIds == null || packageIds.Length == 0)
				throw new ArgumentException(nameof(packageIds));

			var data = JObject.FromObject(new
			{
				package_ids = new JArray(packageIds)
			});

			var json = await PostAsync($"{API_SCHEMA}{API_URL_V1}/{_carrier}/labels", data);

			return json.ToObject<BalikoBotLabel>();
		}

		/// <summary>
		/// předání dat přepravci – svozová dávka
		/// </summary>
		/// <remarks>
		/// Předání dat do systému přepravce („objednání svozu“) pro dosud neodeslané balíky.
		/// </remarks>
		public async Task<BalikoBotOrder> Order(params int[] packageIds)
		{
			if (packageIds == null || packageIds.Length == 0)
				throw new ArgumentException(nameof(packageIds));

			var data = JObject.FromObject(new
			{
				package_ids = new JArray(packageIds)
			});

			var json = await PostAsync($"{API_SCHEMA}{API_URL_V1}/{_carrier}/order", data);

			return json.ToObject<BalikoBotOrder>();
		}

		/// <summary>
		/// informace k poslednímu/konkrétnímu svozu
		/// </summary>
		public async Task<BalikoBotOrder> OrderView(int? orderId = null)
		{
			var url = orderId.HasValue ? $"/{orderId}" : null;
			var json = await PostAsync($"{API_SCHEMA}{API_URL_V1}/{_carrier}/orderview{url}");

			var result = json.ToObject<BalikoBotOrder>();

			// prevod ids z nestandardni 'dictionary' formy na bezne pole na vystup
			var ids = (JObject)json["package_ids"];
			if (ids != null)
			{
				result.PackageIds = ids.ObjectValuesOfProperties().ParseInt();
			}

			return result;
		}

		/// <summary>
		/// Vrací výčet PSČ, na které se dají posílat zásilky u konkrétní služby.
		/// Tato PSČ jsou platná pro atribut rec_zip v metodě ADD.
		/// </summary>
		public async Task<BalikoBotZipCode> ZipCodes(string serviceType)
		{
			if (string.IsNullOrEmpty(serviceType))
				throw new ArgumentException(nameof(serviceType));

			var json = await PostAsync($"{API_SCHEMA}{API_URL_V1}/{_carrier}/zipcodes/{serviceType}");

			var result = json.ToObject<BalikoBotZipCode>();

			// prevod ids z nestandardni 'dictionary' formy na bezne pole na vystup
			var zips = (JObject)json["zip_codes"];
			if (zips != null)
			{
				result.Items = zips.ForEachValuesOfProperties((o, x) => o.ToObject<BalikoBotZipCodeItem>());
			}

			return result;
		}

		/// <summary>
		/// Vrací seznam poboček, na které se dají posílat zásilky u konkrétní služby.
		/// Čísla poboček se poté dají předat do atributu branch_id v metodě ADD.
		/// </summary>
		public async Task<BalikoBotBranch> Branches(string serviceType = null)
		{
			var url = !string.IsNullOrEmpty(serviceType) ? $"/{serviceType}" : null;
			var json = await PostAsync($"{API_SCHEMA}{API_URL_V1}/{_carrier}/branches{url}");

			var result = json.ToObject<BalikoBotBranch>();

			// prevod ids z nestandardni 'dictionary' formy na bezne pole na vystup
			var branches = (JObject)json["branches"];
			if (branches != null)
			{
				result.Items = branches.ForEachValuesOfProperties((o, x) => o.ToObject<BalikoBotBranchItem>());
			}

			return result;
		}

		#region Helpers

		private async Task<JObject> GetAsync(string url)
		{
			return await GetClientInternal(async (client) => await client.GetAsync(url));
		}
		private async Task<JObject> PostAsync(string url)
		{
			return await GetClientInternal(async (client) => await client.PostAsync(url, new StringContent("", Encoding.UTF8, "application/json")));
		}
		private async Task<JObject> PostAsync(string url, JToken data)
		{
			return await GetClientInternal(async (client) => await client.PostAsync(url, new StringContent(data.ToString(Formatting.Indented), Encoding.UTF8, "application/json")));
		}

		private async Task<JObject> GetClientInternal(Func<HttpClient, Task<HttpResponseMessage>> todo)
		{
			var client = new HttpClient();
			var byteArray = Encoding.ASCII.GetBytes($"{_username}:{_password}");
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			var response = await todo(client);
			var content = await response.Content.ReadAsStringAsync();

			var unescaped = UnicodeTools.Unescape(content);
			var json = JObject.Parse(unescaped);

			return json;
		}

		#endregion
	}
}
