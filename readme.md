#FadeJson
极简的Json Parser。代码简洁易读，适合初学者学习编译器前端相关知识。

## 用法
```C#
var jsonObject = FadeJson2.JsonValue.FromString(content);
var value = jsonObject["frameworks"]["dotnet"]["dependencies"]["System.Linq"]; //value == "4.0.0"
```
(就是这么简单)

## 性能
FadeJson的第一个版本使用ANTLR，性能很差。通常，FadeJson解析用时大于200ms。

FadeJson2的代码全部手写，性能有很大改善。

### 最近五次测试
（Visual Studio 2015, Release配置编译）
（单位毫秒。耗时越短越好）

|FadeJson2|Json.NET|
|----|----|
|21ms|27ms|
|21ms|26ms|
|20ms|27ms|
|20ms|26ms|
|20ms|29ms|