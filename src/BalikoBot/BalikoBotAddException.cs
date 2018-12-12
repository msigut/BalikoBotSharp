using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace BalikoBot
{
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
