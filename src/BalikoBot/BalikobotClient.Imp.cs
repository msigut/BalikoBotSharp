using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalikoBot
{
	/// <summary>
	/// klient BalikoBot, Ceska posta
	/// </summary>
	public class BalikoBotCeskaPostaClient : BalikoBotClient
	{
		#region Constructor
		public BalikoBotCeskaPostaClient(string login, string password) : base(Carriers.cp, login, password)
		{
		}
		public BalikoBotCeskaPostaClient(IBalikoBotConfiguration config) : base(Carriers.cp, config)
		{
		}
		#endregion
	}

	/// <summary>
	/// klient BalikoBot, PPL
	/// </summary>
	public class BalikoBotPPLClient : BalikoBotClient
	{
		#region Constructor
		public BalikoBotPPLClient(string login, string password) : base(Carriers.ppl, login, password)
		{
		}
		public BalikoBotPPLClient(IBalikoBotConfiguration config) : base(Carriers.ppl, config)
		{
		}
		#endregion
	}
}
