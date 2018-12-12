using System.Collections.Generic;

namespace BalikoBot.BO
{
	/// <summary>
	/// sledovani
	/// </summary>
	public class BalikoBotTrack : BalikoBotResult
	{
		public string CarrierId { get; set; }
		public IEnumerable<BalikoBotTrackItem> Items { get; set; }
	}
}
