#FadeJson|
极简的Json Parser。代码简洁易读，适合初学者学习编译器前端相关知识。

## 用法
```C#
var jsonObject = FadeJson2.JsonObject.FromString(content);
var value = jsonObject["frameworks"]["dotnet"]["dependencies"]["System.Linq"]; //value == "4.0.0"
```
(就是这么简单)

## 性能
FadeJson的第一个版本使用ANTLR，性能很差。通常，FadeJson解析用时大于200ms。

FadeJson2的代码全部手写，性能有很大改善。最近一次的测试数据（Visual Studio 2015, Release配置编译）：

### 最近五次测试
（单位毫秒。耗时越短越好）
|FadeJson2|Json.NET|
|---------|--------|
|46       |32      |
|45       |32      |
|43       |31      |
|49       |34      |
|45       |33      |
|42       |31      |