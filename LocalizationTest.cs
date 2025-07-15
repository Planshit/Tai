using System;
using System.Globalization;
using System.Resources;
using System.Threading;

namespace LocalizationTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Tai 本地化测试程序");
            Console.WriteLine("==================");
            
            // 测试中文
            Console.WriteLine("\n测试中文 (zh-CN):");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
            TestLocalization();
            
            // 测试英文
            Console.WriteLine("\n测试英文 (en-US):");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            TestLocalization();
            
            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();
        }
        
        static void TestLocalization()
        {
            try
            {
                var resourceManager = new ResourceManager("UI.Properties.Resources", typeof(Program).Assembly);
                
                var testKeys = new[]
                {
                    "Navigation_Overview",
                    "Settings_Title",
                    "Website_Title",
                    "Message_Loading"
                };
                
                foreach (var key in testKeys)
                {
                    var value = resourceManager.GetString(key, Thread.CurrentThread.CurrentUICulture);
                    Console.WriteLine($"{key}: {value}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
            }
        }
    }
} 