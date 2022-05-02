using System;
using System.Windows;
using System.Windows.Media;
using AIStudio.Wpf.Controls.Event;

namespace AIStudio.Wpf.Controls
{
    public class Notice
    {
        public static void Show(
            string message, 
            string title, 
            double? durationSeconds, 
            ControlStatus noticeIcon = ControlStatus.Mid, 
            NoticeCardStyle noticeCardStyle = NoticeCardStyle.Standard, 
            double? width = null,
            double? height = null,
            HorizontalAlignment? horizontalAlignment = null, 
            VerticalAlignment? verticalAlignment = null, 
            bool? showSure = null, 
            bool showClose = true, 
            Action<MessageBoxResult> actionClose = null, 
            string identifier = null)
        {
            if (NoticeWindow.Instance == null)
            {
                var window = new NoticeWindow();
                window.Show();
                Application.Current.MainWindow.Closed += MainWindow_Closed;
            }
            NoticeCard.AddNotice(
                message,
                title,
                durationSeconds,
                noticeIcon, 
                noticeCardStyle,
                width,
                height,
                horizontalAlignment, 
                verticalAlignment,
                showSure, 
                showClose, 
                actionClose, 
                identifier);
        }

        public static void Clear(string identifier = null)
        {
            NoticeCard.ClearNotice(identifier);
        }

        private static void MainWindow_Closed(object sender, System.EventArgs e)
        {
            if (NoticeWindow.Instance != null)
            {
                NoticeWindow.Instance.Close();
            }
        }



    }
}
