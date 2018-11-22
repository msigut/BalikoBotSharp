using BalikoBot;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace BalikoBot.Tests
{
	public class BasicTest
	{
		private static readonly TestOptions Options;

		#region Constuctor
		static BasicTest()
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();

			// inicializace testovaci konfigurace
			Options = new TestOptions();
			configuration.GetSection("BalikoBot").Bind(Options);
		}
		#endregion

		[Fact]
		public async Task TestServices()
		{
			var client = new BalikoBotClient(Options);

			var r1 = await client.GetServices(Carriers.cp);
			Assert.NotEmpty(r1);

			var r2 = await client.GetServices(Carriers.ppl);
			Assert.NotEmpty(r2);
		}

		[Fact]
		public async Task TestCountries4service()
		{
			var client = new BalikoBotClient(Options);

			var r2 = await client.GetCountries4service(Carriers.ppl, "2");
			Assert.NotEmpty(r2);

			var r1 = await client.GetCountries4service(Carriers.cp, "DR");
			Assert.Single(r1);
		}

		[Fact]
		public async Task TestAddOverviewPackageDrop()
		{
			var client = new BalikoBotClient(Options);

			// 1. pridani baliku do svozu
			const string eid = "123001";
			var data = new BalikoBotData()
				.AddDoruceni("john@carter.com", "+420777555666", "John Carter", "Palackého 12", "Praha 9", "19000", "CZ")
				.AddDobirka(12300m, eid, 12345.85m, "CZK");

			var r1 = await client.Add(Carriers.cp, eid, "DR", data);
			Assert.NotNull(r1);
			Assert.NotEmpty(r1.CarrierId);
			Assert.True(r1.PackageId > 0);
			Assert.NotEmpty(r1.LabelUrl);
			Assert.True(r1.Status > 0);

			// 2. overeni baliku
			var all = await client.Overview(Carriers.cp);
			Assert.NotEmpty(all);
			Assert.Contains(all, x => x.EshopId == eid);
			Assert.Contains(all, x => x.CarrierId == r1.CarrierId);
			Assert.Contains(all, x => x.PackageId == r1.PackageId);
			Assert.Contains(all, x => x.LabelUrl == r1.LabelUrl);

			// 3. vsechny informace o baliku
			var pkg = await client.Package(Carriers.cp, r1.PackageId);
			Assert.NotNull(pkg);
			Assert.Contains(pkg, x => x.Key == BalikoBotData.ESHOP_ID && x.Value.ToString() == eid);
			Assert.Contains(pkg, x => x.Key == BalikoBotData.REC_EMAIL && x.Value.ToString() == "john@carter.com");
			Assert.Contains(pkg, x => x.Key == BalikoBotData.REC_PHONE && x.Value.ToString() == "420777555666");

			// 4. zruseni baliku ze svozu
			var r2 = await client.Drop(Carriers.cp, r1.PackageId);
			Assert.Equal(200, r1.Status);
		}

		[Fact]
		public async Task TestLabelOrder()
		{
			var client = new BalikoBotClient(Options);

			// 1. pridani baliku do svozu
			var data = new BalikoBotData()
				.AddDoruceni("john@carter.com", "+420777555666", "John Carter", "Palackého 12", "Praha 9", "19000", "CZ")
				.AddCena(1450m);

			var eid = DateTime.Now.ToString("yyyyMMddHHmmss");
			var r1 = await client.Add(Carriers.cp, eid, "DR", data);

			// 2. vyzvedne stitky
			var l1 = await client.Labels(Carriers.cp, r1.PackageId);
			Assert.NotEmpty(l1.LabelUrl);
			Assert.Equal(200, l1.Status);

			// 3. objedna svoz
			var o1 = await client.Order(Carriers.cp, r1.PackageId);
			Assert.True(o1.OrderId > 0);
			Assert.NotEmpty(o1.FileUrl);
			Assert.NotEmpty(o1.HandoverUrl);
			Assert.NotEmpty(o1.LabelUrl);
			Assert.Equal(200, o1.Status);

			// 4. informace ke konkretni objednavce svozu
			var o2 = await client.OrderView(Carriers.cp, o1.OrderId);
			Assert.True(o2.OrderId > 0);
			Assert.NotEmpty(o2.PackageIds);
		}
	}
}
