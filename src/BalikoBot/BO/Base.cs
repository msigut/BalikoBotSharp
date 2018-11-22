using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace BalikoBot.BO
{
	/// <summary>
	/// vysledek volani BalikoBot
	/// </summary>
	[DataContract]
	public class BalikoBotResult
	{
		[DataMember]
		public int Status { get; set; }
	}
}
