#FadeJson
极简的Json Parser。适合初学者学习编译器前端。

## 用法
```C#
var jsonObject = FadeJson2.JsonObject.FromString(content);
var value = jsonObject["frameworks"]["dotnet"]["dependencies"]["System.Linq"]; //value == "4.0.0"
```
(Quite simple.)

## 性能
FadeJson使用ANTLR，性能很差。同个json文件，Json.NET 用时小于100ms，FadeJson用时大于200ms。
FadeJson2的代码全部手写(递归下降)，没有使用Parser Generater。
FadeJson2的性能有很大改善。最近一次的测试数据：
||FadeJson2||Json.NET||
||65ms||37ms||
||86ms||61ms||
||101ms||90ms||