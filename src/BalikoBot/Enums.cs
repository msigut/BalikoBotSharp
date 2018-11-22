using System;
using System.Collections.Generic;
using System.Text;

namespace BalikoBot
{
	/// <summary>
	/// dopravci
	/// </summary>
	public enum Carriers
	{
		ppl,
		cp,
	}

	/// <summary>
	/// posledni stav baliku
	/// </summary>
	public enum BalikoBotTrackStatuses : int
	{
		NotInProcess = -1,
		Refused = 0,
		Delivered = 1,
		InDelivery = 2,
		Cancelled = 3,
		Returned = 4,
	}
}
