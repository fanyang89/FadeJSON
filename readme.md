#FadeJson
极简的高性能Json Parser。代码简洁易读。

## 用法

###FadeJson
1. 在项目中添加对`FadeJson.dll`的引用。
2. 添加Using

```
using FadeJson;
```

3. 读取Json文件的值

```
var jsonObject = FadeJson2.JsonValue.FromString(content);
var value = jsonObject["frameworks"]["dotnet"]["dependencies"]["System.Linq"]; //value == "4.0.0"
```

###FadeJson.Toolkit.ExportClass
这是一个从Json文件中导出实体类的工具
命令行：
```
FadeJson.Toolkit.ExportClass.exe example.json MyNamespace
```
生成的实体类在`FadeJson.Toolkit.ExportClass.exe`所在的目录中，添加到工程即可使用。

## 性能
FadeJson性能相比初版有很大改善。

### 最近五次测试
（Visual Studio 2015, Release配置编译。读取相同的文件，相同的值）
（单位毫秒。耗时越短越好）

|FadeJson2|Json.NET|
|----|----|
|21ms|27ms|
|21ms|26ms|
|20ms|27ms|
|20ms|26ms|
|20ms|29ms|
