using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace BalikoBot
{
	/// <summary>
	/// internal Newtonsoft JSON extensions
	/// </summary>
	internal static class JsonExtensions
	{
		internal static IEnumerable<JToken> ObjectValuesOfProperties(this JObject o)
		{
			bool TryParse(string s) => int.TryParse(s, out var result);

			return o.Properties().Where(x => TryParse(x.Name)).Select(x => x.Value);
		}
		internal static IEnumerable<string> StringValuesOfProperties(this JObject o)
		{
			return o.Properties().SelectMany(x => x.Values<string>());
		}
		internal static int[] ParseInt(this IEnumerable<JToken> os)
		{
			return os.Select(t => int.Parse(t.ToString())).ToArray();
		}

		internal static JArray FromArrayToJArray<T>(this IEnumerable<T> items, Action<T, JArray> todo)
		{
			var result = new JArray();

			foreach (var item in items)
				todo(item, result);

			return result;
		}

		internal static IEnumerable<T> ForEachValuesOfProperties<T>(this JObject os, Func<JObject, int, T> todo)
		{
			var list = new List<T>();
			var x = 0;
			foreach (JObject o in os.ObjectValuesOfProperties())
			{
				list.Add(todo(o, x));
				x++;
			}

			return list;
		}
	}
}
