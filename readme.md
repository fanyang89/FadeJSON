# FadeJson

代码简洁易读的Json Parser。 支持.NET Framework 3.5及以上版本。

The code is simple and easy to read. Support .NET Framework 3.5 and above.

[FadeJson in GitHub](https://github.com/YangFan789/FadeJson)

[FadeJson in GitOSC](http://git.oschina.net/fuis/FadeJson)

[下载二进制版本（Download Binary）](https://github.com/YangFan789/FadeJson/releases)

<script src='http://git.oschina.net/fuis/FadeJson/star_widget_preview'></script>

<script src='http://git.oschina.net/fuis/FadeJson/fork_widget_preview'></script>

## 用法（Usage）

### FadeJson Library

1. 在项目中添加对`FadeJson.dll`的引用。
   
   （Add the reference of `FadeJson.dll` to your project.） 
   
2. 添加`using FadeJson;`
   
   （Import namespace `using FadeJson;`）
   
3. 读取JSON文件的值
   
   （Reading values from JSON file as shown in the following code.）

``` 
var jsonObject = FadeJson.JsonValue.FromString(content);
var value = jsonObject["frameworks"]["dotnet"]["dependencies"]["System.Linq"]; //value == "4.0.0"
var realValue = value.ValueOf(); //cast value to its real type
```

### FadeJson.ExportClass

这是一个从Json文件中导出实体类的工具

（It a tool which helps you import a real class from your JSON file.）

命令行：

（CommandLines:）

``` 
FadeJson.ExportClass example.json MyNamespace
```

生成的实体类在`FadeJson.ExportClass.exe`所在的目录中，添加到工程即可使用。

（The generated entity classes in the same directory as the `FadeJson.ExportClass.exe`, can be added to a project. ）

## 性能（performance）

FadeJson是一个高性能的Json Parser（至少比Json.NET要更快）。

（FadeJson is a high performance Json Parser (or at least faster than theJson.NET).）

### 测试（Test）

.NET Framework 4.6, Release配置编译。读取相同的JSON文件。

每轮测试迭代10次，取平均值。单位毫秒。耗时越短越好。

.NET Framework 4.6 Release configuration to compile, Reading the same JSON files.

Each test iteration 10 times and averaged, Units of milliseconds. Take the shorter the better.



#### 大文件（Big JSON file）（3660KiB）

| FadeJson | Json.NET |
| -------- | -------- |
| 1106ms   | 1318ms   |
| 1123ms   | 1373ms   |
| 1185ms   | 1331ms   |

#### 小文件（Small JSON file）（2KiB）

| FadeJson | Json.NET |
| -------- | -------- |
| 12ms     | 44ms     |
| 13ms     | 44ms     |
| 12ms     | 44ms     |