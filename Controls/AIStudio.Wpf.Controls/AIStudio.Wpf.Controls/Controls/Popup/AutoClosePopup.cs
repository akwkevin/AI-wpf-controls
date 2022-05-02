using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace AIStudio.Wpf.Controls
{
    public class AutoClosePopup : Popup
    {
        protected override void OnOpened(EventArgs e)
        {
            FrameworkElement target = this.PlacementTarget as FrameworkElement;
            if (target != null)
            {
                target.MouseLeave += (s, ev) => {
                    if (timerCloseRecentPopup == null)
                    {
                        timerCloseRecentPopup = new DispatcherTimer();
                        timerCloseRecentPopup.Interval = new TimeSpan(0, 0, 1);
                        timerCloseRecentPopup.Tag = this;
                        timerCloseRecentPopup.Tick += closePopup;
                    }
                    timerCloseRecentPopup.Stop();
                    timerCloseRecentPopup.Start();
                };
            }
        }

        /// <summary>
        /// 定时关闭 更多 popup窗口
        /// </summary>
        private DispatcherTimer timerCloseRecentPopup;
        private void closePopup(object state, EventArgs e)
        {
            Popup pop = timerCloseRecentPopup.Tag as Popup;
            if (pop == null)
            {
                //todo timer里面的Assert没有对话框出来
                Debug.Assert(true, "pop==null");
                return;
            }

            FrameworkElement target = pop.PlacementTarget as FrameworkElement;

            if (!pop.IsMouseOver)
            {
                if (target != null)
                {
                    if (!target.IsMouseOver)
                    {
                        pop.IsOpen = false;
                        timerCloseRecentPopup.Stop();
                    }

                }
                else
                {
                    pop.IsOpen = false;
                    timerCloseRecentPopup.Stop();
                }
            }

        }
    }
}
