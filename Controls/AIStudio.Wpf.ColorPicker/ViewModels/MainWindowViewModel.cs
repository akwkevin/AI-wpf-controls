using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
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
        bool _stop = false;
        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        void hook_OnMouseActivity(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (_isActivity == true || _stop == true)
                return;

            _isActivity = true;
            Position = "X:" + e.X + " Y:" + e.Y;

            var color = ColorPickerManager.GetColor(e.X, e.Y);

            DecimalColor = color.R.ToString() + "," + color.G.ToString() + "," + color.B.ToString();
            Color = color.ToString();

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
            Debug.WriteLine(e.KeyValue.ToString() + e.Control + e.KeyData + Keyboard.IsKeyDown(Key.LeftCtrl)) ;
            if (e.KeyCode == System.Windows.Forms.Keys.C && Control.ModifierKeys == Keys.Control)
            {
                System.Windows.Clipboard.SetDataObject(Color);
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.S && Control.ModifierKeys == Keys.Control)
            {
                _stop = !_stop;
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
