## BalikoBotSharp

BalikoBot .NET Core Library (for netstandard2.0). By [BalikoBot](https://www.balikobot.cz/) documentation [1.843](https://www.balikobot.cz/dokumentace/Balikobot-dokumentace-API.pdf).

Supports
- Dependency injection by Factory pattern `BalikoBotFactory`
- Use `Add` Error messages and general `Status` codes of API

**Config**

Implement `IBalikoBotConfiguration` to your config file or use More in [BalikoBot.Tests](/src/BalikoBot.Tests) project, file: [TestOptions.cs](/src/BalikoBot.Tests/TestOptions.cs).
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

**Add**
```
var data = new BalikoBotData(DateTime.Now.ToString("yyyyMMddHHmmss"), "8")
	.AddDoruceni("john@carter.com", "+420777555666", "John Carter", "Hlavni 12", "Praha 9", "19000", "CZ")
	.AddCena(1450m)
	.AddRozmeryHmotnost(40.5m, 60m, 30m, 2.5m)
	.AddSafe(BalikoBotData.REC_FIRM, "Moje s.r.o.");

await _balikoBot.PplClient.Add(data);
```

**Drop**
```
await _balikoBot.CpClient.Drop(r1.PackageId);
```

**Overview**
```
await _balikoBot.CpClient.Overview();
```

**Package**
```
await _balikoBot.CpClient.Package(packageId);
```

**Labels**
```
await _balikoBot.CpClient.Labels(packageId);
```

**Order**
```
await _balikoBot.CpClient.Order(packageId);
```

**OrderView**
```
await _balikoBot.CpClient.OrderView(orderId);
```

**Track**
```
await _balikoBot.CpClient.Track(carrierId);
```

**TrackStatus**
```
await _balikoBot.CpClient.TrackStatus(carrierId);
```

### Other
```
await _balikoBot.CpClient.Services();
await _balikoBot.CpClient.Countries4service("DR");
await _balikoBot.PplClient.ZipCodes("3");
await _balikoBot.ZasilkovnaClient.Branches();
```
