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
    public partial class Item : UserControl
    {
        public Action<ClipboardItem> onCope;
        public Action<Item, ClipboardItem> onDelete;
        public Action<Item, bool> onTop;
        ClipboardItem clipboardItem;
        bool flag = false;
        public void setXuHao(int i)
        {
            lbxuhao.Content = i + "";
        }
        public Item(ClipboardItem clipboardItem)
        {
            this.clipboardItem = clipboardItem;
            InitializeComponent();
            tbText.Visibility = Visibility.Collapsed;
            img.Visibility = Visibility.Collapsed;
            switch (clipboardItem.Type)
            {
                case ClipboardType.Text:
                    tbText.Text = clipboardItem.Text;
                    tbText.Visibility = Visibility.Visible;
                    path.Data = Geometry.Parse("M692.2597343921661 882.78857421875H331.711297750473v-30.689996480941772a563.3140236139297 563.3140236139297 0 0 1 48.088091611862176-14.83154296875c16.200274229049683-4.278552532196046 33.656305074691765-7.757592201232912 52.39561200141907-10.638445615768433v-630.340576171875h-136.73552870750427l-57.244107127189636 151.53810381889343H211.2339789867401c-2.1675199270248413-14.118209481239319-3.9642512798309326-30.233752727508545-5.419161915779114-48.23148250579833-1.4541864395141602-17.939794063568115-2.6809751987457275-36.30831241607666-3.7933409214019784-55.10410666465761-1.1123657226562498-18.59736442565918-1.9683659076690674-37.078857421875-2.6527315378189087-55.019375681877136C198.6264432668686 171.53063368797297 198.2563788890838 155.35860311985022 198.2563788890838 141.21142578125h627.4886906147003c0 14.147177338600159-0.3997564315795899 30.090361833572388-1.1123657226562498 47.689059376716614-0.7415771484374999 17.655909061431885-1.6547888517379765 35.852792859077454-2.766430377960205 54.56313192844391s-2.139276266098023 37.078857421875-3.251641988754273 55.10410666465761c-1.0841220617294314 17.998453974723816-2.7099430561065674 34.48406159877777-4.9064308404922485 49.28736090660096h-28.408053517341607l-56.75889551639557-151.53810381889343H592.8891206979752V826.6850764751434c18.71033906936645 3.5659432411193848 36.22285723686218 7.07395076751709 52.366644144058235 10.638445615768433 16.229242086410522 3.508731722831726 31.85957372188568 8.415162563323973 47.00469374656678 14.83154296875v30.633509159088135z");
                    break;
                case ClipboardType.HtmlText:
                    tbText.Text = clipboardItem.Text;
                    tbText.Visibility = Visibility.Visible;
                    path.Data = Geometry.Parse("M914.285714 0a109.714286 109.714286 0 0 1 44.032 210.212571v603.574858a109.714286 109.714286 0 1 1-144.530285 144.530285H210.212571A109.714286 109.714286 0 1 1 65.828571 813.714286v-603.428572A109.714286 109.714286 0 1 1 210.285714 65.828571h603.428572A109.714286 109.714286 0 0 1 914.285714 0z m0 877.714286a36.571429 36.571429 0 1 0 0 73.142857 36.571429 36.571429 0 0 0 0-73.142857z m-804.571428 0a36.571429 36.571429 0 1 0 0 73.142857 36.571429 36.571429 0 0 0 0-73.142857z m704.073143-723.894857H210.212571c-11.044571 25.161143-31.232 45.348571-56.393142 56.393142v603.574858c25.234286 11.117714 45.494857 31.378286 56.466285 56.612571h603.428572c10.971429-25.307429 31.378286-45.641143 56.685714-56.685714v-603.428572a110.153143 110.153143 0 0 1-56.612571-56.466285zM555.885714 753.371429a14.628571 14.628571 0 0 1-14.628571 14.628571h-58.514286a14.628571 14.628571 0 0 1-14.628571-14.628571l-0.073143-373.101715L343.771429 380.342857a14.628571 14.628571 0 0 1-14.628572-14.628571v-58.514286c0-8.045714 6.582857-14.628571 14.628572-14.628571h336.457142c8.045714 0 14.628571 6.582857 14.628572 14.628571V365.714286a14.628571 14.628571 0 0 1-14.628572 14.628571l-124.342857-0.073143V753.371429zM914.285714 73.142857a36.571429 36.571429 0 1 0 0 73.142857 36.571429 36.571429 0 0 0 0-73.142857z m-804.571428 0a36.571429 36.571429 0 1 0 0 73.142857 36.571429 36.571429 0 0 0 0-73.142857z");
                    break;
                case ClipboardType.File:
                    tbText.Text = clipboardItem.Text;
                    tbText.Visibility = Visibility.Visible;
                    path.Data = Geometry.Parse("M989.541 940.667H130.618s-68.409 10.642-68.409-80.572V122.789s1.521-82.091 85.132-82.091h301.003s36.486-7.601 66.89 39.525c28.884 45.607 45.607 74.491 45.607 74.491s10.642 12.161 34.965 12.161h387.655s68.409-7.601 68.409 68.409v629.371s10.642 76.012-62.33 76.012z m-63.849-577.683c0-18.243-15.202-33.445-33.445-33.445H223.351c-19.763 0-34.965 15.203-34.965 33.445v3.04c0 19.763 15.202 34.965 34.965 34.965h668.896c18.243 0 33.445-15.203 33.445-34.965v-3.04z");
                    break;
                case ClipboardType.Image:
                    img.Source = (BitmapSource)clipboardItem.Data;
                    maxImg.Source = (BitmapSource)clipboardItem.Data;
                    path.Data = Geometry.Parse("M0 921.6V102.4a102.4 102.4 0 0 1 102.4-102.4h1024a102.4 102.4 0 0 1 102.4 102.4v819.2a102.4 102.4 0 0 1-102.4 102.4H102.4a102.4 102.4 0 0 1-102.4-102.4z m102.4-515.3792q591.872-31.1296 1024 261.3248V102.4H102.4v303.8208zM921.6 409.6a102.4 102.4 0 1 0 0-204.8 102.4 102.4 0 0 0 0 204.8zM102.4 921.6h1024V793.2928Q703.2832 475.648 102.4 508.7232V921.6z");
                    img.Visibility = Visibility.Visible;
                    break;
                case ClipboardType.Audio:
                    img.Source = (BitmapSource)clipboardItem.Data;
                    maxImg.Source = (BitmapSource)clipboardItem.Data;
                    path.Data = Geometry.Parse("M0 921.6V102.4a102.4 102.4 0 0 1 102.4-102.4h1024a102.4 102.4 0 0 1 102.4 102.4v819.2a102.4 102.4 0 0 1-102.4 102.4H102.4a102.4 102.4 0 0 1-102.4-102.4z m102.4-515.3792q591.872-31.1296 1024 261.3248V102.4H102.4v303.8208zM921.6 409.6a102.4 102.4 0 1 0 0-204.8 102.4 102.4 0 0 0 0 204.8zM102.4 921.6h1024V793.2928Q703.2832 475.648 102.4 508.7232V921.6z");
                    img.Visibility = Visibility.Visible;
                    break;
            }
            if (clipboardItem.IsTop)
            {
                topBtn.Tag = "true";
            }
        }

        private void DockPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Released)
            {
                onCope?.Invoke(clipboardItem);
            }           
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            onDelete?.Invoke(this, clipboardItem);
        }

        private void topBtn_Click(object sender, RoutedEventArgs e)
        {
            if (topBtn.Tag != null && topBtn.Tag.ToString() == "true")
            {
                topBtn.Tag = "";
                clipboardItem.IsTop = false;
                onTop?.Invoke(this, false);
            }
            else
            {
                topBtn.Tag = "true";
                clipboardItem.IsTop = true;
                onTop?.Invoke(this, true);
            }

        }
        private void MouseRDown(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                flag = true;
                if (clipboardItem.Type == ClipboardType.Image && flag)
                {
                    popup.IsOpen = true;
                }
            }
        }
        private void MouseRUp(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Released)
            {
                flag = false;
                if (clipboardItem.Type == ClipboardType.Image && !flag)
                {
                    popup.IsOpen = false;
                }
            }
        }
        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            if (clipboardItem.Type == ClipboardType.Image && flag)
            {
                //popup.IsOpen = true;}

            }
        }
        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            if (clipboardItem.Type == ClipboardType.Image && flag)
            {
                popup.IsOpen = false;
            }
        }
    }
}
