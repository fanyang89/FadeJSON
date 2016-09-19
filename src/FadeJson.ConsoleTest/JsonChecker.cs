using System;

namespace FadeJSON.ConsoleTest
{
    internal static class JsonChecker
    {
        public static void Check(string title, Action<string> testAction) {
            Console.WriteLine($"============ {title} ============");
            var failedTestCount = 3;
            for (var i = 1; i <= 3; i++) {
                try {
                    testAction($"JSONChecker/pass{i}.json");
                } catch (Exception) {
                    failedTestCount--;
                    Console.WriteLine($"pass{i}.json parsing failed.");
                }
            }
            for (var i = 1; i <= 33; i++) {
                var isFailed = false;
                try {
                    testAction($"JSONChecker/fail{i}.json");
                } catch (Exception) {
                    isFailed = true;
                    failedTestCount++;
                }
                if (!isFailed) {
                    Console.WriteLine($"fail{i}.json parsing failed.");
                }
            }
            Console.WriteLine($"({failedTestCount}/36) {(failedTestCount / 36.0 * 100).ToString("F")}%");
            Console.WriteLine();
        }
    }
}