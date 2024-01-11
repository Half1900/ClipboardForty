using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfClipboard;
using 剪贴板;
using App = WpfClipboard.App;

namespace Clipboards
{
    public class Config
    {
        [DllImport("kernel32.dll")]
        private static extern long WritePrivateProfileString(string section, string key, string value, string filepath);

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        private string config_path;
        private string exe_path;
        /*        public Config()
                {
                    config_path = System.Environment.CurrentDirectory + "\\config.ini";
                    exe_path = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                }*/
        public Config()
        {
            exe_path = Process.GetCurrentProcess().MainModule.FileName;
            //exe_path = Assembly.GetExecutingAssembly().Location;
            string exename = Process.GetCurrentProcess().MainModule.ModuleName;
            Trace.WriteLine("exe_path----" + exe_path);
            Trace.WriteLine("exename----" + exename);
            config_path = exe_path.Replace(exename, "") + "config.ini";
        }


        public int readint(string key)
        {
            string str = readstring(Convert.ToString(key));
            return int.Parse(str);
        }

        public int readbool(string key)
        {
            string str = readstring(Convert.ToString(key));
            //MessageBox.Show(str);
            //bool b = str == "True" ? true : false;

            if (str == "True")
            {
                return 1;
            }
            else
            {
                if (str == "0")
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public String readstring(string key)
        {
            StringBuilder v = new StringBuilder(1024);
            string v2;
            GetPrivateProfileString("config", key, "0", v, 1024, config_path);
            v2 = v.ToString();
            return v2;
        }

        public void write(string key, bool value)
        {
            string b = value ? "True" : "False";
            write(key, Convert.ToString(value));
        }

        public void write(string key, int value)
        {
            write(key, Convert.ToString(value));
        }

        public void write(string key, string value)
        {
            WritePrivateProfileString("config", key, value, config_path);
        }
        public bool read_auto_starts()
        {
            string applicationName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;// 获取当前正在运行的应用程序进程列表

            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            string str = (string)registryKey.GetValue("Clipboards");
            var path = Path.GetFileName(str);
            Trace.WriteLine("GetValue:" + Path.GetFileName(str));
            //bool b = str == exe_path ? true : false;
            //return b;
            //MessageBox.Show("0---" + System.Reflection.Assembly.GetExecutingAssembly());
            //MessageBox.Show("1---" + t);
            //MessageBox.Show("2---" + str);
            //MessageBox.Show("3---" + exe_path);

            return path == "ClipsForty.exe" ? true : false;

        }
        public bool read_auto_start()
        {
            /*  RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
              string str = (string)registryKey.GetValue("Clipboards");
              Trace.WriteLine("GetValue:" + str);
              bool b = str == exe_path ? true : false;
              registryKey.Close();
              return b;
  */

            try
            {
                using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    string str = (string)registryKey.GetValue("Clipboards");
                    Trace.WriteLine("GetValue:" + str);
                    //bool b = str == exe_path ? true : false;
                    registryKey.Close();
                    return str == exe_path ? true : false;
                }
            }
            catch (Exception ex)
            {
                // 在这里处理异常，例如记录日志或向用户显示错误消息  
                Console.WriteLine("Error accessing registry: " + ex.Message);
                return false; // 或其他适当的默认值  
            }
        }
        public void add_auto_start()
        {
            //AddToAutostartw();
            SetSelfStart(true);
            //SetSelfStarting(true, "ClipsForty");
            //App.SetRunOnStartup(true);

            //App.SetRunOnStartup(true);
            /*             RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                         Trace.WriteLine("=-----"+exe_path);
                        if (registryKey.GetValue("Clipboards") != null)
                        {
                            registryKey.DeleteValue("Clipboards", false);
                        }
                         registryKey.SetValue("Clipboards", exe_path);
                         registryKey.Close();*/
        }
        public void del_auto_start()
        {
            //RemoveFromAutostart();
            SetSelfStart(false);
            //SetSelfStarting(false, "ClipsForty");
            //App.SetRunOnStartup(false);
            /*                        RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                                    try
                                    {
                                        if (registryKey.GetValue("Clipboards") != null)
                                        {
                                            registryKey.DeleteValue("Clipboards", false);
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    registryKey.Close();*/
        }

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
                        Trace.WriteLine(ex.Message + "");
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
                        Trace.WriteLine(ex.Message + "");
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                if (key != null)
                {
                    key.Close();
                }
                return false;
            }
        }

