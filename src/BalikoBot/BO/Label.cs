using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

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
