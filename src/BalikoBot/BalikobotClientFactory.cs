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

		#region Properties

		private BalikoBotClient _cpClient;
		public BalikoBotClient CpClient => _cpClient ?? (_cpClient = new BalikoBotClient(Carriers.cp, _config));

		private BalikoBotClient _dpdClient;
		public BalikoBotClient DpdClient => _dpdClient ?? (_dpdClient = new BalikoBotClient(Carriers.dpd, _config));

		private BalikoBotClient _geisClient;
		public BalikoBotClient GeisClient => _geisClient ?? (_geisClient = new BalikoBotClient(Carriers.geis, _config));

		private BalikoBotClient _glsClient;
		public BalikoBotClient GlsClient => _glsClient ?? (_glsClient = new BalikoBotClient(Carriers.gls, _config));

		private BalikoBotClient _intimeClient;
		public BalikoBotClient IntimeClient => _intimeClient ?? (_intimeClient = new BalikoBotClient(Carriers.intime, _config));

		private BalikoBotClient _pbhClient;
		public BalikoBotClient PbhClient => _pbhClient ?? (_pbhClient = new BalikoBotClient(Carriers.pbh, _config));

		private BalikoBotClient _pplClient;
		public BalikoBotClient PplClient => _pplClient ?? (_pplClient = new BalikoBotClient(Carriers.ppl, _config));

		private BalikoBotClient _spClient;
		public BalikoBotClient SpClient => _spClient ?? (_spClient = new BalikoBotClient(Carriers.sp, _config));

		private BalikoBotClient _toptransClient;
		public BalikoBotClient ToptransClient => _toptransClient ?? (_toptransClient = new BalikoBotClient(Carriers.toptrans, _config));

		private BalikoBotClient _ulozenkaClient;
		public BalikoBotClient UlozenkaClient => _ulozenkaClient ?? (_ulozenkaClient = new BalikoBotClient(Carriers.ulozenka, _config));

		private BalikoBotClient _zasilkovnaClient;
		public BalikoBotClient ZasilkovnaClient => _zasilkovnaClient ?? (_zasilkovnaClient = new BalikoBotClient(Carriers.zasilkovna, _config));

		#endregion

		public BalikoBotClient Get(Carriers carrier)
		{
			switch (carrier)
			{
				case Carriers.cp:
					return CpClient;

				case Carriers.dpd:
					return DpdClient;

				case Carriers.geis:
					return GeisClient;

				case Carriers.gls:
					return GlsClient;

				case Carriers.intime:
					return IntimeClient;

				case Carriers.pbh:
					return PbhClient;

				case Carriers.ppl:
					return PplClient;

				case Carriers.sp:
					return SpClient;

				case Carriers.toptrans:
					return ToptransClient;

				case Carriers.ulozenka:
					return UlozenkaClient;

				case Carriers.zasilkovna:
					return ZasilkovnaClient;

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

					if (_dpdClient != null)
					{
						_dpdClient = null;
					}

					if (_geisClient != null)
					{
						_geisClient = null;
					}

					if (_glsClient != null)
					{
						_glsClient = null;
					}

					if (_intimeClient != null)
					{
						_intimeClient = null;
					}

					if (_pbhClient != null)
					{
						_pbhClient = null;
					}

					if (_pplClient != null)
					{
						_pplClient = null;
					}

					if (_spClient != null)
					{
						_spClient = null;
					}

					if (_toptransClient != null)
					{
						_toptransClient = null;
					}

					if (_ulozenkaClient != null)
					{
						_ulozenkaClient = null;
					}

					if (_zasilkovnaClient != null)
					{
						_zasilkovnaClient = null;
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
