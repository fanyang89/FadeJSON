# FadeJson

代码简洁易读的Json Parser。 支持.NET Framework 3.5及以上版本。

The code is simple and easy to read. Support .NET Framework 3.5 and above.

[FadeJson in GitHub](https://github.com/YangFan789/FadeJson)

[FadeJson in GitOSC](http://git.oschina.net/fuis/FadeJson)

[下载二进制版本（Download Binary）](https://github.com/YangFan789/FadeJson/releases)

## 用法 Getting Started

### FadeJson Main Library

1. 在项目中添加对`FadeJson.dll`的引用。

2. 添加`using FadeJson;`

3. 读取JSON文件的值


1. Add the reference of `FadeJson.dll` to your project.

2. Import namespace `using FadeJson;`

3. Reading values from JSON file as shown in the following code.

``` 
var jsonObject = FadeJson.JsonValue.FromString(content);
var value = jsonObject["frameworks"]["dotnet"]["dependencies"]["System.Linq"];
var realValue = value.ValueOf(); //cast value to its real type
```

### FadeJson.ExportClass

这是一个从Json文件中导出实体类的工具

It a tool which helps you to generate class files(`*.cs`) for your JSON data.

#### 用法 Usage

``` 
FadeJson.ExportClass example.json MyNamespace
```

生成的实体类在`FadeJson.ExportClass.exe`所在的目录中，添加到工程即可使用。

The generated entity classes in the same directory as the `FadeJson.ExportClass.exe`, can be added to a project. 

## 解析性能 Parsing Performance

### 测试 Tests

.NET Framework 4.6.1, Release配置编译。读取相同的JSON文件。

每轮测试迭代10次，取平均值。单位毫秒。耗时越短越好。

.NET Framework 4.6 Release configuration to compile, Reading the same JSON files.

Each test iteration 10 times and averaged, Units of milliseconds. Take the shorter the better.

#### 大文件 Big JSON file（3660KiB）

| FadeJson | Json.NET |
| -------- | -------- |
| 1257ms   | 1227ms   |
| 1205ms   | 1220ms   |
| 1209ms   | 1219ms   |

#### 小文件 Small JSON file（2KiB）

| FadeJson | Json.NET |
| -------- | -------- |
| 8ms      | 41ms     |
| 7ms      | 39ms     |
| 8ms      | 43ms     |