using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Permissions;
using System.Text;
using Microsoft.Win32;


namespace WPFClipboard.Class {
    //注册表辅助类 Author:BingBing
    public static class Regedit
    {
        //自定义节点名称
        public static string NodeName = "XX";
        //SOFTWARE节点
        public static string SoftWare = "SOFTWARE";

        /// <summary>
        /// 读取指定名称的注册表的值
        /// </summary>
        /// <param name="key">注册表值的key</param>
        /// <returns></returns>
        public static string GetValue(string key)
        {

            using (var aimdir = Load(SoftWare))
            {
                using (var child = aimdir.OpenSubKey(NodeName))
                {
                    if (child == null) return null;
                    var registData = child.GetValue(key);
                    return registData == null ? null : registData.ToString();
                }
            }
        }

        /// <summary>
        /// 注册表中写数据 
        /// </summary>
        /// <param name="key">注册表值的key</param>
        /// <param name="tovalue">值</param>
        public static void AddValue(string key, string value)
        {
            using (var aimdir = Load(SoftWare))
            {
                using (var child = aimdir.OpenSubKey(NodeName, true))
                {
                    if (child == null) return;
                    if (ExistsValue(key, NodeName)) return;
                    child.SetValue(key, value);
                }
            }
        }

        /// <summary>
        /// 删除注册表中指定的注册表项
        /// </summary>
        /// <param name="key">注册表值中的键</param>
        public static void DeleteValue(string key)
        {
            using (var aimdir = Load(SoftWare))
            {
                if (!ExistsValue(key, NodeName)) return;
                using (var child = aimdir.OpenSubKey(NodeName, true))
                {
                    if (child != null)
                    {
                        child.DeleteValue(key);

                    }
                }
            }
        }

        /// <summary>
        /// Load
        /// </summary>
        /// <returns></returns>
        public static RegistryKey Load(string rootName)
        {
            var software = Registry.CurrentUser;
            return software.OpenSubKey(rootName, RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.FullControl);

        }

        /// <summary>
        /// 判断指定注册表项是否存在
        /// </summary>
        /// <param name="nodeName">注册表左侧节点名称</param>
        /// <returns></returns>
        public static bool Exists(string nodeName)
        {
            var exit = false;
            using (var aimdir = Load(SoftWare))
            {
                var subkeyNames = aimdir.GetSubKeyNames();
                if (subkeyNames.Any(keyName => keyName == nodeName))
                {
                    exit = true;
                }

            }

            return exit;
        }

        /// <summary>
        /// 判断指定注册表项是否存在
        /// </summary>
        /// <param name="nodeName">注册表左侧节点</param>
        /// <param name="rootName">父节点名称</param>
        /// <returns></returns>
        public static bool ExistsValue(string key, string rootName)
        {
            var exit = false;
            using (var aimdir = Load(SoftWare))
            {
                using (var child = aimdir.OpenSubKey(NodeName, true))
                {
                    if (child == null) return exit;
                    var subkeyNames = child.GetValueNames();
                    if (subkeyNames.Any(keyName => keyName == key))
                    {
                        exit = true;
                    }
                }
            }

            return exit;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="name">左侧节点名称</param>
        public static void Create(string nodeName)
        {
            using (var software = Load(SoftWare))
            {
                if (!Exists(nodeName))
                {
                    software.CreateSubKey(nodeName);
                }
            }
        }

        /// <summary>
        /// 跟随windows一起启动
        /// </summary>
        /// <param name="path">要启动程序的绝对路径</param>
        public static void AutoRun(string path)
        {
            using (var autorun = Load(SoftWare + @"\Microsoft\Windows\CurrentVersion\Run"))
            {
                autorun.SetValue("ClipsForty.exe", path);
            }
        }

        /// <summary>
        /// 跟随windows一起启动
        /// </summary>
        public static void RemoveAutoRun()
        {
            using (var autorun = Load(SoftWare + @"\Microsoft\Windows\CurrentVersion\Run"))
            {
                if (autorun.GetValue("ClipsForty.exe") == null) return;
                autorun.DeleteValue("ClipsForty.exe");
            }
        }
    }
}
