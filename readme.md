#FadeJson
代码简洁易读的Json Parser。

[FadeJson in GitHub](https://github.com/YangFan789/FadeJson)
[FadeJson in GitOSC](http://git.oschina.net/fuis/FadeJson)

<script src='http://git.oschina.net/fuis/FadeJson/star_widget_preview'></script>
<script src='http://git.oschina.net/fuis/FadeJson/fork_widget_preview'></script>

## 用法

###FadeJson
1. 在项目中添加对`FadeJson.dll`的引用。
2. 添加Using

```
using FadeJson;
```

3. 读取Json文件的值

```
var jsonObject = FadeJson.JsonValue.FromString(content);
var value = jsonObject["frameworks"]["dotnet"]["dependencies"]["System.Linq"]; //value == "4.0.0"
```

###FadeJson.ExportClass
这是一个从Json文件中导出实体类的工具
命令行：
```
FadeJson.ExportClass example.json MyNamespace
```
生成的实体类在`FadeJson.ExportClass.exe`所在的目录中，添加到工程即可使用。

## 性能
FadeJson在读取较小的Json文件时比Json.NET快速。

### 测试
.NET Framework 4.6, Release配置编译。读取相同的Json文件。
每轮测试迭代10次，取平均值。单位毫秒。耗时越短越好。

#### 大文件（3660KiB）

|FadeJson3|Json.NET|
|---------|--------|
|1714ms|1312ms|
|1722ms|1315ms|
|1750ms|1314ms|

#### 小文件（2KiB）

|FadeJson3|Json.NET|
|---------|--------|
|13ms|45ms|
|13ms|46ms|
|13ms|46ms|