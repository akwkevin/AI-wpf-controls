using System;
using System.Windows;
using System.Windows.Media;
using AIStudio.Wpf.Controls.Event;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// NoticeWindow.xaml 的交互逻辑
    /// </summary>
    internal partial class NoticeWindow : System.Windows.Window
    {
        public NoticeWindow()
        {
            InitializeComponent();
            Instance = this;
        }

        public static string Identifier { get; set; } = "Desktop";

        #region Property
        public static NoticeWindow Instance
        {
            get; set;
        }
        #endregion

        #region EventHandler

        private void NoticeWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Instance = null;
        }
        #endregion

    }
}
