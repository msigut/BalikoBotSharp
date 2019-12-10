using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BalikoBot.Tests
{
	public class BasicTest : IClassFixture<TestFixture>
	{
		#region DI

		private readonly BalikoBotClientFactory _balikoBot;

		public BasicTest(TestFixture test)
		{
			_balikoBot = test.Services.GetRequiredService<BalikoBotClientFactory>();
		}

		#endregion

		[Fact]
		public async Task TestServices()
		{
			var r1 = await _balikoBot.CpClient.Services();
			Assert.NotEmpty(r1);

			var r2 = await _balikoBot.PplClient.Services();
			Assert.NotEmpty(r2);
		}

		[Fact]
		public async Task TestCountries4service()
		{
			var r2 = await _balikoBot.PplClient.Countries4service("2");
			Assert.NotEmpty(r2);

			var r1 = await _balikoBot.CpClient.Countries4service("DR");
			Assert.Single(r1);
		}

		[Fact]
		public async Task TestZipCodes()
		{
			var r = await _balikoBot.PplClient.ZipCodes("3");
			Assert.NotNull(r);
			Assert.Equal("3", r.ServiceType);
			Assert.Equal(ZipTypes.Zip, r.Type);
			Assert.Contains(r.Items, x => x.Country == "CZ");
			Assert.Contains(r.Items, x => !string.IsNullOrEmpty(x.Zip));
		}

		[Fact]
		public async Task TestBranches()
		{
			var b = await _balikoBot.ZasilkovnaClient.Branches();
			Assert.NotNull(b);
			Assert.Contains(b.Items, x => !string.IsNullOrEmpty(x.BranchId));
			Assert.Contains(b.Items, x => x.Name == "Jablonec nad Nisou, Palackého LSC - IT Partner, IT Servis");
			Assert.Contains(b.Items, x => x.Street == "Potoky 33");
			Assert.Contains(b.Items, x => x.City == "Hanušovice");
			Assert.Contains(b.Items, x => x.ZIP == "736 01");
			Assert.Contains(b.Items, x => x.Country == "CZ");
		}

		[Fact]
		public async Task TestAddOverviewPackageDrop()
		{
			// 1. pridani baliku do svozu
			const string eid = "123001";
			var data = new BalikoBotData(eid, "DR")
				.AddDoruceni("john@carter.com", "+420777555666", "John Carter", "Palackého 12", "Praha 9", "19000", "CZ")
				.AddDobirka(12300m, eid, 12345.85m, "CZK");

			var res = await _balikoBot.CpClient.Add(data);
			Assert.NotEmpty(res);

			var r1 = res.FirstOrDefault();
			Assert.NotNull(r1);
			Assert.NotEmpty(r1.CarrierId);
			Assert.True(r1.PackageId > 0);
			Assert.NotEmpty(r1.LabelUrl);
			Assert.True(r1.Status == 200 || r1.Status == 208);

			// 2. overeni baliku
			var all = await _balikoBot.CpClient.Overview();
			Assert.NotEmpty(all);
			Assert.Contains(all, x => x.EshopId == eid);
			Assert.Contains(all, x => x.CarrierId == r1.CarrierId);
			Assert.Contains(all, x => x.PackageId == r1.PackageId);
			Assert.Contains(all, x => x.LabelUrl == r1.LabelUrl);

			// 3. vsechny informace o baliku
			var pkg = await _balikoBot.CpClient.Package(r1.PackageId);
			Assert.NotNull(pkg);
			Assert.Contains(pkg, x => x.Key == BalikoBotData.ESHOP_ID && x.Value.ToString() == eid);
			Assert.Contains(pkg, x => x.Key == BalikoBotData.REC_EMAIL && x.Value.ToString() == "john@carter.com");
			Assert.Contains(pkg, x => x.Key == BalikoBotData.REC_PHONE && x.Value.ToString() == "420777555666");

			// 4. zruseni baliku ze svozu
			var r2 = await _balikoBot.CpClient.Drop(r1.PackageId);
			Assert.Equal(200, r1.Status);
		}

		[Fact]
		public async Task TestLabelOrderTrack()
		{
			var client = _balikoBot.CpClient;

			// 0. sluzby
			var services = await client.Services();
			Assert.NotEmpty(services);

			// 1. pridani baliku do svozu (Balik do ruky: 'DR')
			var data = new BalikoBotData(DateTime.Now.ToString("yyyyMMddHHmmss"), "DR")
				.AddDoruceni("john@carter.com", "+420777555666", "John Carter", "Palackého 12", "Praha 9", "19000", "CZ")
				.AddCena(1450m, "CZK")
				.AddContent("prvni pokusna zasilka se zbozim - mixer");

			var res = await client.Add(data);
			Assert.NotEmpty(res);
			var r1 = res.FirstOrDefault();
			Assert.Equal(200, r1.Status);

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

			// 5. track
			var t = await client.Track(r1.CarrierId);
			Assert.NotEmpty(t);
			var t1 = t.FirstOrDefault();
			Assert.NotNull(t1);
			//Assert.NotEmpty(t1.Items);
			//Assert.Contains(t1.Items, x => x.Date != default);
			//Assert.Contains(t1.Items, x => x.StatusId >= -1 && x.StatusId <= 4);
			//Assert.Contains(t1.Items, x => !string.IsNullOrEmpty(x.Name));

			// 6. track status
			var tt = await client.TrackStatus(r1.CarrierId);
			Assert.NotEmpty(tt);
			var tt1 = tt.FirstOrDefault();
			Assert.True(tt1.StatusId >= -1);
			Assert.NotEmpty(tt1.CarrierId);
			Assert.NotEmpty(tt1.Text);
		}

		[Fact]
		public async Task TestCena()
		{
			// 1. CP - Recomando
			var data = new BalikoBotData(DateTime.Now.ToString("yyyyMMddHHmmss"), "rr")
				.AddDoruceni("john@carter.com", "+420777555666", "John Carter", "Palackého 12", "Praha 9", "19000", "CZ")
				.AddHmotnost(0.11m)
				.AddCena(1759m);

			var res = await _balikoBot.CpClient.Add(data);
			Assert.NotEmpty(res);
			var r1 = res.FirstOrDefault();
			Assert.Equal(200, r1.Status);

			// 2. vsechny informace o baliku (rozmery a hmotnost jsou zaokrouhlovany)
			var pkg = await _balikoBot.CpClient.Package(r1.PackageId);
			Assert.NotNull(pkg);
			Assert.Contains(pkg, x => x.Key == BalikoBotData.PRICE && x.Value.ToString() == "1759.00");
			Assert.Contains(pkg, x => x.Key == BalikoBotData.WEIGHT && x.Value.ToString() == "0.110");
		}

		[Fact]
		public async Task TestSizeWeight()
		{
			// 1. PPL - Firemni balik
			var data = new BalikoBotData(DateTime.Now.ToString("yyyyMMddHHmmss"), "8")
				.AddDoruceni("john@carter.com", "+420777555666", "John Carter", "Palackého 12", "Praha 9", "19000", "CZ")
				.AddCena(1450m)
				.AddRozmeryHmotnost(40.5m, 60m, 30m, 2.5m)
				.AddSafe(BalikoBotData.REC_FIRM, "Moje s.r.o.");

			var res = await _balikoBot.PplClient.Add(data);
			Assert.NotEmpty(res);
			var r1 = res.FirstOrDefault();
			Assert.Equal(200, r1.Status);

			// 2. vsechny informace o baliku (rozmery a hmotnost jsou zaokrouhlovany)
			var pkg = await _balikoBot.PplClient.Package(r1.PackageId);
			Assert.NotNull(pkg);
			Assert.Contains(pkg, x => x.Key == BalikoBotData.WIDTH && x.Value.ToString() == "40.00");
			Assert.Contains(pkg, x => x.Key == BalikoBotData.LENGTH && x.Value.ToString() == "60.00");
			Assert.Contains(pkg, x => x.Key == BalikoBotData.HEIGHT && x.Value.ToString() == "30.00");
			Assert.Contains(pkg, x => x.Key == BalikoBotData.WEIGHT && x.Value.ToString() == "2.00");
		}

		[Fact]
		public async Task TestCheckError()
		{
			// 1. kontrola baliku do svozu; chybi jmeno
			var data = new BalikoBotData("123001", "8").AddDoruceni("john@carter.com", "+420777555666", "", "Palackého 12", "Praha 9", "19000", "CZ");
			var ex = await Assert.ThrowsAsync<BalikoBotAddException>(async () => await _balikoBot.PplClient.Check(data));
			Assert.NotEmpty(ex.Errors);
			Assert.Contains(ex.Errors, x => x.Type == 406 && x.Attribute == BalikoBotData.REC_NAME);
			Assert.Contains(ex.Errors, x => x.Type == 406 && x.Attribute == BalikoBotData.REC_FIRM);

			// 2. pridani baliku do svozu; chybi cena
			data = new BalikoBotData("123001", "DR").AddDoruceni("john@carter.com", "+420777555666", "John Carter", "Palackého 12", "Praha 9", "19000", "CZ");
			ex = await Assert.ThrowsAsync<BalikoBotAddException>(async () => await _balikoBot.CpClient.Add(data));
			Assert.NotEmpty(ex.Errors);
			Assert.Contains(ex.Errors, x => x.Type == 406 && x.Attribute == BalikoBotData.PRICE);

			// 3. track neexistujici zasilky (testovaci prostredi posila vysledek vzdy, nelze otestovat)
			//var t = await client.Track("4613245");
			//Assert.NotEmpty(t);
		}

		[Fact]
		public void TestFactory()
		{
			var carriers = Enum.GetValues(typeof(Carriers)).Cast<Carriers>();

			Assert.All(carriers, x => Assert.NotNull(_balikoBot.Get(x)));
		}
	}
}
