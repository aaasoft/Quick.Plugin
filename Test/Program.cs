using Quick.Plugin;
using System;
using System.IO;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(@"D:\Work\Wesan\Launcher\bin\Debug\netcoreapp2.0");
            PluginManager.Instance.Init();
            PluginManager.Instance.Start();
            Console.ReadLine();
        }
    }
}
