using System;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;


namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// 媒体进度条
    /// </summary>
    public class SliderProgress : Slider
    {
        private ToolTip _autoToolTip;
        private string _autoToolTipFormat;

        public string AutoToolTipFormat
        {
            get
            {
                return _autoToolTipFormat;
            }
            set
            {
                _autoToolTipFormat = value;
            }
        }

        protected override void OnThumbDragStarted(DragStartedEventArgs e)
        {
            base.OnThumbDragStarted(e);
            this.FormatAutoToolTipContent();
        }

        protected override void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            base.OnThumbDragDelta(e);
            this.FormatAutoToolTipContent();
        }

        private void FormatAutoToolTipContent()
        {
            if (!string.IsNullOrWhiteSpace(this.AutoToolTipFormat))
            {
                TimeSpan s = TimeSpan.FromSeconds(double.Parse(this.AutoToolTip.Content.ToString()));
                this.AutoToolTip.Content = s.ToString("mm\\:ss");
            }
        }

        private ToolTip AutoToolTip
        {
            get
            {
                if (_autoToolTip == null)
                {
                    FieldInfo field = typeof(Slider).GetField("_autoToolTip", BindingFlags.NonPublic | BindingFlags.Instance);

                    _autoToolTip = field.GetValue(this) as ToolTip;
                }

                return _autoToolTip;
            }
        }
    }

}