        public void SetSelfStart1(bool b)
        {
            try
            {
                //设置开机自启动  
                if (b)
                {
                    /*方法一*/
                    string StartupPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonStartup);
                    //获得文件的当前路径
                    string dir = Directory.GetCurrentDirectory();
                    //MessageBox.Show(StartupPath);
                    //MessageBox.Show(dir);
                    //获取可执行文件的全部路径
                    string exeDir = dir + @"\ClipsForty.exe.lnk";
                    File.Copy(exeDir, StartupPath + @"\ClipsForty.exe.lnk", true);

                }
                //取消开机自启动  
                else
                {
                    /*方法一*/
                    string StartupPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonStartup);
                    try
                    {
                        System.IO.File.Delete(StartupPath + @"\ClipsForty.exe.lnk");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("无法删除文件: " + ex.Message);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static void CreateShortcut(string lnkFilePath, string args = "")
        {
            var shellType = Type.GetTypeFromProgID("WScript.Shell");
            dynamic shell = Activator.CreateInstance(shellType);
            var shortcut = shell.CreateShortcut(lnkFilePath);
            shortcut.TargetPath = Assembly.GetEntryAssembly().Location;
            shortcut.Arguments = args;
            shortcut.WorkingDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            shortcut.Save();
        }
        public void SetSelfStart(bool b)
        {
            try
            {
                // 检查是否需要管理员权限  
                if (b)
                {
                    // 设置开机自启动    
                    string StartupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                    string dir = Directory.GetCurrentDirectory();
                    string exeDir = dir + @"\FortyClipboard.lnk";
                    //MessageBox.Show(StartupPath);
                    //MessageBox.Show(exeDir);
                    if (!File.Exists(exeDir))
                    {
                        CreateShortcut(exeDir, "");
                    }                    
                    File.Copy(exeDir, StartupPath + @"\FortyClipboard.lnk", true); 
                }
                else
                {
                    // 取消开机自启动    
                    string RoamingStartupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\FortyClipboard.lnk";
                    string exeDir = Directory.GetCurrentDirectory() + @"\FortyClipboard.lnk";
                    try
                    {
                        File.Delete(RoamingStartupPath);                                              
                        File.Delete(exeDir);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("无法删除文件: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void setSelfAuto(bool b)
        {
            string shortcutPath = "C:\\Users\\YourUsername\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\Startup\\ClipsForty.exe.lnk";
            if (b)
            {
                // 创建快捷方式的目标路径和名称  
                string targetPath = "C:\\Program Files\\YourApplication\\ClipsForty.exe";

                // 创建快捷方式对象  
                ProcessStartInfo startInfo = new ProcessStartInfo(targetPath);
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = true;
                startInfo.Verb = "runas"; // 请求管理员权限  

                // 创建快捷方式进程  
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }

                // 将快捷方式添加到开机自启动目录  
                using (StreamWriter writer = new StreamWriter(shortcutPath))
                {
                    writer.WriteLine("[shell:startup]\r\nscenarios=multiple-desktop\r\ndesktop=multi-mon.explorer\r\nicon=C:\\Program Files\\YourApplication\\YourApplication.exe");
                    writer.WriteLine("[InternetShortcut]\r\nURL=file:///C:\\Program Files\\YourApplication\\YourApplication.exe");
                    writer.WriteLine("[DefaultIcon]\r\nfile=C:\\Program Files\\YourApplication\\YourApplication.exe,0");
                }
            }
            else
            {
                // 检查快捷方式是否存在  
                if (File.Exists(shortcutPath))
                {
                    // 删除快捷方式  
                    File.Delete(shortcutPath);
                }
            }
        }


        private void RemoveFromAutostart()
        {
            //获取自启动文件夹的路径
            string startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            //注意这里要放入你根目录下程序的快捷方式
            string destinationPath = Path.Combine(startupFolderPath, "ClipsForty.exe.lnk");
            // 删除快捷方式文件  
            File.Delete(destinationPath);
        }

        private void AddToAutostartw()
        {
            ///动态获取程序的相对路径
            string currentDirectory = System.IO.Directory.GetCurrentDirectory();
            //获取自启动文件夹的路径
            string startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            string shortcutPath = currentDirectory + "\\ClipsForty.exe.lnk";
            MessageBox.Show(shortcutPath);
            string destinationPath = Path.Combine(startupFolderPath, "ClipsForty.exe.lnk");
            MessageBox.Show(destinationPath);
            File.Copy(shortcutPath, destinationPath, true);
        }

    }
}

