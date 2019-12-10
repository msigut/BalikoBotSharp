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
				.AddJsonFile("appsettings.json", true)
				.AddJsonFile("appsettings.workspace.json", true)
				.Build();

			// inicializace testovaci konfigurace
			Options = configuration.GetSection("BalikoBot").Get<TestOptions>();

			// DI
			var services = new ServiceCollection();
			services.AddSingleton<IBalikoBotConfiguration>(Options);
			services.AddScoped<BalikoBotClientFactory>();
			services.AddHttpClient();

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
