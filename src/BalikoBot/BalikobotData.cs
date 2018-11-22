using System.Collections.Generic;

namespace BalikoBot
{
	/// <summary>
	/// data balikobota
	/// </summary>
	public class BalikoBotData : Dictionary<string, object>
	{
		public static readonly string VS = "vs";
		public static readonly string EID = "eid";
		public static readonly string ESHOP_ID = "eshop_id";
		public static readonly string PRICE = "price";
		public static readonly string SERVICE_TYPE = "service_type";

		public static readonly string REC_EMAIL = "rec_email";
		public static readonly string REC_PHONE = "rec_phone";
		public static readonly string REC_NAME = "rec_name";
		public static readonly string REC_STREET = "rec_street";
		public static readonly string REC_CITY = "rec_city";
		public static readonly string REC_ZIP = "rec_zip";
		public static readonly string REC_COUNTRY = "rec_country";

		public static readonly string COD_PRICE = "cod_price";
		public static readonly string COD_CURRENCY = "cod_currency";

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

		#region Doruceni
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
		#endregion

		#region Dobirka
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
		#endregion

		#region Cena
		/// <summary>
		/// prida udaje bezne ceny baliku
		/// </summary>
		public BalikoBotData AddCena(decimal price)
		{
			AddSafe(PRICE, price);
			return this;
		}
		#endregion

		#region Execute
		/// <summary>
		/// bezpecne pridani klice
		/// </summary>
		internal void AddSafe(string key, object val)
		{
			if (!ContainsKey(key))
				Add(key, val);
		}
		#endregion
	}
}
