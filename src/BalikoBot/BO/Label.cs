using System.Runtime.Serialization;

namespace BalikoBot.BO
{
	/// <summary>
	/// hromadne stitky
	/// </summary>
	[DataContract]
	public class BalikoBotLabel : BalikoBotResult
	{
		[DataMember(Name = "labels_url")]
		public string LabelUrl { get; set; }
	}
}
