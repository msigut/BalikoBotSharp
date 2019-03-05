using System.Collections.Generic;

namespace BalikoBot.BO
{
	/// <summary>
	/// sledovani
	/// </summary>
	public class BalikoBotTrack
	{
		public string CarrierId { get; set; }
		public int? Status { get; set; }
		public IEnumerable<BalikoBotTrackItem> Items { get; set; }
	}
}
