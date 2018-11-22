using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace BalikoBot.BO
{
	/// <summary>
	/// objednavka
	/// </summary>
	[DataContract]
	public class BalikoBotOrder : BalikoBotResult
	{
		[DataMember(Name = "order_id")]
		public int OrderId { get; set; }

		[DataMember(Name = "file_url")]
		public string FileUrl { get; set; }

		[DataMember(Name = "handover_url")]
		public string HandoverUrl { get; set; }

		[DataMember(Name = "labels_url")]
		public string LabelUrl { get; set; }

		public int[] PackageIds { get; set; } 
	}
}
