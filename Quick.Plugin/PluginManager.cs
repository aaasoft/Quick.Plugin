using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Quick.Plugin
{
    public class PluginManager
    {
        public static PluginManager Instance = new PluginManager();

        private IPluginFileProvider pluginFileProvider;
        private List<PluginInfo> pluginInfoList = new List<PluginInfo>();

        public PluginInfo GetPluginInfo(string pluginId)
        {
            return pluginInfoList.FirstOrDefault(t => t.Id == pluginId);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(IPluginFileProvider pluginFileProvider = null)
        {
            if (pluginFileProvider == null)
                pluginFileProvider = new SameFolderPluginFileFolder();
            this.pluginFileProvider = pluginFileProvider;

            foreach (var pluginFile in pluginFileProvider.GetPluginFiles())
            {
                var assemblyName = Path.GetFileNameWithoutExtension(pluginFile);
                var assembly = Assembly.Load(new AssemblyName(assemblyName));
                var plugin = new PluginInfo();
                plugin.Id = assembly.GetName().Name;
                //读取插件名称
                plugin.Name = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? plugin.Id;
                //读取依赖的程序集
                plugin.Refrences = assembly
                    .GetReferencedAssemblies()
                    .Select(t => t.ToString())
                    .ToArray();
                pluginInfoList.Add(plugin);
            }

            //根据依赖关系排序
            while (true)
            {
                //顺序是否改变过
                Boolean isOrderChanged = false;
                for (int i = 0; i < pluginInfoList.Count; i++)
                {
                    var plugin = pluginInfoList[i];
                    if (plugin.Refrences == null
                        || plugin.Refrences.Length == 0)
                        continue;
                    //如果此插件依赖的插件在自己的后面
                    if (plugin.Refrences.Any(t => pluginInfoList.IndexOf(GetPluginInfo(t)) > i))
                    {
                        //将此插件移动到尾部
                        pluginInfoList.RemoveAt(i);
                        pluginInfoList.Insert(pluginInfoList.Count, plugin);
                        isOrderChanged = true;
                    }
                }
                //如果顺序没有改变，则跳出循环
                if (!isOrderChanged)
                    break;
            }
            //创建Activator的实例
            foreach (var pluginInfo in pluginInfoList)
            {
                pluginInfo.Activator = createActivatorInstance(pluginInfo.Id);
            }
        }

        /// <summary>
        /// 获取全部的插件
        /// </summary>
        /// <returns></returns>
        public PluginInfo[] GetAllPlugins()
        {
            return pluginInfoList.ToArray();
        }

        /// <summary>
        /// 获取插件启动器
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private IPluginActivator createActivatorInstance(string id)
        {
            Assembly assembly = Assembly.Load(new AssemblyName(id));
            Type activatorType = assembly.GetTypes()
                .FirstOrDefault(t => typeof(IPluginActivator).GetTypeInfo().IsAssignableFrom(t));
            if (activatorType == null)
                return null;
            IPluginActivator activator = (IPluginActivator)Activator.CreateInstance(activatorType);
            return activator;
        }

        /// <summary>
        /// 启动全部插件
        /// </summary>
        public void Start()
        {
            //全部插件
            var plugins = GetAllPlugins();
            Console.WriteLine($"->共{plugins.Length}个插件");
            string finalStr = "".PadLeft(plugins.Length, '-');
            Console.WriteLine(finalStr);

            foreach (var plugin in plugins)
            {
                Console.Write(">");
                if (plugin.Activator == null)
                    continue;
                plugin.Activator.Start();
            }
            Console.WriteLine();
        }

        /// <summary>
        /// 停止全部插件
        /// </summary>
        public void Stop()
        {
            //全部的插件
            var plugins = GetAllPlugins();

            foreach (var plugin in plugins)
            {
                if (plugin.Activator == null)
                    continue;
                Console.Write($"插件[{plugin}]");
                Console.Write("->停止中");
                plugin.Activator.Stop();
                Console.WriteLine("->完成");
            }
        }
    }
}
