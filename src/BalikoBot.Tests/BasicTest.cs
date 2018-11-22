using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
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
			var cp = new BalikoBotCeskaPostaClient(Options);
			var r1 = await cp.GetServices();
			Assert.NotEmpty(r1);

			var ppl = new BalikoBotPPLClient(Options);
			var r2 = await ppl.GetServices();
			Assert.NotEmpty(r2);
		}

		[Fact]
		public async Task TestCountries4service()
		{
			var ppl = new BalikoBotPPLClient(Options);
			var r2 = await ppl.GetCountries4service("2");
			Assert.NotEmpty(r2);

			var cp = new BalikoBotCeskaPostaClient(Options);
			var r1 = await cp.GetCountries4service("DR");
			Assert.Single(r1);
		}

		[Fact]
		public async Task TestAddOverviewPackageDrop()
		{
			var client = new BalikoBotCeskaPostaClient(Options);

			// 1. pridani baliku do svozu
			const string eid = "123001";
			var data = new BalikoBotData(eid, "DR")
				.AddDoruceni("john@carter.com", "+420777555666", "John Carter", "Palackého 12", "Praha 9", "19000", "CZ")
				.AddDobirka(12300m, eid, 12345.85m, "CZK");

			var res = await client.Add(data);
			Assert.NotEmpty(res);

			var r1 = res.FirstOrDefault();
			Assert.NotNull(r1);
			Assert.NotEmpty(r1.CarrierId);
			Assert.True(r1.PackageId > 0);
			Assert.NotEmpty(r1.LabelUrl);
			Assert.True(r1.Status > 0);

			// 2. overeni baliku
			var all = await client.Overview();
			Assert.NotEmpty(all);
			Assert.Contains(all, x => x.EshopId == eid);
			Assert.Contains(all, x => x.CarrierId == r1.CarrierId);
			Assert.Contains(all, x => x.PackageId == r1.PackageId);
			Assert.Contains(all, x => x.LabelUrl == r1.LabelUrl);

			// 3. vsechny informace o baliku
			var pkg = await client.Package(r1.PackageId);
			Assert.NotNull(pkg);
			Assert.Contains(pkg, x => x.Key == BalikoBotData.ESHOP_ID && x.Value.ToString() == eid);
			Assert.Contains(pkg, x => x.Key == BalikoBotData.REC_EMAIL && x.Value.ToString() == "john@carter.com");
			Assert.Contains(pkg, x => x.Key == BalikoBotData.REC_PHONE && x.Value.ToString() == "420777555666");

			// 4. zruseni baliku ze svozu
			var r2 = await client.Drop(r1.PackageId);
			Assert.Equal(200, r1.Status);
		}

		[Fact]
		public async Task TestLabelOrder()
		{
			var client = new BalikoBotCeskaPostaClient(Options);

			// 1. pridani baliku do svozu
			var data = new BalikoBotData(DateTime.Now.ToString("yyyyMMddHHmmss"), "DR")
				.AddDoruceni("john@carter.com", "+420777555666", "John Carter", "Palackého 12", "Praha 9", "19000", "CZ")
				.AddCena(1450m);

			var res = await client.Add(data);
			Assert.NotEmpty(res);

			var r1 = res.FirstOrDefault();

			// 2. vyzvedne stitky
			var l1 = await client.Labels(r1.PackageId);
			Assert.NotEmpty(l1.LabelUrl);
			Assert.Equal(200, l1.Status);

			// 3. objedna svoz
			var o1 = await client.Order(r1.PackageId);
			Assert.True(o1.OrderId > 0);
			Assert.NotEmpty(o1.FileUrl);
			Assert.NotEmpty(o1.HandoverUrl);
			Assert.NotEmpty(o1.LabelUrl);
			Assert.Equal(200, o1.Status);

			// 4. informace ke konkretni objednavce svozu
			var o2 = await client.OrderView(o1.OrderId);
			Assert.True(o2.OrderId > 0);
			Assert.NotEmpty(o2.PackageIds);
		}

		[Fact]
		public async Task TestError()
		{
			var client = new BalikoBotCeskaPostaClient(Options);

			// 1. pridani baliku do svozu; chybi cena
			var data = new BalikoBotData("123001", "DR").AddDoruceni("john@carter.com", "+420777555666", "John Carter", "Palackého 12", "Praha 9", "19000", "CZ");
			var ex = await Assert.ThrowsAsync<BalikoBotAddException>(async () => await client.Add(data));
			Assert.NotEmpty(ex.Errors);
			Assert.Contains(ex.Errors, x => x.Type == 406 && x.Attribute == BalikoBotData.PRICE);
		}
	}
}
