using System.Collections.Generic;

namespace BalikoBot
{
	/// <summary>
	/// data balikobota
	/// </summary>
	public class BalikoBotData : Dictionary<string, object>
	{
		/// <summary>
		/// Variabilní symbol platby, povinný pro dobírkové zásilky.
		/// </summary>
		public static readonly string VS = "vs";
		/// <summary>
		/// Unikátní ID balíku v rámci e-shopu(ů), které využívají stejné konfigurační údaje přepravce.
		/// </summary>
		public static readonly string EID = "eid";
		/// <summary>
		/// Unikátní ID balíku v rámci e-shopu(ů) (v návratových hodnotách z API pod názvem: eshop_id)
		/// </summary>
		public static readonly string ESHOP_ID = "eshop_id";
		/// <summary>
		/// Udaná cena balíku – používá se pak pro účely připojištění del_insurance.
		/// </summary>
		public static readonly string PRICE = "price";
		/// <summary>
		/// Typ sluzby
		/// </summary>
		public static readonly string SERVICE_TYPE = "service_type";
		/// <summary>
		/// Cislo objednavky.
		/// Povinný pro sdružené zásilky, u samostatných se nemusí uvádět.
		/// </summary>
		public static readonly string ORDER_NUMBER = "order_number";
		/// <summary>
		/// Poznámka přepravci (maximální délka 350 znaků, delší text se zkrátí)
		/// </summary>
		public static readonly string NOTE = "note";

		/// <summary>
		/// E-mail příjemce.
		/// </summary>
		public static readonly string REC_EMAIL = "rec_email";
		/// <summary>
		/// Telefonní číslo příjemce, povinné při využití služby oznámení pomocí SMS
		/// nebo jiném telefonickém kontaktu – formát čísla(uvádějte i s předvolbou): +AAAXXXYYYZZZ
		/// </summary>
		public static readonly string REC_PHONE = "rec_phone";
		/// <summary>
		/// Jméno a příjmení příjemce.
		/// </summary>
		public static readonly string REC_NAME = "rec_name";
		/// <summary>
		/// Název firmy příjemce.
		/// </summary>
		public static readonly string REC_FIRM = "rec_firm";
		/// <summary>
		/// Ulice příjemce.
		/// </summary>
		public static readonly string REC_STREET = "rec_street";
		/// <summary>
		/// Adresační město příjemce.
		/// </summary>
		public static readonly string REC_CITY = "rec_city";
		/// <summary>
		/// O PSČ příjemce, uvádějte bez mezer ve formátu XXXXX
		/// </summary>
		public static readonly string REC_ZIP = "rec_zip";
		/// <summary>
		/// Kód země příjemce dle ISO 3166-1 alpha-2 (viz. http://cs.wikipedia.org/wiki/ISO_3166-1),
		/// výchozí hodnotou je Česká republika(CZ), pro zásilky mimo ČR je toto pole povinné
		/// </summary>
		public static readonly string REC_COUNTRY = "rec_country";

		/// <summary>
		/// Cena dobírky, povinná u všech dobírkových služeb
		/// </summary>
		public static readonly string COD_PRICE = "cod_price";
		/// <summary>
		/// Měna dobírky dle ISO 4217 kódu http://en.wikipedia.org/wiki/ISO_4217
		/// </summary>
		public static readonly string COD_CURRENCY = "cod_currency";

		/// <summary>
		/// Šířka balíku v cm, datový typ float.
		/// </summary>
		public static readonly string WIDTH = "width";
		/// <summary>
		/// Délka balíku v cm, datový typ float.
		/// </summary>
		public static readonly string LENGTH = "length";
		/// <summary>
		/// Výška balíku v cm, datový typ float.
		/// </summary>
		public static readonly string HEIGHT = "height";
		/// <summary>
		/// Váha balíku v kg, datový typ float.
		/// </summary>
		public static readonly string WEIGHT = "weight";

		/// <summary>
		/// Pro navrácení chyb v textové podobě namísto standardních číselných kódů
		/// zašlete hodnotu „1“ nebo TRUE(boolean).
		/// </summary>
		public static readonly string ERRORS = "return_full_errors";

		#region Constructor

		public BalikoBotData()
		{
		}
		public BalikoBotData(string eid, string serviceType, bool errors = true) : this()
		{
			// EshopId
			AddSafe(BalikoBotData.EID, eid);
			// typ sluzby
			AddSafe(BalikoBotData.SERVICE_TYPE, serviceType);
			// detailni chybove hlaseni
			AddSafe(BalikoBotData.ERRORS, errors);
		}

		#endregion

		#region Helpers

		/// <summary>
		/// prida udaje o doruceni
		/// </summary>
		public BalikoBotData AddDoruceni(string recEmail, string recPhone, string recName, string recStreet, string recCity, string recZIP, string recCountry)
		{
			AddSafe(REC_EMAIL, recEmail);
			AddSafe(REC_PHONE, recPhone);
			AddSafe(REC_NAME, recName);
			AddSafe(REC_STREET, recStreet);
			AddSafe(REC_CITY, recCity);
			AddSafe(REC_ZIP, recZIP);
			AddSafe(REC_COUNTRY, recCountry);
			return this;
		}

		/// <summary>
		/// prida udaje dobirky
		/// </summary>
		public BalikoBotData AddDobirka(decimal price, string vs, decimal codPrice, string codCurrency)
		{
			AddSafe(PRICE, price);
			AddSafe(VS, vs);
			AddSafe(COD_PRICE, codPrice);
			AddSafe(COD_CURRENCY, codCurrency);
			return this;
		}

		/// <summary>
		/// prida udaje bezne ceny baliku
		/// </summary>
		public BalikoBotData AddCena(decimal price)
		{
			AddSafe(PRICE, price);
			return this;
		}

		/// <summary>
		/// prida rozmeny a hmotnost dobirky
		/// </summary>
		public BalikoBotData AddRozmeryHmotnost(decimal width, decimal length, decimal height, decimal weight)
		{
			AddSafe(WIDTH, width);
			AddSafe(LENGTH, length);
			AddSafe(HEIGHT, height);
			AddSafe(WEIGHT, weight);
			return this;
		}

		/// <summary>
		/// prida rozmery dobirky
		/// </summary>
		public BalikoBotData AddRozmery(decimal width, decimal length, decimal height)
		{
			AddSafe(WIDTH, width);
			AddSafe(LENGTH, length);
			AddSafe(HEIGHT, height);
			return this;
		}

		/// <summary>
		/// prida udaje dobirky
		/// </summary>
		public BalikoBotData AddHmotnost(decimal weight)
		{
			AddSafe(WEIGHT, weight);
			return this;
		}

		/// <summary>
		/// bezpecne pridani klice
		/// </summary>
		public BalikoBotData AddSafe(string key, object val)
		{
			if (!ContainsKey(key))
				Add(key, val);

			return this;
		}
		#endregion
	}
}
