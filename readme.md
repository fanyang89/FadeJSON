# FadeJSON

Micro and fast JSON library for .NET Platform.



## Features

- Micro. Binary library only 13KiB.
- Easy to use.
- Fast.



## Releases

//TODO



## Getting Started

//TODO



## Benchmarks

Tester and test suites can be found in `FadeJson.ConsoleTests`

### Performance

| Time Unit: ms              | FadeJSON | Jil  | JSON.NET | SimpleJson | jsonfx |
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