﻿using BalikoBot.BO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BalikoBot
{
	/// <summary>
	/// klinet BalikoBot
	/// </summary>
	public class BalikoBotClient
	{
		public static readonly string API_SCHEMA = "https://";
		public static readonly string API_URL_V1 = "api.balikobot.cz";
		public static readonly string API_URL_V2 = "api.balikobot.cz/v2";

		#region Constructor
		private readonly string _username;
		private readonly string _password;
		public BalikoBotClient(string login, string password)
		{
			_username = login;
			_password = password;
		}
		public BalikoBotClient(IBalikoBotConfiguration config)
		{
			_username = config.Username;
			_password = config.Password;
		}
		#endregion

		#region Services
		/// <summary>
		/// sluzby podle dopravce
		/// </summary>
		public async Task<IEnumerable<BalikoBotService>> GetServices(Carriers carrier)
		{
			var json = await GetAsync($"{API_SCHEMA}{API_URL_V1}/{carrier}/services");

			var services = (JObject)json["service_types"];
			return services.Properties()
				.Select(x => new BalikoBotService()
				{
					Type = x.Name,
					Description = x.Value.ToString()
				})
				.ToArray();
		}
		#endregion

		#region Countries4service
		/// <summary>
		/// Seznam států, do kterých lze zasílat skrze jednotlivé služby přepravce
		/// </summary>
		public async Task<IEnumerable<string>> GetCountries4service(Carriers carrier, string serviceType)
		{
			var json = await GetAsync($"{API_SCHEMA}{API_URL_V1}/{carrier}/countries4service");

			var services = (JObject)json["service_types"];
			var service = services.ObjectValuesOfProperties().FirstOrDefault(x => x.Value<string>("service_type") == serviceType);

			var countries = (JObject)service["countries"];
			return countries.StringValuesOfProperties().ToArray();
		}
		#endregion

		#region Add
		/// <summary>
		/// přidání balíku/balíků		/// </summary>
		/// <remarks>
		/// Přidává balík/balíky, které se odešlou ke svozu.
		/// </remarks>
		public async Task<BalikoBotPackage> Add(Carriers carrier, string eid, string serviceType, BalikoBotData data)
		{
			// EshopId
			data.AddSafe(BalikoBotData.EID, eid);
			// typ sluzby
			data.AddSafe(BalikoBotData.SERVICE_TYPE, serviceType);
			// typ sluzby
			data.AddSafe(BalikoBotData.ERRORS, true);

			var o = JObject.FromObject(data);
			var json = await PostAsync($"{API_SCHEMA}{API_URL_V1}/{carrier}/add", o);

			var result = json.ObjectValuesOfProperties().First();
			return new BalikoBotPackage()
			{
				EshopId = eid,
				CarrierId = (string)result["carrier_id"],
				PackageId = (int)result["package_id"],
				LabelUrl = (string)result["label_url"],
				Status = (int)result["status"],
			};
		}
		#endregion

		#region Drop
		/// <summary>
		/// odstranění balíku
		/// </summary>
		/// <remarks>
		/// Odstranit lze pouze zásilky, které ještě nebyly odeslány ke „svozu“ metodou ORDER.
		/// </remarks>
		public async Task<IEnumerable<BalikoBotResult>> Drop(Carriers carrier, params int[] packageIds)
		{
			if (packageIds == null || packageIds.Length == 0)
				throw new ArgumentException(nameof(packageIds));

			var data = packageIds.FromArrayToJArray((x, arr) =>
			{
				arr.Add(JObject.FromObject(new { id = x }));
			});

			var json = await PostAsync($"{API_SCHEMA}{API_URL_V1}/{carrier}/drop", data);

			return json.ForEachValuesOfProperties((o, x) => new BalikoBotResult()
			{
				Status = (int)o["status"]
			});
		}
		#endregion

		#region Track
		/// <summary>
		/// stav balíku
		/// </summary>
		/// <remarks>
		/// Vrací všechny stavy balíku/balíků, ve kterých se dosud ocitl s textovým popisem.
		/// </remarks>
		public async Task<IEnumerable<BalikoBotTrack>> Track(Carriers carrier, params string[] carrierIds)
		{
			if (carrierIds == null || carrierIds.Length == 0)
				throw new ArgumentException(nameof(carrierIds));

			var data = carrierIds.FromArrayToJArray((x, arr) =>
			{
				arr.Add(JObject.FromObject(new { id = x }));
			});

			var json = await PostAsync($"{API_SCHEMA}{API_URL_V2}/{carrier}/track", data);

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
		#endregion

		#region TrackStatus
		/// <summary>
		/// posledni stav balíku
		/// </summary>
		/// <remarks>
		/// Vrací poslední stav balíku/balíků ve formě čísla a textové prezentace.
		/// </remarks>
		public async Task<IEnumerable<BalikoBotTrackStatus>> TrackStatus(Carriers carrier, params string[] carrierIds)
		{
			if (carrierIds == null || carrierIds.Length == 0 || carrierIds.Length > 4)
				throw new ArgumentException(nameof(carrierIds));

			var data = carrierIds.FromArrayToJArray((x, arr) =>
			{
				arr.Add(JObject.FromObject(new { id = x }));
			});

			var json = await PostAsync($"{API_SCHEMA}{API_URL_V1}/{carrier}/track", data);

			return json.ForEachValuesOfProperties((o, x) => new BalikoBotTrackStatus()
			{
				CarrierId = carrierIds[x],
				Status = (BalikoBotTrackStatuses)(int)o["status_id"],
				StatusId = (int)o["status_id"],
				Text = (string)o["status_text"],
			});
		}
		#endregion

		#region Overview
		/// <summary>
		/// informace k poslednímu/konkrétnímu svozu
		/// </summary>
		/// <remarks>
		/// Soupis dosud neodeslaných balíků se základními informacemi.
		/// </remarks>
		public async Task<IEnumerable<BalikoBotPackage>> Overview(Carriers carrier)
		{
			var json = await PostAsync($"{API_SCHEMA}{API_URL_V1}/{carrier}/overview");

			return json.ForEachValuesOfProperties((o, x) => new BalikoBotPackage()
			{
				EshopId = (string)o["eshop_id"],
				CarrierId = (string)o["carrier_id"],
				PackageId = (int)o["package_id"],
				LabelUrl = (string)o["label_url"],
			});
		}
		#endregion

		#region Package
		/// <summary>
		/// zaslané údaje o konkrétním balíku a odkaz na štítek pro tisk
		/// </summary>
		/// <remarks>
		/// Vrací poslední stav balíku/balíků ve formě čísla a textové prezentace.
		/// </remarks>
		public async Task<BalikoBotData> Package(Carriers carrier, int packageId)
		{
			if (packageId == 0)
				throw new ArgumentException(nameof(packageId));

			var json = await PostAsync($"{API_SCHEMA}{API_URL_V1}/{carrier}/package/{packageId}");

			return json.ToObject<BalikoBotData>();
		}
		#endregion

		#region Labels
		/// <summary>
		/// vrací hromadné PDF se štítky pro vybrané balíky u konkrétního dopravce
		/// </summary>
		/// <remarks>
		/// Metoda vracející hromadné PDF se štítky pro vyžádané balíčky (package_ids) u vybraného dopravce.
		/// </remarks>
		public async Task<BalikoBotLabel> Labels(Carriers carrier, params int[] packageIds)
		{
			if (packageIds == null || packageIds.Length == 0)
				throw new ArgumentException(nameof(packageIds));

			var data = JObject.FromObject(new
			{
				package_ids = new JArray(packageIds)
			});

			var json = await PostAsync($"{API_SCHEMA}{API_URL_V1}/{carrier}/labels", data);

			return json.ToObject<BalikoBotLabel>();
		}
		#endregion

		#region Order
		/// <summary>
		/// předání dat přepravci – svozová dávka
		/// </summary>
		/// <remarks>
		/// Předání dat do systému přepravce („objednání svozu“) pro dosud neodeslané balíky.
		/// </remarks>
		public async Task<BalikoBotOrder> Order(Carriers carrier, params int[] packageIds)
		{
			if (packageIds == null || packageIds.Length == 0)
				throw new ArgumentException(nameof(packageIds));

			var data = JObject.FromObject(new
			{
				package_ids = new JArray(packageIds)
			});

			var json = await PostAsync($"{API_SCHEMA}{API_URL_V1}/{carrier}/order", data);

			return json.ToObject<BalikoBotOrder>();
		}
		#endregion

		#region OrderView
		/// <summary>
		/// informace k poslednímu/konkrétnímu svozu
		/// </summary>
		public async Task<BalikoBotOrder> OrderView(Carriers carrier, int? orderId = null)
		{
			var url = orderId.HasValue ? $"/{orderId}" : null;

			var json = await PostAsync($"{API_SCHEMA}{API_URL_V1}/{carrier}/orderview{url}");

			var result = json.ToObject<BalikoBotOrder>();

			// prevod ids z nestandardni 'dictionary' formy na bezne pole na vystup
			var ids = (JObject)json["package_ids"];
			if (ids != null)
			{
				result.PackageIds = ids.ObjectValuesOfProperties().ParseInt();
			}

			return result;
		}
		#endregion

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

			var unescaped = Regex.Unescape(content);
			var json = JObject.Parse(unescaped);
			return json;
		}
		#endregion
	}
}
