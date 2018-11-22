using BalikoBot.BO;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BalikoBot
{
	/// <summary>
	/// detail chyby
	/// </summary>
	public class BalikoBotError
	{
		[DataMember(Name = "type")]
		public int Type { get; set; }

		[DataMember(Name = "attribute")]
		public string Attribute { get; set; }

		[DataMember(Name = "message")]
		public string Message { get; set; }
	}

	/// <summary>
	/// vyjimka BalikoBotu na metode Add
	/// </summary>
	public class BalikoBotAddException : Exception
	{
		/// <summary>
		/// chyby z API BalikoBotu
		/// </summary>
		public List<BalikoBotError> Errors { get; private set; }

		internal BalikoBotAddException(BalikoBotData[] datas, JObject json)
		{
			Errors = new List<BalikoBotError>();

			json.ForEachValuesOfProperties((o, x) =>
			{
				var temp = (JObject)o["errors"];
				if (temp == null)
					return false;

				Errors.AddRange(temp.ForEachValuesOfProperties((oo, xx) => oo.ToObject<BalikoBotError>()));
				return true;
			});
		}
	}
}
