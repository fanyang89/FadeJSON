#FadeJson
极简的Json Parser。代码非常容易读，第一版代码不过**190**行。适合初学者学习编译器前端。

## 用法
```C#
var jsonObject = FadeJson.JsonObject.FromString(content);
var value = jsonObject["frameworks"]["dotnet"]["dependencies"]["System.Linq"]; //value == "4.0.0"
```
(Quite easy.)

## 性能
第一版使用ANTLR，性能很差。（但是ANTLR用起来真的很爽啊）

同个json文件，Json.NET 用时小于100ms，FadeJson用时大于200ms。

FadeJson2重写前端，加强优化。（Working）