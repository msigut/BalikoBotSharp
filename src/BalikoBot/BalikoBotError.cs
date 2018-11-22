using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace BalikoBot
{
	/// <summary>
	/// detail chyby
	/// </summary>
	public class BalikoBotError
	{
		[DataMember(Name = "type")]
		public int Type { get; set; }

		[DataMember(Name = "attribute")]
		public string Attribute { get; set; }

		[DataMember(Name = "message")]
		public string Message { get; set; }
	}
}
