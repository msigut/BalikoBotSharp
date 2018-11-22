using System;
using System.Collections.Generic;
using System.Text;

namespace BalikoBot.BO
{
	/// <summary>
	/// balik
	/// </summary>
	public class BalikoBotPackage : BalikoBotResult
	{
		public string EshopId { get; set; }
		public string CarrierId { get; set; }
		public int PackageId { get; set; }
		public string LabelUrl { get; set; }
	}
}
