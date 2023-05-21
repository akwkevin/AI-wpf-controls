using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Media;

namespace AIStudio.Wpf.ColorPicker.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        GlobalHook _hook;
        public MainWindowViewModel()
        {
            _hook = new GlobalHook();
            _hook.KeyDown += new System.Windows.Forms.KeyEventHandler(hook_KeyDown);
            _hook.KeyPress += new System.Windows.Forms.KeyPressEventHandler(hook_KeyPress);
            _hook.KeyUp += new System.Windows.Forms.KeyEventHandler(hook_KeyUp);
            _hook.OnMouseActivity += new System.Windows.Forms.MouseEventHandler(hook_OnMouseActivity);
            _hook.Start();
        }

        bool _isActivity = false;
        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        void hook_OnMouseActivity(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (_isActivity == true)
                return;

            _isActivity = true;
            Position = "X:" + e.X + " Y:" + e.Y;

            POINT point;

            GetCursorPos(out point);

            IntPtr hdc = GetDC(IntPtr.Zero);
            uint c = GetPixel(hdc, point.X, point.Y);
            ReleaseDC(IntPtr.Zero, hdc);

            byte r = Convert.ToByte(c & 0xFF);
            var g = Convert.ToByte((c & 0xFF00) / 256);
            var b = Convert.ToByte((c & 0xFF0000) / 65536);

            DecimalColor = r.ToString() + "," + g.ToString() + "," + b.ToString();
            Color = System.Windows.Media.Color.FromRgb(r, g, b).ToString();
            //Thread.Sleep(10);
            _isActivity = false;
        }
        /// <summary>
        /// 键盘抬起
        /// </summary>
        void hook_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
           
        }
        /// <summary>
        /// 键盘输入
        /// </summary>
        void hook_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
        }
        /// <summary>
        /// 键盘按下
        /// </summary>
        void hook_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Space)
            {
                System.Windows.Clipboard.Clear();
                System.Windows.Clipboard.SetData(System.Windows.DataFormats.Text, Color);
            }
        }

        public void Dispose()
        {
            _hook.Stop();
        }

        private string _position;

        /// <summary>
        /// 位置
        /// </summary>
        public string Position
        {
            get => _position;
            set
            {
                _position = value;
                OnPropertyChanged();
            }
        }

        private string _color;

        /// <summary>
        /// 颜色
        /// </summary>
        public string Color
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged();
            }
        }

        private string _decimalColor;
        /// <summary>
        /// 十进制颜色
        /// </summary>
        public string DecimalColor
        {
            get => _decimalColor;
            set
            {
                _decimalColor = value;
                OnPropertyChanged();
            }
        }      

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);
        /// <summary>   
        /// 获取鼠标的坐标   
        /// </summary>   
        /// <param name="lpPoint">传址参数，坐标point类型</param>   
        /// <returns>获取成功返回真</returns>   
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out POINT pt);

        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        public static extern IntPtr GetDesktopWindow();

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性修改事件
        /// </summary>
        /// <param name="propertyName">属性名</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
