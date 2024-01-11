using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace WpfClipboard
{
    public class MainVM : ValidationBase
    {
        ClipForty win;
        public static MainVM Instance;
        public MainVM(ClipForty window)
        {
            win = window;
            ClipboardsItems = new ObservableCollection<ClipboardItem>();
            Instance = this;
        }

        private ObservableCollection<ClipboardItem> clipboardsItems;
        public ObservableCollection<ClipboardItem> ClipboardsItems
        {
            get { return clipboardsItems; }
            set { clipboardsItems = value; NotifyPropertyChanged(nameof(ClipboardsItems)); }
        }
    }
}
