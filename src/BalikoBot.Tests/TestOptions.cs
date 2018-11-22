using System;
using System.Collections.Generic;
using System.Text;
using BalikoBot;

namespace BalikoBot.Tests
{
	/// <summary>
	/// konfigurace UNIT testu BalikoBota
	/// </summary>
	public class TestOptions : IBalikoBotConfiguration
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}
}
