namespace BalikoBot.BO
{
	/// <summary>
	/// posledni stav zasilky
	/// </summary>
	public class BalikoBotTrackStatus
	{
		public string CarrierId { get; set; }
		public BalikoBotTrackStatuses Status { get; set; }
		public int StatusId { get; set; }
		public string Text { get; set; }
	}
}
