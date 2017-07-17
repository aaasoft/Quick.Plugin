using System;
using System.Collections.Generic;
using System.Text;

namespace Quick.Plugin
{
    public interface IPluginFileProvider
    {
        /// <summary>
        /// 根据插件名获取插件文件
        /// </summary>
        /// <param name="pluginId"></param>
        /// <returns></returns>
        string GetPluginFile(string pluginId);
        /// <summary>
        /// 获取全部的插件文件
        /// </summary>
        /// <returns></returns>
        string[] GetPluginFiles();
    }
}
