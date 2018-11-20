using BalikoBot;
using System;
using Xunit;

namespace BalikoBot.Tests
{
	public class BasicTest
	{
		#region Set up
		private BalikoBotClient GetClient()
		{
			return new BalikoBotClient("algorimczt", "XT2Hzgt2");
		}
		#endregion

		[Fact]
		public async void TestServices()
		{
			var client = GetClient();

			var r1 = await client.GetServices(Carriers.cp);
			Assert.NotEmpty(r1);

			var r2 = await client.GetServices(Carriers.ppl);
			Assert.NotEmpty(r2);
		}

		[Fact]
		public async void TestCountries4service()
		{
			var client = GetClient();

			var r2 = await client.GetCountries4service(Carriers.ppl, "2");
			Assert.NotEmpty(r2);

			var r1 = await client.GetCountries4service(Carriers.cp, "DR");
			Assert.Single(r1);
		}

		[Fact]
		public async void TestAdd()
		{
			var client = GetClient();

			var r1 = await client.Add(Carriers.cp, "123456789", "DR", 12345.85m, "CZK", "123123", 12300m, "+420777555666", "John Carter", "Palackého 12", "Praha 9", "19000", "CZ", "john@carter.com");
			Assert.NotNull(r1);
			Assert.NotEmpty(r1.CarrierId);
			Assert.True(r1.PackageId > 0);
			Assert.NotEmpty(r1.LabelUrl);
			Assert.True(r1.Status > 0);
		}
	}
}
