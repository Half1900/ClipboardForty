using Clipboards;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
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

namespace WpfClipboard
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        List<ClipboardItem> clipboardItems = new List<ClipboardItem>();
        int ICopy=0;
        private const int WM_DRAWCLIPBOARD = 0x308;
        private const int WM_CHANGECBCHAIN = 0x30D;
        private IntPtr mNextClipBoardViewerHWnd;
        ClipboardType tabSelect = ClipboardType.All;
        bool tag = false;
        bool flagAltV = false;
        System.Windows.Forms.NotifyIcon notifyIcon;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern bool ChangeClipboardChain(IntPtr HWnd, IntPtr HWndNext);

        [DllImport("user32.dll")]
        private static extern bool SetIcon(IntPtr hWnd, string iconPath);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        private IntPtr m_Hwnd = new IntPtr();
        public MainWindow()
        {
            Process[] processes = Process.GetProcesses();     //获得本机所有应用进程
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
                Environment.Exit(1);
                return;
            }
            InitializeComponent();
            flagAltV = !flagAltV;
            icon();
            contextMenu();
            SetTab();
            DragMove();
            itemList.Children.Clear();
            Topmost = true;
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
            private void icon()
        {
            string path = System.IO.Path.GetFullPath(@"image/clipboard.ico");
            System.Drawing.Icon nIcon = new System.Drawing.Icon(path);//程序图标
            
            if (File.Exists(path))
            {
                this.notifyIcon = new System.Windows.Forms.NotifyIcon();
                this.notifyIcon.BalloonTipText = "剪切板"; 
                this.notifyIcon.Text = "剪切板";
                this.notifyIcon.Icon = nIcon; // System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath) ;//
                this.notifyIcon.Visible = true;
                notifyIcon.MouseDoubleClick += NotifyIcon_MouseClick; 

            }
        }
        private void contextMenu()
        {
            System.Windows.Forms.ContextMenuStrip cms = new System.Windows.Forms.ContextMenuStrip();

            //关联 NotifyIcon 和 ContextMenuStrip
            notifyIcon.ContextMenuStrip = cms;

            System.Windows.Forms.ToolStripMenuItem exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exitMenuItem.Text = "退出";
            exitMenuItem.Click += new EventHandler(exitMenuItem_Click);
           
            System.Windows.Forms.ToolStripMenuItem settingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            settingMenuItem.Text = "设置";
            settingMenuItem.Click += new EventHandler(settingMenuItem_Click);

            System.Windows.Forms.ToolStripMenuItem hideMenumItem = new System.Windows.Forms.ToolStripMenuItem();
            hideMenumItem.Text = "隐藏";
            hideMenumItem.Click += new EventHandler(hideMenumItem_Click);

            System.Windows.Forms.ToolStripMenuItem showMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            showMenuItem.Text = "显示";
            showMenuItem.Click += new EventHandler(showMenuItem_Click);

            cms.Items.Add(showMenuItem);
            cms.Items.Add(hideMenumItem);
            cms.Items.Add(settingMenuItem);
            cms.Items.Add(exitMenuItem);
        }
        private void settingMenuItem_Click(object sender, EventArgs e)
        {
            var settingWindow = new Setting(); // 假设SettingWindow是你的设置界面的类  
            settingWindow.Show();
        }
        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void hideMenumItem_Click(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void showMenuItem_Click(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Visible;
        }
        private void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            tag = !tag;
            if (tag)
            {
                this.Visibility = Visibility.Visible;
            }
            else
            {
                this.Visibility = Visibility.Collapsed;
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
                                else   if (System.Windows.Clipboard.ContainsText(TextDataFormat.Html))
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
                        
                            catch (Exception ex)
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
                // 将 System.Drawing.Image 保存到内存流中
                image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                memoryStream.Seek(0, SeekOrigin.Begin);

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
            ICopy = 1;
            switch (clipboardItem.Type)
            {
                case ClipboardType.Text:
                    System.Windows.Forms.Clipboard.SetText(clipboardItem.Data.ToString());
                    break;
                case ClipboardType.HtmlText:
                    System.Windows.Forms.Clipboard.SetData(DataFormats.UnicodeText, clipboardItem.Text);
                    System.Windows.Forms.Clipboard.SetData(DataFormats.Html, clipboardItem.Data.ToString());

                    //再帮我加个状态栏和exe文件显示图标把

                    //Clipboard.SetData(DataFormats.Rtf, clipboardItem.Data.ToString());
                    break;
                case ClipboardType.File:
                    Clipboard.SetFileDropList((StringCollection)clipboardItem.Data);
                    break;
                case ClipboardType.Image:
                    Clipboard.SetImage((BitmapSource)clipboardItem.Data);
                    break;
            }
            clipboardItems.Remove(clipboardItem);
            clipboardItems.Insert(0, clipboardItem);
            ShuaXin();
            ICopy = 0;
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
