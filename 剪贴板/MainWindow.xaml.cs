using Clipboards;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;

namespace WpfClipboard
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ClipForty : Window
    {
        List<ClipboardItem> clipboardItems = new List<ClipboardItem>();
        int ICopy=0;
        private const int WM_DRAWCLIPBOARD = 0x308;
        private const int WM_CHANGECBCHAIN = 0x30D;
        private IntPtr mNextClipBoardViewerHWnd;
        ClipboardType tabSelect = ClipboardType.All;
        bool flagAltV = false;
        bool topTag = true;
        bool barTag = true;
        int clickChangeTag = 0;

        Config config = new Config();
        
        System.Windows.Forms.NotifyIcon notifyIcon;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern bool ChangeClipboardChain(IntPtr HWnd, IntPtr HWndNext);
        
        [DllImport("user32.dll")]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        private IntPtr m_Hwnd = new IntPtr();
        public ClipForty()
        {
            /*           Process[] processes = Process.GetProcesses();     //获得本机所有应用进程
                       int currentCount = 0;                              //记录程序打开次数
                       foreach (Process item in processes)                //循环本机所有应用进程名字
                       {
                           if (item.ProcessName == Process.GetCurrentProcess().ProcessName) //判断进程名字和本程序进程名字是否一致
                           {
                               currentCount += 1;
                           }
                       }
                       if (currentCount > 1)     //本程序进程大于2就退出
                       {
                           MessageBox.Show("打开重复，应用程序已存在", "提示",MessageBoxButton.OK,MessageBoxImage.Information);
                           Environment.Exit(1);
                           return;
                       }

                       foreach (Process item in processes)
                       {
                           if (item.ProcessName == Process.GetCurrentProcess().ProcessName)
                           {
                               currentCount += 1;

                               // 如果旧进程存在，尝试关闭它  
                               if (item.Id != Process.GetCurrentProcess().Id)
                               {
                                   item.Kill();
                               }
                           }
                       }

                       if (currentCount > 1)
                       {
                           MessageBox.Show("打开重复，应用程序已存在", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                           Environment.Exit(1);
                           return;
                       }*/
            
              
            string applicationName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;// 获取当前正在运行的应用程序进程列表
            //MessageBox.Show(applicationName);                                                                                                      
            Process[] processes = Process.GetProcesses(); // 获取当前正在运行的应用程序进程列表             
            foreach (Process process in processes) // 遍历进程列表，查找与当前应用程序名称相同的进程  
            {
                if (process.ProcessName == applicationName && process.Id != Process.GetCurrentProcess().Id)
                {
                    process.CloseMainWindow();    // 关闭旧进程
                    //MessageBox.Show("旧的应用程序已关闭", "提示", MessageBoxButton.OK, MessageBoxImage.Information); // 显示关闭成功消息框
                }
            }
            InitializeComponent();
            
            Top = System.Windows.SystemParameters.PrimaryScreenHeight - this.Height - 50;
            Left = System.Windows.SystemParameters.PrimaryScreenWidth - this.Width - 20;
            flagAltV = !flagAltV;
            Icons(true);
            if (int.Parse(config.readstring("clickChangeTag")) >= GetIconCount(System.IO.Path.GetFullPath(@"image")))
            {
                clickChangeTag = 0;
                config.write("clickChangeTag", clickChangeTag);
            }           
            SetTab();
            DragMove();
            if (config.readbool("barTag")== -1) {
                config.write("barTag", barTag);
            }
            if (config.readbool("topTag") == -1)
            {
                config.write("topTag", topTag);
            }
            
            itemList.Children.Clear();
            Topmost = config.readbool("topTag") == 1 ? true : false ;
            this.showInTaskBar(config.readbool("barTag") == 1 ? true : false);
            if (config.readbool("topTag") == 1)
            {
                top.Fill = new SolidColorBrush(Color.FromRgb(0xE0, 0x0B, 0xC4));
            }
            else
            {
                top.Fill = new SolidColorBrush(Color.FromRgb(0x5C, 0x5C, 0x66));
            }
            if (config.readbool("barTag") == 1)
            {
                statusBar.Fill = new SolidColorBrush(Color.FromRgb(0xE0, 0x0B, 0xC4));
            }
            else
            {
                statusBar.Fill = new SolidColorBrush(Color.FromRgb(0x5C, 0x5C, 0x66));
            }
        }
        private void top_Click(object sender, RoutedEventArgs e)
        {
           
        }
        private void Status_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void Setting_Down(object sender, RoutedEventArgs e)
        {
                     
        }

        private void BackTop_Up(object sender, RoutedEventArgs e)
        {
            scrollViewer.ScrollToVerticalOffset(0);
        }
        private void Enter_BackTop(object sender, RoutedEventArgs e)
        {
            backTop.Fill = new SolidColorBrush(Color.FromRgb(0x25, 0xAE, 0xF3));
        }
        private void Leave_BackTop(object sender, RoutedEventArgs e)
        {
            backTop.Fill = new SolidColorBrush(Color.FromRgb(0x5C, 0x5C, 0x66));

        }

        private void BackBottom_Up(object sender, RoutedEventArgs e)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.ScrollableHeight);
        }
        private void Enter_BackBottom(object sender, RoutedEventArgs e)
        {
            backBottom.Fill = new SolidColorBrush(Color.FromRgb(0x25, 0xAE, 0xF3));
        }
        private void Leave_BackBottom(object sender, RoutedEventArgs e)
        {
            backBottom.Fill = new SolidColorBrush(Color.FromRgb(0x5C, 0x5C, 0x66));

        }

        private void Setting_Up(object sender, RoutedEventArgs e)
        {
            //setting.Fill = Brushes.Red;
            setting.Fill = new SolidColorBrush(Color.FromRgb(0x5C, 0x5C, 0x66));
            Setting setWin = new Setting();
            setWin.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            setWin.Owner = this;
            setWin.ShowDialog();
        }

        private void Top_Up(object sender, RoutedEventArgs e)
        {
            topTag = !(config.readbool("topTag")==1?true:false);
            config.write("topTag",topTag);
            Topmost = topTag;
            if (topTag)//已经置顶
            {
                top.Fill = new SolidColorBrush(Color.FromRgb(0xE0, 0x0B, 0xC4));
            }
            else
            {
                top.Fill = new SolidColorBrush(Color.FromRgb(0x5C, 0x5C, 0x66));
            }               
        }
        private void Bar_Up(object sender, RoutedEventArgs e)
        {
            barTag = !(config.readbool("barTag") == 1 ? true : false); ;
            config.write("barTag",barTag);
            this.showInTaskBar(barTag);
            if (barTag)
            {
                statusBar.Fill = new SolidColorBrush(Color.FromRgb(0xE0, 0x0B, 0xC4));
            }
            else
            {
                statusBar.Fill = new SolidColorBrush(Color.FromRgb(0x5C, 0x5C, 0x66));
            }
        }
        private void Enter_Show(object sender, RoutedEventArgs e)
        {

            setting.Fill = new SolidColorBrush(Color.FromRgb(0x25, 0xAE, 0xF3));            
        }
        private void Enter_Leave(object sender, RoutedEventArgs e)
        {
            setting.Fill = new SolidColorBrush(Color.FromRgb(0x5C, 0x5C, 0x66));
        }

        private void Enter_Top(object sender, RoutedEventArgs e)
        {
            top.Fill = new SolidColorBrush(Color.FromRgb(0x25, 0xAE, 0xF3));
        }
        private void Leave_Top(object sender, RoutedEventArgs e)
        {
            if (config.readbool("topTag") == 1)
            {
                top.Fill = new SolidColorBrush(Color.FromRgb(0xE0, 0x0B, 0xC4));
            }
            else
            {
                top.Fill = new SolidColorBrush(Color.FromRgb(0x5C, 0x5C, 0x66));
            }
            
        }
        private void Enter_Bar(object sender, RoutedEventArgs e)
        {
            statusBar.Fill = new SolidColorBrush(Color.FromRgb(0x25, 0xAE, 0xF3));
        }
        private void Leave_Bar(object sender, RoutedEventArgs e)
        {
            if (config.readbool("barTag")==1)
            {
                statusBar.Fill = new SolidColorBrush(Color.FromRgb(0xE0, 0x0B, 0xC4));
            }
            else
            {
                statusBar.Fill = new SolidColorBrush(Color.FromRgb(0x5C, 0x5C, 0x66));
            }
            
        }

        private void Enter_Img(object sender, RoutedEventArgs e)
        {

           img.Fill = new SolidColorBrush(Color.FromRgb(0x25, 0xAE, 0xF3));
        }
        private void Img_Up(object sender, RoutedEventArgs e)
        {
            img.Fill = new SolidColorBrush(Color.FromRgb(0x5C, 0x5C, 0x66));
            clickChangeTag++;                       
            if (clickChangeTag >= GetIconCount(System.IO.Path.GetFullPath(@"image")))
            {
                clickChangeTag = 0;
            }
            config.write("clickChangeTag", clickChangeTag);
            Icons(false);
            Icons(true);
        }
        private void Img_Down(object sender, RoutedEventArgs e)
        {
            img.Fill = new SolidColorBrush(Color.FromRgb(0x25, 0xAE, 0xF3));
        }
            private void Leave_Img(object sender, RoutedEventArgs e)
        {
            img.Fill = new SolidColorBrush(Color.FromRgb(0x5C, 0x5C, 0x66));
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                WindowState = WindowState.Normal;
                DragMove();
            }
        }
        private void SetTab()
        {
            tabList.Children.Clear();

            Action<ClipboardType> action = (ClipboardType type) =>
              {
                  tabSelect = type;
                  foreach (TabItem _item in tabList.Children)
                  {
                      _item.SetSelect(false);
                  }
                  ShuaXin();
              };

            TabItem item = new TabItem(ClipboardType.All,"全部");
            item.SetSelect(true);
            TabItem item1 = new TabItem(ClipboardType.Text, "文本");
            TabItem item2 = new TabItem(ClipboardType.HtmlText, "富文本");
            TabItem item3 = new TabItem(ClipboardType.Image, "图片");
            TabItem item4 = new TabItem(ClipboardType.File, "文件");
            item.onSelect = action;
            item1.onSelect = action;
            item2.onSelect = action;
            item3.onSelect = action;
            item4.onSelect = action;
            tabList.Children.Add(item);
            tabList.Children.Add(item1);
            tabList.Children.Add(item2);
            tabList.Children.Add(item3);
            tabList.Children.Add(item4);
           
        }
        public void showInTaskBar(bool tag)
        {
            if (tag) {
                ShowInTaskbar = true;
            }
            else
            {
                ShowInTaskbar = false;
            }
            
        }
        
        private void Icons(bool b)
        {
            if (b)
            {
                if (this.notifyIcon == null)
                {
                    this.notifyIcon = new System.Windows.Forms.NotifyIcon();
                    // 可以添加其他初始化代码，如设置托盘图标的初始属性等  
                }
                string[] a = (string[])GetIcons(System.IO.Path.GetFullPath(@"image"));
                clickChangeTag = int.Parse(config.readstring("clickChangeTag"));
                //MessageBox.Show(a[clickChangeTag]+"---"+clickChangeTag);
                Icon = new BitmapImage(new Uri(System.IO.Path.GetFullPath(@"image/" + a[clickChangeTag])));

                if (clickChangeTag >= GetIconCount(System.IO.Path.GetFullPath(@"image")))
                {
                    clickChangeTag = 0;
                    config.write("clickChangeTag", clickChangeTag);
                }         
                UpdateIcons(System.IO.Path.GetFullPath(@"image/" + a[int.Parse(config.readstring("clickChangeTag"))])); // 调用更新图标的函数  
            }
            else
            {              
               
                notifyIcon.Visible = false; // 隐藏系统托盘图标  
                notifyIcon = null;
            }
            if (notifyIcon != null)
            {
                contextMenu();
            }
           
        }
        private void UpdateIcons(string path)
        {
            if (File.Exists(path))
            {
                
                // 如果系统托盘图标存在，则更新图标  
                //Icon newIcon = new Icon(path);
                notifyIcon.Icon = new Icon(path);
                notifyIcon.Visible = true;
                notifyIcon.BalloonTipText = "剪切板";
                notifyIcon.Text = "剪切板";
                notifyIcon.MouseDoubleClick += NotifyIcon_MouseClick;
            }
            else
            {
                // 如果系统托盘图标不存在，则隐藏图标  
                notifyIcon.Visible = false;
            }
        }
        static int GetIconCount(string folderPath)
        {
            int count = 0;
            try
            {
                // 获取文件夹中的所有文件  
                var files = Directory.GetFiles(folderPath);
                // 筛选出.ico文件  
                var icons = files.Where(file => file.EndsWith(".ico"));
                // 计算数量  
                count = icons.Count();
            }
            catch (Exception ex)
            {
                // 输出错误信息，例如文件夹不存在或没有访问权限等  
                Console.WriteLine($"出现错误: {ex.Message}");
            }
            return count;
        }
        static Array GetIcons(string folderPath)
        {
            string[] fileNames = Directory.GetFiles(folderPath, "*.ico");               
            string[] icoFileNames = fileNames.Where(fileName => fileName.ToLower().EndsWith(".ico")).ToArray(); // 过滤出.ico文件
            List<string> icoFileNamesOnly = new List<string>();  // 使用List<string>来保存文件名，因为我们不知道有多少个.ico文件 
            foreach (string icoFileName in icoFileNames)
            {                 
                string fileNameOnly = System.IO.Path.GetFileName(icoFileName);// 获取文件名并添加到列表中 
                icoFileNamesOnly.Add(fileNameOnly);
            }             
            return icoFileNamesOnly.ToArray();// 将列表转换为数组并返回 
        }
        private void contextMenu()
        {
            System.Windows.Forms.ContextMenuStrip cms = new System.Windows.Forms.ContextMenuStrip();

            //关联 NotifyIcon 和 ContextMenuStrip
            notifyIcon.ContextMenuStrip = cms;

            System.Windows.Forms.ToolStripMenuItem exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exitMenuItem.Text = "退出";
            exitMenuItem.Click += new EventHandler(exitMenuItem_Click);

            /* System.Windows.Forms.ToolStripMenuItem settingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
             settingMenuItem.Text = "设置";
             settingMenuItem.Click += new EventHandler(settingMenuItem_Click);*/

            System.Windows.Forms.ToolStripMenuItem hideMenumItem = new System.Windows.Forms.ToolStripMenuItem();           
            hideMenumItem.Text = "隐藏";
            hideMenumItem.Click += new EventHandler(hideMenumItem_Click);

            System.Windows.Forms.ToolStripMenuItem showMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            showMenuItem.Text = "显示";
            showMenuItem.Click += new EventHandler(showMenuItem_Click); 
       
            cms.Items.Add(showMenuItem);
            cms.Items.Add(hideMenumItem);
            //cms.Items.Add(settingMenuItem);
            cms.Items.Add(exitMenuItem);
        }
        private void settingMenuItem_Click(object sender, EventArgs e)
        {
            var settingWindow = new Setting(); // 假设SettingWindow是你的设置界面的类  
            settingWindow.Show();
        }
        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Close();
            Environment.Exit(0);
            Application.Current.Shutdown();
            
        }

        private void hideMenumItem_Click(object sender, EventArgs e)
        {
            flagAltV = false;
            this.Visibility = Visibility.Collapsed;
        }

        private void showMenuItem_Click(object sender, EventArgs e)
        {
            flagAltV = true;
            this.Visibility = Visibility.Visible;
        }
        private void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //MessageBox.Show(this.Visibility+"");
            if (this.Visibility == Visibility.Visible)
            {
                Application.Current.MainWindow.Hide();
            }
            else
            {
                Application.Current.MainWindow.Show();
            }         
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            //挂消息钩子
            mNextClipBoardViewerHWnd = SetClipboardViewer(source.Handle);
            source.AddHook(WndProc);

            // 获取窗体句柄
            m_Hwnd = new WindowInteropHelper(this).Handle;
            HwndSource hWndSource = HwndSource.FromHwnd(m_Hwnd);
            // 添加处理程序
           if (hWndSource != null) hWndSource.AddHook(WndProc2);
        }
        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {

       //     var clip = new ClipboardItem();
            switch (msg)
            {
                case WM_DRAWCLIPBOARD:
                    {
                        if (ICopy == 1) return IntPtr.Zero;
                        SendMessage(mNextClipBoardViewerHWnd, msg, wParam.ToInt32(), lParam.ToInt32());
                        if (true)
                        {
                            try
                            {
                                if (Clipboard.ContainsImage())
                                {
                                    System.Drawing.Image img2 = System.Windows.Forms.Clipboard.GetImage();
                                  
                                    BitmapSource img = ConvertToBitmapSource(img2);//wpf的部分图片截图无法获取，改用Winfrom的这里用于转换
                                    ClipboardItem clipboardItem = new ClipboardItem();
                                        clipboardItem.Type = ClipboardType.Image;
                                        clipboardItem.Text = "";
                                        clipboardItem.Data = img;
                                        AddClipboardItem(clipboardItem);
                                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                                    encoder.Frames.Add(BitmapFrame.Create(img));

                                  /*  using (FileStream fileStream = new FileStream("image.png", FileMode.Create))
                                    {
                                        // 将图像编码并保存到文件流中
                                        encoder.Save(fileStream);
                                    }*/
                                    

                                }
                                else if (Clipboard.ContainsFileDropList())
                                {
                                    StringCollection ss = Clipboard.GetFileDropList();
                                    string textdata = "";
                                    foreach (string text in ss)
                                    {
                                        string t = "";
                                        t = text;
                                        textdata += t + "\n";
                                    }
                                    ClipboardItem clipboardItem = new ClipboardItem();
                                    clipboardItem.Type = ClipboardType.File;
                                    clipboardItem.Text = textdata;
                                    clipboardItem.Data = ss;
                                    AddClipboardItem(clipboardItem);
                                }
                                //文本内容检测
                                else if (System.Windows.Clipboard.ContainsText(TextDataFormat.Html))
                                {
                                    ClipboardItem clipboardItem = new ClipboardItem();
                                    clipboardItem.Type = ClipboardType.HtmlText;
                                    clipboardItem.Text= Clipboard.GetText();
                                    clipboardItem.Data = Clipboard.GetText(TextDataFormat.Html);
                                    AddClipboardItem(clipboardItem);
                                   
                                }
                                else if (System.Windows.Clipboard.ContainsText())
                                {
                                    string text=  Clipboard.GetText();
                                    ClipboardItem clipboardItem = new ClipboardItem();
                                    clipboardItem.Type = ClipboardType.Text;
                                    clipboardItem.Text = text;
                                    clipboardItem.Data = text;
                                    AddClipboardItem(clipboardItem);

                                }
                               
                                 
                                /*if (vm.ClipboardsItems.All(c => c.HashCode != clip.HashCode) && clip.HashCode != 0)
                                {
                                    vm.ClipboardsItems.Insert(0, clip);
                                }*/
                            }
                        
                            catch (Exception)
                            {
                               // LogHelper.Instance._logger.Error(ex);
                            }
                        }
                        else
                        {
                        //    HotKeyHelper.IsIgnorance = false;
                        }
                    }
                    break;
                case WM_CHANGECBCHAIN:
                    {
                        if (wParam == (IntPtr)mNextClipBoardViewerHWnd)
                        {
                            mNextClipBoardViewerHWnd = lParam;
                        }
                        else
                        {
                            SendMessage(mNextClipBoardViewerHWnd, msg, wParam.ToInt32(), lParam.ToInt32());
                        }
                    }
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }
        public BitmapSource ConvertToBitmapSource(System.Drawing.Image image)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                try {
                    image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                }
                catch (Exception) { 
                }
                // 将 System.Drawing.Image 保存到内存流中
               

                // 创建一个 BitmapSource
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }
        public bool AreBitmapSourcesEqual(BitmapSource bitmapSource1, BitmapSource bitmapSource2)
        {
            if (bitmapSource1.PixelWidth != bitmapSource2.PixelWidth || bitmapSource1.PixelHeight != bitmapSource2.PixelHeight)
            {
                return false; // 如果尺寸不同，直接返回false
            }

            int bytesPerPixel = (bitmapSource1.Format.BitsPerPixel + 7) / 8; // 计算每个像素占用的字节数
            int stride = bitmapSource1.PixelWidth * bytesPerPixel; // 计算图像的跨距

            byte[] pixels1 = new byte[bitmapSource1.PixelHeight * stride];
            byte[] pixels2 = new byte[bitmapSource2.PixelHeight * stride];

            bitmapSource1.CopyPixels(pixels1, stride, 0);
            bitmapSource2.CopyPixels(pixels2, stride, 0);

            for (int i = 0; i < pixels1.Length; i++)
            {
                if (pixels1[i] != pixels2[i])
                {
                    return false; // 如果发现任何一个像素不一样，就返回false
                }
            }

            return true; // 如果所有像素都一样，返回true
        }
        public void AddClipboardItem(ClipboardItem clipboardItem)
        {
            foreach (ClipboardItem data in clipboardItems)
            {
                if(clipboardItem.Type== ClipboardType.Image&& data.Type == ClipboardType.Image)
                {
                    if (AreBitmapSourcesEqual((BitmapSource)clipboardItem.Data, (BitmapSource)data.Data))
                    {
                        clipboardItems.Remove(data);
                        clipboardItems.Insert(0, clipboardItem);
                        ShuaXin();
                        return;
                    }
                }
                else
                {
                    if (clipboardItem.Text == data.Text)
                    {
                        clipboardItems.Remove(data);
                        clipboardItems.Insert(0, clipboardItem);
                        ShuaXin();
                        return;
                    }
                }
            }
            if (clipboardItems.Count >= 500)
            {
                clipboardItems.Remove(clipboardItems[clipboardItems.Count - 1]);
                clipboardItems.Insert(0, clipboardItem);
                ShuaXin();
            }
            else
            {
                clipboardItems.Insert(0, clipboardItem);
                Item item = new Item(clipboardItem);
                item.onCope += onCope;
                item.onDelete += onDelete;
                item.onTop += onTop;
                if (tabSelect == ClipboardType.All || tabSelect == clipboardItem.Type)
                {
                    if (tbSearch.Text == string.Empty || clipboardItem.Text.Contains(tbSearch.Text))
                    {
                        item.setXuHao(itemList.Children.Count + itemTopList.Children.Count);
                        itemList.Children.Insert(0, item);
                    }
                }
                ShuaXinXuHao();
            }
        }
        public void ShuaXin()
        {
            itemList.Children.Clear();
            itemTopList.Children.Clear();
            foreach (ClipboardItem data in clipboardItems)
            {
                if (tabSelect == ClipboardType.All || tabSelect == data.Type)
                {
                    if (tbSearch.Text == string.Empty || data.Text.Contains(tbSearch.Text))
                    {
                        if (data.IsTop)
                        {
                            Item item = new Item(data);
                            item.onCope += onCope;
                            item.onDelete += onDelete;
                            item.onTop += onTop;
                            itemTopList.Children.Add(item);
                        }
                        else
                        {
                            Item item = new Item(data);
                            item.onCope += onCope;
                            item.onDelete += onDelete;
                            item.onTop += onTop;
                            item.setXuHao(itemList.Children.Count + itemTopList.Children.Count);
                            itemList.Children.Add(item);
                        }
                    }
                }
            }
            ShuaXinXuHao();

        }
        public void ShuaXinXuHao()
        {
            int i = 1;
            foreach (Item item in itemTopList.Children)
            {
                item.setXuHao(i++);
            }
            foreach (Item item in itemList.Children)
            {
                item.setXuHao(i++);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
        public const int WM_HOTKEY = 0x312;
        /// <summary>
        /// 记录快捷键注册项的唯一标识符
        /// </summary>
        private Dictionary<EHotKeySetting, int> m_HotKeySettings = new Dictionary<EHotKeySetting, int>();
        /// 
        /// 窗体回调函数，接收所有窗体消息的事件处理函数
        /// 
        /// 窗口句柄
        /// 消息
        /// 附加参数1
        /// 附加参数2
        /// 是否处理
        /// 返回句柄
        private IntPtr WndProc2(IntPtr hWnd, int msg, IntPtr wideParam, IntPtr longParam, ref bool handled)
        {
            //var hotkeySetting = new EHotKeySetting();
            switch (msg)
            {
                case HotKeyManager.WM_HOTKEY:
                    int sid = wideParam.ToInt32();
                    //if (sid == m_HotKeySettings[EHotKeySetting.复制])
                    //{
                    //    hotkeySetting = EHotKeySetting.复制;
                    //    //TODO 执行全屏操作
                    //}
                    if (sid == m_HotKeySettings[EHotKeySetting.剪贴板])
                    {
                        //hotkeySetting = EHotKeySetting.剪贴板;
                        if (Application.Current.MainWindow == null)
                        {
                            Application.Current.MainWindow = new Window();
                        }
                        flagAltV = !flagAltV;
                        if (flagAltV)
                        {
                            Application.Current.MainWindow.Show();
                        }
                        else
                        {
                            Application.Current.MainWindow.Hide();
                        }
                        
                    }
                    
                    //MessageBox.Show(string.Format("触发【{0}】快捷键", hotkeySetting.ToString()));
                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }
        /// 
        /// 所有控件初始化完成后调用
        /// 
        /// 
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            // 注册热键
            InitHotKey();
        }
        /// 
        /// 初始化注册快捷键
        /// 
        /// 待注册热键的项
        /// true:保存快捷键的值；false:弹出设置窗体
        private bool InitHotKey(ObservableCollection<HotKeyModel> hotKeyModelList = null)
        {
            var list = hotKeyModelList ?? HotKeySettingsManager.Instance.LoadDefaultHotKey();
            // 注册全局快捷键
            string failList = HotKeyHelper.RegisterGlobalHotKey(list, m_Hwnd, out m_HotKeySettings);
            if (string.IsNullOrEmpty(failList))
                return true;

            //MessageBoxResult mbResult = MessageBox.Show("提示", string.Format("无法注册下列快捷键\n\r{0}是否要改变这些快捷键？", failList), MessageBoxButton.YesNo);

            return true;
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            flagAltV = !flagAltV;
            this.Visibility = Visibility.Collapsed;
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void onCope(ClipboardItem clipboardItem)
        {
            try
            {

                ICopy = 1;
                switch (clipboardItem.Type)
                {
                    case ClipboardType.Text:
                        System.Windows.Forms.Clipboard.SetText(clipboardItem.Data.ToString());
                        break;
                    case ClipboardType.HtmlText:
                        System.Windows.Forms.Clipboard.SetData(DataFormats.UnicodeText, clipboardItem.Text);
                        System.Windows.Forms.Clipboard.SetData(DataFormats.Html, clipboardItem.Data.ToString());

                        //Clipboard.SetData(DataFormats.Rtf, clipboardItem.Data.ToString());
                        break;
                    case ClipboardType.File:
                        System.Windows.Forms.Clipboard.SetFileDropList((StringCollection)clipboardItem.Data);
                        break;
                    case ClipboardType.Image:
                        Clipboard.SetImage((BitmapImage)clipboardItem.Data);
                        break;
                }
                clipboardItems.Remove(clipboardItem);
                clipboardItems.Insert(0, clipboardItem);
                ShuaXin();
                ICopy = 0;
            }
            catch
            {

            }

        }
        private void onDelete(Item item,ClipboardItem clipboardItem)
        {
            clipboardItems.Remove(clipboardItem);
            itemList.Children.Remove(item);
            itemTopList.Children.Remove(item);
            ShuaXinXuHao();
        }
        private void onTop(Item item, bool isTop)
        {
            if(isTop)
            {
                itemList.Children.Remove(item);
                itemTopList.Children.Insert(0,item);
            }
            else
            {
                itemTopList.Children.Remove(item);
                itemList.Children.Insert(0,item);
            }
            ShuaXinXuHao();
        }

        private void tbSearch_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ShuaXin();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Released)
            {
                clipboardItems.Clear();
                ShuaXin();
            }            
        }
    }
}
