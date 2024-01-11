using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace WpfClipboard
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        System.Threading.Mutex mutex;
       /* public App()
        {
            this.Startup += new StartupEventHandler(App_Startup);
            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(Application_DispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }*/
        private void App_Startup(object sender, StartupEventArgs e)
        {
            bool ret;
            mutex = new System.Threading.Mutex(true, "ClipsForty", out ret);

            if (!ret)
            {
                MessageBox.Show("已有一个客户端正在运行,请先结束原来客户端!");
                Environment.Exit(0);
            }
           // #region 设置程序开机自动运行(+注册表项)
            try
            {
                SetSelfStarting(true, "ClipsForty.exe");
            }
            catch (Exception ex)
            {
                WriteLog(ex);
            }

           //#endregion

        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception)
                WriteLog(e.ExceptionObject);
            else
                WriteLog(e);
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is Exception)
                WriteLog(e.Exception);
            else
                WriteLog(e);
        }

       // #region 注册表开机自启动


        /// <summary>
        /// 开机自动启动
        /// </summary>
        /// <param name="started">设置开机启动，或取消开机启动</param>
        /// <param name="exeName">注册表中的名称</param>
        /// <returns>开启或停用是否成功</returns>
        public bool SetSelfStarting(bool started, string exeName)
        {
            RegistryKey key = null;
            try
            {

                string exeDir = System.Windows.Forms.Application.ExecutablePath;
                //RegistryKey HKLM = Registry.CurrentUser;
                //key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//打开注册表子项
                key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//打开注册表子项

                if (key == null)//如果该项不存在的话，则创建该子项
                {
                    key = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                }
                if (started)
                {
                    try
                    {
                        object ob = key.GetValue(exeName, -1);

                        if (!ob.ToString().Equals(exeDir))
                        {
                            if (!ob.ToString().Equals("-1"))
                            {
                                key.DeleteValue(exeName);//取消开机启动
                            }
                            key.SetValue(exeName, exeDir);//设置为开机启动
                        }
                        key.Close();

                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex);
                        return false;
                    }
                }
                else
                {
                    try
                    {
                        key.DeleteValue(exeName);//取消开机启动
                        key.Close();
                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex);
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                if (key != null)
                {
                    key.Close();
                }
                return false;
            }
        }

        //#endregion

        public void WriteLog(object exception)
        {
            Exception ex = exception as Exception;

            using (FileStream fs = File.Open(".//ErrorLog.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.Seek(0, SeekOrigin.End);
                byte[] buffer = Encoding.Default.GetBytes("-------------------------------------------------------\r\n");
                fs.Write(buffer, 0, buffer.Length);

                buffer = Encoding.Default.GetBytes(DateTime.Now.ToString() + "\r\n");
                fs.Write(buffer, 0, buffer.Length);

                if (ex != null)
                {
                    buffer = Encoding.Default.GetBytes("成员名: " + ex.TargetSite + "\r\n");
                    fs.Write(buffer, 0, buffer.Length);

                    buffer = Encoding.Default.GetBytes("引发异常的类: " + ex.TargetSite.DeclaringType + "\r\n");
                    fs.Write(buffer, 0, buffer.Length);

                    buffer = Encoding.Default.GetBytes("异常信息: " + ex.Message + "\r\n");
                    fs.Write(buffer, 0, buffer.Length);

                    buffer = Encoding.Default.GetBytes("引发异常的程序集或对象: " + ex.Source + "\r\n");
                    fs.Write(buffer, 0, buffer.Length);

                    buffer = Encoding.Default.GetBytes("栈：" + ex.StackTrace + "\r\n");
                    fs.Write(buffer, 0, buffer.Length);
                }
                else
                {
                    buffer = Encoding.Default.GetBytes("应用程序错误: " + exception.ToString() + "\r\n");
                    fs.Write(buffer, 0, buffer.Length);
                }
            }

        }


        public static void SetRunOnStartups(bool runOnStartup)
        {

          
            var exePath = ResourceAssembly.Location;
            var exe_path = Process.GetCurrentProcess().MainModule.FileName;//全路径
            string exeName = Process.GetCurrentProcess().MainModule.ModuleName;//应用名称
            Trace.WriteLine("exe_path----" + exe_path);
            Trace.WriteLine("exename----" + exeName);
            Trace.WriteLine("1---"+ResourceAssembly.Location);
            var keyName = GetSha256Hash(exePath);
            var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);          

            if (runOnStartup)
            {
                if (key.GetValue(keyName) != null)
                {
                    key?.DeleteValue(keyName, false);
                }
                key?.SetValue(keyName, exePath);
            }            
            else {
                if (key.GetValue(keyName) != null)
                {
                    key?.DeleteValue(keyName, false);
                }
                //key?.DeleteValue(keyName, false);
                Trace.WriteLine("2---" + key?.GetValue(keyName));
            }
               
        }
        public static void SetRunOnStartup(bool runOnStartup)
        {
            
            var exePath = ResourceAssembly.Location;
            Trace.WriteLine("1---" + ResourceAssembly.Location);
            var keyName = GetSha256Hash(exePath);

            using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                if (key == null)
                {
                    // 处理或记录错误  
                    Trace.WriteLine("3-------");
                    return;
                }
                try
                {
                    if (runOnStartup)
                    {
                        if (key?.GetValue(keyName) != null)
                        {
                            key?.DeleteValue(keyName, false);
                        }
                        key?.SetValue(keyName, exePath);
                    }
                    else
                    {
                        if (key?.GetValue(keyName) != null)
                        {
                            key?.DeleteValue(keyName, false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 处理或记录异常  
                    Trace.WriteLine("Error: " + ex.Message);
                }
                Trace.WriteLine("2---" + key?.GetValue(keyName));
            }          
        }
        internal static string GetSha256Hash(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var sha = new System.Security.Cryptography.SHA256Managed();
            var textData = System.Text.Encoding.UTF8.GetBytes(text);
            var hash = sha.ComputeHash(textData);
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }
        internal static string GetExePath()
        {            
            return ResourceAssembly.Location; 
        }
    }
}
