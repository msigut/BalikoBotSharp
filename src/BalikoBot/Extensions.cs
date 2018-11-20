using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BalikoBot
{
	public static class Extensions
	{
		public static IEnumerable<JObject> ObjectValuesOfProperties(this JObject o)
		{
			return o.Properties().Select(x => (JObject)x.Value);
		}
		public static IEnumerable<string> StringValuesOfProperties(this JObject o)
		{
			return o.Properties().SelectMany(x => x.Values<string>());
		}
	}
}
