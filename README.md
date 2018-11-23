## BalikoBotSharp

BalikoBot .NET Core Library (for netstandard2.0). By [BalikoBot](https://www.balikobot.cz/) documentation [1.843](https://www.balikobot.cz/dokumentace/Balikobot-dokumentace-API.pdf).

Supports
- Dependency injection by Factory pattern `BalikoBotFactory`
- `Add` Error messages and `Status` codes

**Add**
```
var data = new BalikoBotData(DateTime.Now.ToString("yyyyMMddHHmmss"), "8")
	.AddDoruceni("john@carter.com", "+420777555666", "John Carter", "Hlavni 12", "Praha 9", "19000", "CZ")
	.AddCena(1450m)
	.AddRozmeryHmotnost(40.5m, 60m, 30m, 2.5m)
	.AddSafe(BalikoBotData.REC_FIRM, "Moje s.r.o.");

await _balikoBot.PplClient.Add(data);
```

**Labels**
```
await _balikoBot.CpClient.Labels(packageId)
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
