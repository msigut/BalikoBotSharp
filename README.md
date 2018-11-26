## BalikoBotSharp

BalikoBot .NET Standard 2.0 (netstandard2.0). By [BalikoBot](https://www.balikobot.cz/) documentation [1.843](https://www.balikobot.cz/dokumentace/Balikobot-dokumentace-API.pdf).

Supports
- Dependency injection by Factory pattern **BalikoBotFactory**
- Use **Add** function error messages and general **Status** codes of API
- All common API functions by reference

**Config**

Implement **IBalikoBotConfiguration** to your config file or use More in [BalikoBot.Tests](/src/BalikoBot.Tests) project, file: [TestOptions.cs](/src/BalikoBot.Tests/TestOptions.cs).
```
public class TestOptions : IBalikoBotConfiguration
{
	public string Username { get; set; }
	public string Password { get; set; }
}
```

**Start**
```
_balikoBot = new BalikoBotClientFactory(options);
```

**With Dependency injection**

More in [BalikoBot.Tests](/src/BalikoBot.Tests) project, file: [TestFixture.cs](/src/BalikoBot.Tests/TestFixture.cs) and [BasicTest.cs](/src/BalikoBot.Tests/BasicTest.cs).
```
// DI configuration
services.AddSingleton<IBalikoBotConfiguration>(Options);
services.AddScoped<BalikoBotClientFactory>();

// in constructor
_balikoBot = test.Services.GetRequiredService<BalikoBotClientFactory>();
```

**Add** package to the front. With address, cost of delivery (COD), sizes and weight and all possible API parameters (by API documentation).
```
var data = new BalikoBotData(DateTime.Now.ToString("yyyyMMddHHmmss"), "8")
	.AddDoruceni("john@carter.com", "+420777555666", "John Carter", "Hlavni 12", "Praha 9", "19000", "CZ")
	.AddCena(1450m)
	.AddRozmeryHmotnost(40.5m, 60m, 30m, 2.5m)
	.AddSafe(BalikoBotData.REC_FIRM, "Moje s.r.o.");

await _balikoBot.PplClient.Add(data);
```

**Check** do everything checks as **Add**.
```
await _balikoBot.PplClient.Check(data)
```

**Drop** package from front.
```
await _balikoBot.CpClient.Drop(r1.PackageId);
```

**Overview** method to get list of packages in the front (added by **Add** method).
```
await _balikoBot.CpClient.Overview();
```

**Package** returns all package data from API.
```
await _balikoBot.CpClient.Package(packageId);
```

**Labels** create link of Labels PDF for given packages.
```
await _balikoBot.CpClient.Labels(packageId);
```

**Order** order carrier for package.
```
await _balikoBot.CpClient.Order(packageId);
```

**OrderView** get list of ordered packages.
```
await _balikoBot.CpClient.OrderView(orderId);
```

**Track** method to get list of tracking information.
```
await _balikoBot.CpClient.Track(carrierId);
```

**TrackStatus** get last tracking status.
```
await _balikoBot.CpClient.TrackStatus(carrierId);
```

### Other
- `await _balikoBot.CpClient.Services();` - list of all carier **services**
- `await _balikoBot.CpClient.Countries4service(serviceType);` - list of all **countries** for service
- `await _balikoBot.PplClient.ZipCodes(serviceType);` - list of all **ZIP codes** for service
- `await _balikoBot.ZasilkovnaClient.Branches(serviceType);` - list of all **Branches** for service

