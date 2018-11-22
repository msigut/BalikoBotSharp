using System;

namespace BalikoBot
{
	public class BalikoBotClientFactory : IDisposable
	{
		#region DI

		private IBalikoBotConfiguration _config;

		public BalikoBotClientFactory(IBalikoBotConfiguration config)
		{
			_config = config ?? throw new ArgumentNullException(nameof(config));
		}

		#endregion

		private BalikoBotClient _cpClient;
		public BalikoBotClient CpClient => _cpClient ?? (_cpClient = new BalikoBotClient(Carriers.cp, _config));

		private BalikoBotClient _pplClient;
		public BalikoBotClient PplClient => _pplClient ?? (_pplClient = new BalikoBotClient(Carriers.ppl, _config));

		public BalikoBotClient Get(Carriers carrier)
		{
			switch (carrier)
			{
				case Carriers.cp:
					return CpClient;

				case Carriers.ppl:
					return PplClient;

				default:
					throw new InvalidOperationException($"Undefined behavior for '{carrier}'");
			}
		}

		#region IDisposable

		private bool _disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					if (_cpClient != null)
					{
						_cpClient = null;
					}

					if (_pplClient != null)
					{
						_pplClient = null;
					}
				}

				_disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}

		#endregion
	}
}
