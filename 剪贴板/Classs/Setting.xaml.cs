
using Microsoft.Win32;
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
            initial();
        }

        private void initial()
        {
            //checkbox_auto_start.IsChecked = con.readbool("auto_start");
            checkbox_auto_start.IsChecked = con.read_auto_start();
            //checkbox_taskBar.IsChecked = con.readbool("taskbar_state");
        }
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            bool auto_start=(bool)checkbox_auto_start.IsChecked;      
            con.write("auto_start",auto_start);            
            if (auto_start == true) con.add_auto_start();
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
