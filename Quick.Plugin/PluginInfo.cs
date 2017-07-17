using System;

namespace Quick.Plugin
{
    public class PluginInfo
    {
        /// <summary>
        /// 插件的编号
        /// </summary>
        public String Id { get; set; }
        /// <summary>

        /// <summary>
        /// 插件的名称
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 依赖的插件
        /// </summary>
        public String[] Refrences { get; set; }

        public IPluginActivator Activator { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
