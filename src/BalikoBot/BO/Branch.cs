using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace BalikoBot.BO
{
	/// <summary>
	/// pobocka
	/// </summary>
	[DataContract]
	public class BalikoBotBranch : BalikoBotResult
	{
		[DataMember(Name = "service_type")]
		public string ServiceType { get; set; }

		public IEnumerable<BalikoBotBranchItem> Items { get; set; }
	}
}
