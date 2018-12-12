using System.Runtime.Serialization;

namespace BalikoBot.BO
{
	/// <summary>
	/// konkretni PSC (ZIP)
	/// </summary>
	[DataContract]
	public class BalikoBotZipCodeItem
	{
		[DataMember(Name = "zip")]
		public string Zip { get; set; }

		[DataMember(Name = "zip_start")]
		public string ZipStart { get; set; }

		[DataMember(Name = "zip_end")]
		public string ZipEnd { get; set; }

		[DataMember(Name = "country")]
		public string Country { get; set; }

		[DataMember(Name = "city")]
		public string City { get; set; }
	}
}
