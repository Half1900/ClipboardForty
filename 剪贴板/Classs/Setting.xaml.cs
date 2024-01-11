
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using WpfClipboard;

namespace Clipboards
{
    /// <summary>
    /// Setting.xaml 的交互逻辑
    /// </summary>
    public partial class Setting : Window
    {
        Config con=new Config();
        public Setting()
        {
            InitializeComponent();            
            Topmost=true;
                       
            //Top = System.Windows.SystemParameters.PrimaryScreenHeight - this.Height - 50;
            //Left = System.Windows.SystemParameters.PrimaryScreenWidth - this.Width - 20;
            
            Initial();
        }

        private void Initial()
        {
            checkbox_auto_start.IsChecked = con.readbool("auto_starts") == 1 ? true : false; //con.read_auto_starts();//
            bool tag =(bool)checkbox_auto_start.IsChecked;
            if (tag) con.add_auto_start();
            else con.del_auto_start();
        }
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            bool auto_starts=(bool)checkbox_auto_start.IsChecked;      
            con.write("auto_starts",auto_starts);            
            if(auto_starts) con.add_auto_start();
            else con.del_auto_start();
            
        }

        private void TaskBar_Click(object sender, RoutedEventArgs e)
        {
            /*          
            bool taskbar_state = (bool)checkbox_taskBar.IsChecked;
            con.write("taskbar_state", taskbar_state);
            
            if (taskbar_state == true)
            {
                var mv = new MainWindow();
                mv.showInTaskBar(true);
                //MainWindow.ShowInTaskbar=true ;
            }
            else
            {
                var mv = new MainWindow();
                mv.showInTaskBar(false);
            }*/
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                WindowState = WindowState.Normal;
                DragMove();
            }
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
