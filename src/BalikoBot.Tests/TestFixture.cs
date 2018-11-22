using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace BalikoBot.Tests
{
	public class TestFixture : IDisposable
	{
		/// <summary>
		/// nastaveni testu
		/// </summary>
		public TestOptions Options;

		/// <summary>
		/// DI
		/// </summary>
		public IServiceProvider Services { get; private set; }

		/// <summary>
		/// initialize
		/// </summary>
		public TestFixture()
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();
				
			// inicializace testovaci konfigurace
			Options = new TestOptions();
			configuration.GetSection("BalikoBot").Bind(Options);

			// DI
			var services = new ServiceCollection();
			services.AddSingleton<IBalikoBotConfiguration>(Options);
			services.AddScoped<BalikoBotClientFactory>();
			Services = services.BuildServiceProvider();
		}

		/// <summary>
		/// clean up
		/// </summary>
		public void Dispose()
		{
		}
	}
}
