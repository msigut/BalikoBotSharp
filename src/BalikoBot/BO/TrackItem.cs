using System;

namespace BalikoBot.BO
{
	/// <summary>
	/// udalost ve sledovani
	/// </summary>
	public class BalikoBotTrackItem
	{
		public DateTime Date { get; set; }
		public string Name { get; set; }
		public BalikoBotTrackStatuses Status { get; set; }
		public int StatusId { get; set; }
	}
}
