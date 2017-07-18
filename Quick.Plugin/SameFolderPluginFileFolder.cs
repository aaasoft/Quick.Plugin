using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Quick.Plugin
{
    public class SameFolderPluginFileFolder : IPluginFileProvider
    {
        private DirectoryInfo dir;
        public SameFolderPluginFileFolder()
        {
            dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            if (!dir.Exists)
                throw new IOException($"Cann't found plugin folder[{dir.FullName}].");
        }

        public string GetPluginFile(string pluginId)
        {
            var fileInfo = new FileInfo(Path.Combine(dir.FullName, $"{pluginId}.dll"));
            if (fileInfo.Exists)
                return fileInfo.FullName;
            return null;
        }

        public string[] GetPluginFiles()
        {
            return dir.GetFiles("Plugin.*.dll")
                .Select(t => t.FullName)
                .ToArray();
        }
    }
}
