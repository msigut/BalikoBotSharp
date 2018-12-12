using System.Runtime.Serialization;

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
