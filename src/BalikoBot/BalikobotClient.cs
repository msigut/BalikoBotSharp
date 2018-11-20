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
	/// dopravci
	/// </summary>
	public enum Carriers
	{
		ppl,
		cp,
	}

	/// <summary>
	/// sluzby
	/// </summary>
	public class BalikoBotService
	{
		public string Type { get; set; }
		public string Description { get; set; }
	}

	/// <summary>
	/// balik
	/// </summary>
	public class BalikoBotPackage
	{
		public int Status { get; set; }
		public string CarrierId { get; set; }
		public int PackageId { get; set; }
		public string LabelUrl { get; set; }
	}

	/// <summary>
	/// klinet BalikoBota
	/// </summary>
	public class BalikoBotClient
	{
		public static readonly string API_SCHEMA = "https://";
		public static readonly string API_URL = "api.balikobot.cz";

		#region Constructor
		private readonly string _login;
		private readonly string _password;
		public BalikoBotClient(string login, string password)
		{
			_login = login;
			_password = password;
		}
		#endregion

		#region Services
		/// <summary>
		/// sluzby podle dopravce
		/// </summary>
		public async Task<IEnumerable<BalikoBotService>> GetServices(Carriers carrier)
		{
			var json = await GetAsync($"{API_SCHEMA}{API_URL}/{carrier}/services");

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
			var json = await GetAsync($"{API_SCHEMA}{API_URL}/{carrier}/countries4service");

			var services = (JObject)json["service_types"];
			var service = services.ObjectValuesOfProperties().FirstOrDefault(x => x.Value<string>("service_type") == serviceType);

			var countries = (JObject)service["countries"];
			return countries.StringValuesOfProperties().ToArray();
		}
		#endregion

		#region Add
		/// <summary>
		/// Přidává balík/balíky, které se odešlou ke svozu.		/// </summary>
		public async Task<BalikoBotPackage> Add(Carriers carrier, string eid, string serviceType, decimal codPrice, string codCurrency, string vs, decimal price,
			string recPhone, string recName, string recStreet, string recCity, string recZIP, string recCountry, string recEmail)
		{
			var o = JObject.FromObject(new
			{
				eid,
				service_type = serviceType,
				cod_price = codPrice,
				cod_currency = codCurrency,
				vs,
				price,
				rec_phone = recPhone,
				rec_name = recName,
				rec_street = recStreet,
				rec_city = recCity,
				rec_email = recEmail,
				rec_zip = recZIP,
				rec_country = recCountry,
				return_full_errors = true,
			});

			var json = await PostAsync($"{API_SCHEMA}{API_URL}/{carrier}/add", o);

			var result = json.ObjectValuesOfProperties().First();
			return new BalikoBotPackage()
			{
				CarrierId = (string)result["carrier_id"],
				PackageId = (int)result["package_id"],
				LabelUrl = (string)result["label_url"],
				Status = int.Parse((string)result["status"]),
			};
		}
		#endregion

		#region Helpers
		private async Task<JObject> GetAsync(string url)
		{
			return await GetClientInternal(async (client) => await client.GetAsync(url));
		}
		private async Task<JObject> PostAsync(string url, JObject data)
		{
			return await GetClientInternal(async (client) => await client.PostAsync(url, new StringContent(data.ToString(Formatting.Indented), Encoding.UTF8, "application/json")));
		}

		private async Task<JObject> GetClientInternal(Func<HttpClient, Task<HttpResponseMessage>> todo)
		{
			var client = new HttpClient();
			var byteArray = Encoding.ASCII.GetBytes($"{_login}:{_password}");
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
