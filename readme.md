# FadeJSON

The fastest dynamic Deserialization JSON library in .NET Platform.



## Features

- Lightweight. 6 files above, 13KiB after compilation.
- Fast dynamic Deserialization.
- Low memory usage. CLR GC-friendly.




## Getting Started

[Binaries here](https://github.com/YangFan789/FadeJSON/releases)

```c#
var json = FadeJSON.Json.FromString("{\"key\":\"123\""}");
var v = json["key"].Value; // v == 123
```

FadeJSON v4 **only** support  .NET Framework 4.6.

If you are using other versions of .NET Framework, please using FadeJSON v3.



## Benchmarks

Tester and test suites can be found in `FadeJson.ConsoleTests`

### Deserialization Performance

`NetJSON` is excluded because it doesn't support deserialization for dynamic object.

Time Unit is millisecond. The lower the better.

|                            | FadeJSON | Jil  | JSON.NET | SimpleJson | jsonfx |
| -------------------------- | -------- | ---- | -------- | ---------- | ------ |
| `auctions.json`            | 7288     | 9213 | 15768    | 14872      | 60028  |
| `data.json`                | 1093     | 1179 | 1284     | 1464       | 13875  |
| `data1.json`               | 18       | 35   | 65       | 79         | 199    |
| `SkipWhitespaceTest1.json` | 42       | 142  | 23       | 120        | 174    |
| `TestObject.json`          | 38       | 69   | 97       | 161        | 608    |
| `twitter.json`             | 105      | 113  | 153      | 286        | 1342   |

### Parse Validation

Use `JSON_Checker` test suites to test whether the library can identify valid and invalid JSONs. `fail18.json` is excluded as depth of JSON is not specified.

|            | FadeJSON | Jil    | JSON.NET | ServiceStack.Text | SimpleJson | jsonfx |
| ---------- | -------- | ------ | -------- | ----------------- | ---------- | ------ |
| Passes     | 35/35    | 29/35  | 30/35    | 3/35              | 0/35       | 24/35  |
| Percentage | 100%     | 82.66% | 85.71%   | 8.57%             | 0.00%      | 68.57% |



## License

MIT License