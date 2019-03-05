using System.Runtime.Serialization;

namespace BalikoBot.BO
{
	/// <summary>
	/// konkretni pobocka
	/// </summary>
	[DataContract]
	public class BalikoBotBranchItem
	{
		[DataMember(Name = "type")]
		public BranchTypes Type { get; set; }

		[DataMember(Name = "id")]
		public string BranchId { get; set; }

		[DataMember(Name = "zip")]
		public string ZIP { get; set; }

		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "street")]
		public string Street { get; set; }

		[DataMember(Name = "city")]
		public string City { get; set; }

		[DataMember(Name = "country")]
		public string Country { get; set; }
	}
}
