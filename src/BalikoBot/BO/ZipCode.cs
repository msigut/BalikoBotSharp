using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BalikoBot.BO
{
	/// <summary>
	/// PSC (ZIP)
	/// </summary>
	[DataContract]
	public class BalikoBotZipCode : BalikoBotResult
	{
		[DataMember(Name = "service_type")]
		public string ServiceType { get; set; }

		[DataMember(Name = "type")]
		public ZipTypes Type { get; set; }

		public IEnumerable<BalikoBotZipCodeItem> Items { get; set; }
	}
}
