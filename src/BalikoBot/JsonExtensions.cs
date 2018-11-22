using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BalikoBot
{
	public static class JsonExtensions
	{
		public static IEnumerable<JToken> ObjectValuesOfProperties(this JObject o)
		{
			Func<string, bool> tryParse = (s) => int.TryParse(s, out int result);

			return o.Properties().Where(x => tryParse(x.Name)).Select(x => x.Value);
		}
		public static IEnumerable<string> StringValuesOfProperties(this JObject o)
		{
			return o.Properties().SelectMany(x => x.Values<string>());
		}
		public static int[] ParseInt(this IEnumerable<JToken> os)
		{
			return os.Select(t => int.Parse(t.ToString())).ToArray();
		}

		public static JArray FromArrayToJArray<T>(this IEnumerable<T> items, Action<T, JArray> todo)
		{
			var result = new JArray();

			foreach (T item in items)
				todo(item, result);

			return result;
		}

		public static IEnumerable<T> ForEachValuesOfProperties<T>(this JObject os, Func<JObject, int, T> todo)
		{
			var list = new List<T>();

			int x = 0;
			foreach (JObject o in os.ObjectValuesOfProperties())
			{
				list.Add(todo(o, x));
				x++;
			}

			return list;
		}
	}
}
