using System;
using System.Collections.Generic;
using System.Text;

namespace BalikoBot
{
	/// <summary>
	/// konfigurace BalikoBotu
	/// </summary>
	public interface IBalikoBotConfiguration
	{
		string Username { get; }
		string Password { get; }
	}
}
