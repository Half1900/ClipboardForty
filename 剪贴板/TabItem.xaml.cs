using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfClipboard
{
    /// <summary>
    /// Item.xaml 的交互逻辑
    /// </summary>
    public partial class TabItem : UserControl
    {
        ClipboardType clipboardType;
        public Action<ClipboardType> onSelect;
        public TabItem(ClipboardType clipboardType,string text)
        {
            this.clipboardType = clipboardType;
            InitializeComponent();
            tbText.Text = text;
            SetSelect(false);
        }
        public void SetSelect(bool bl)
        {
            if (bl)
            {
                border.Background = new SolidColorBrush(Color.FromRgb(0x25,0xAE,0xF3));
                tbText.Foreground = Brushes.White;
            }
            else
            {
                border.Background = Brushes.White;
                tbText.Foreground = Brushes.Black;
            }
        }

        private void border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            onSelect?.Invoke(clipboardType);
            SetSelect(true);
        }
    }
}
