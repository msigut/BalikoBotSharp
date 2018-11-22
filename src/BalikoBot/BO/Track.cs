using System;
using System.Collections.Generic;
using System.Text;

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
