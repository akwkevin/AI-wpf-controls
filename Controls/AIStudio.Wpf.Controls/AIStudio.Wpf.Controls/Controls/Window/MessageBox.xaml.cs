using System;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    ///     消息框
    /// </summary>
    [TemplatePart(Name = PART_CancelButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_NoButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_OkButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_YesButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_CloseButton, Type = typeof(Button))]

    public sealed class MessageBox : Window
    {
        private const string PART_CancelButton = "PART_CancelButton";
        private const string PART_NoButton = "PART_NoButton";
        private const string PART_OkButton = "PART_OkButton";
        private const string PART_YesButton = "PART_YesButton";
        private const string PART_CloseButton = "PART_CloseButton";

        private Button _cancelButton;
        private Button _noButton;
        private Button _okButton;
        private Button _yesButton;
        private Button _closeButton;

        private MessageBoxResult _messageBoxResult = MessageBoxResult.Cancel;

        public bool _showOk;

        public bool _showCancel;

        public bool _showYes;

        public bool _showNo;

        public MessageBoxResult MessageBoxResult
        {
            get
            {
                return _messageBoxResult;
            }
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            nameof(Message), typeof(string), typeof(MessageBox), new PropertyMetadata(default(string)));

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
            nameof(Image), typeof(Geometry), typeof(MessageBox), new PropertyMetadata(default(Geometry)));

        public Geometry Image
        {
            get => (Geometry)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public static readonly DependencyProperty ImageBrushProperty = DependencyProperty.Register(
            nameof(ImageBrush), typeof(Brush), typeof(MessageBox), new PropertyMetadata(default(Brush)));

        public Brush ImageBrush
        {
            get => (Brush)GetValue(ImageBrushProperty);
            set => SetValue(ImageBrushProperty, value);
        }

        public static readonly DependencyProperty ShowImageProperty = DependencyProperty.Register(
            nameof(ShowImage), typeof(bool), typeof(MessageBox), new PropertyMetadata(false));

        public bool ShowImage
        {
            get => (bool)GetValue(ShowImageProperty);
            set => SetValue(ShowImageProperty, value);
        }

        static MessageBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageBox), new FrameworkPropertyMetadata(typeof(MessageBox)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_cancelButton != null)
            {
                _cancelButton.Click -= Button_Click;
            }
            if (_noButton != null)
            {
                _noButton.Click -= Button_Click;
            }
            if (_okButton != null)
            {
                _okButton.Click -= Button_Click;
            }
            if (_yesButton != null)
            {
                _yesButton.Click -= Button_Click;
            }
            if (_closeButton != null)
            {
                _closeButton.Click -= Button_Click;
            }

            _cancelButton = GetTemplateChild(PART_CancelButton) as Button;
            _noButton = GetTemplateChild(PART_NoButton) as Button;
            _okButton = GetTemplateChild(PART_OkButton) as Button;
            _yesButton = GetTemplateChild(PART_YesButton) as Button;
            _closeButton = GetTemplateChild(PART_CloseButton) as Button;

            if (_cancelButton != null)
            {
                _cancelButton.IsEnabled = _showCancel;
                _cancelButton.Click += Button_Click;
            }
            if (_noButton != null)
            {
                _noButton.IsEnabled = _showNo;
                _noButton.Click += Button_Click;
            }
            if (_okButton != null)
            {
                _okButton.IsEnabled = _showOk;
                if (_showOk)
                {
                    _okButton.Focus();
                }
                _okButton.Click += Button_Click;
            }
            if (_yesButton != null)
            {
                _yesButton.IsEnabled = _showYes;
                if (_showYes)
                {
                    _yesButton.Focus();
                }
                _yesButton.Click += Button_Click;
            }
            if (_closeButton != null)
            {
                _closeButton.Click += Button_Click;
            }
        }

        /// <summary>
        /// Sets the MessageBoxResult according to the button pressed and then closes the MessageBox.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = e.OriginalSource as Button;

            if (button == null)
                return;

            switch (button.Name)
            {
                case PART_NoButton:
                    _messageBoxResult = MessageBoxResult.No;
                    this.Close();
                    break;
                case PART_YesButton:
                    _messageBoxResult = MessageBoxResult.Yes;
                    this.Close();
                    break;
                case PART_CancelButton:
                    _messageBoxResult = MessageBoxResult.Cancel;
                    this.Close();
                    break;
                case PART_OkButton:
                    _messageBoxResult = MessageBoxResult.OK;
                    this.Close();
                    break;
                case PART_CloseButton:
                    _messageBoxResult = MessageBoxResult.Cancel;
                    this.Close();
                    break;
            }

            e.Handled = true;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            this.DragMove();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.C)
            {
                var builder = new StringBuilder();
                var line = new string('-', 27);
                builder.Append(line);
                builder.Append(Environment.NewLine);
                builder.Append(Title);
                builder.Append(Environment.NewLine);
                builder.Append(line);
                builder.Append(Environment.NewLine);
                builder.Append(Message);
                builder.Append(Environment.NewLine);
                builder.Append(line);
                builder.Append(Environment.NewLine);
                if (_showOk)
                {
                    builder.Append("Confirm");
                    builder.Append("   ");
                }
                if (_showYes)
                {
                    builder.Append("Yes");
                    builder.Append("   ");
                }
                if (_showNo)
                {
                    builder.Append("No");
                    builder.Append("   ");
                }
                if (_showCancel)
                {
                    builder.Append("Cancel");
                    builder.Append("   ");
                }
                builder.Append(Environment.NewLine);
                builder.Append(line);
                builder.Append(Environment.NewLine);
                Clipboard.SetText(builder.ToString());
            }


            if (_showYes && e.Key == Key.Y)
            {
                _messageBoxResult = MessageBoxResult.Yes;
                this.Close();
                e.Handled = true;
            }
            else if (_showNo && e.Key == Key.N)
            {
                _messageBoxResult = MessageBoxResult.No;
                this.Close();
                e.Handled = true;
            }
        }

        /// <summary>
        ///     成功
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        public static MessageBoxResult Success(string messageBoxText, string caption = "系统提示", System.Windows.Window owner = null)
        {
            MessageBox messageBox = null;
            Application.Current.Dispatcher.Invoke(new Action(() => {
                messageBox = CreateMessageBox(null, messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK);
                SetButtonStatus(messageBox, MessageBoxButton.OK);
                messageBox.ShowImage = true;
                messageBox.Image = Application.Current.TryFindResource("SuccessGeometry") as Geometry;
                messageBox.ImageBrush = Application.Current.TryFindResource("SuccessBrush") as Brush;
                SystemSounds.Asterisk.Play();
                if (owner != null)
                {
                    messageBox.Owner = owner;
                    messageBox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                }
                messageBox.ShowDialog();
            }));

            return messageBox._messageBoxResult;
        }

        /// <summary>
        ///     成功
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        public static MessageBoxResult Success(string messageBoxText, string windowIdentifier, string caption = "系统提示")
        {
            var owner = WindowBase.GetWindowBase(windowIdentifier);          

            return Success(messageBoxText, caption, owner);
        }

        /// <summary>
        ///     消息
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        public static MessageBoxResult Info(string messageBoxText, string caption = "系统提示", System.Windows.Window owner = null)
        {
            MessageBox messageBox = null;
            Application.Current.Dispatcher.Invoke(new Action(() => {
                messageBox = CreateMessageBox(null, messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                SetButtonStatus(messageBox, MessageBoxButton.OK);
                SetImage(messageBox, MessageBoxImage.Information);
                SystemSounds.Asterisk.Play();
                if (owner != null)
                {
                    messageBox.Owner = owner;
                    messageBox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                }
                messageBox.ShowDialog();
            }));

            return messageBox._messageBoxResult;
        }

        /// <summary>
        ///     消息
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        public static MessageBoxResult Info(string messageBoxText, string windowIdentifier, string caption = "系统提示")
        {
            var owner = WindowBase.GetWindowBase(windowIdentifier);

            return Info(messageBoxText, caption, owner);
        }

        /// <summary>
        ///     警告
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        public static MessageBoxResult Warning(string messageBoxText, string caption = "系统提示", System.Windows.Window owner = null)
        {
            MessageBox messageBox = null;
            Application.Current.Dispatcher.Invoke(new Action(() => {
                messageBox = CreateMessageBox(null, messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                SetButtonStatus(messageBox, MessageBoxButton.OK);
                SetImage(messageBox, MessageBoxImage.Warning);
                SystemSounds.Asterisk.Play();
                if (owner != null)
                {
                    messageBox.Owner = owner;
                    messageBox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                }
                messageBox.ShowDialog();
            }));

            return messageBox._messageBoxResult;
        }

        /// <summary>
        ///     警告
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        public static MessageBoxResult Warning(string messageBoxText, string windowIdentifier, string caption = "系统提示")
        {
            var owner = WindowBase.GetWindowBase(windowIdentifier);

            return Warning(messageBoxText, caption, owner);
        }

        /// <summary>
        ///     错误
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        public static MessageBoxResult Error(string messageBoxText, string caption = "系统提示", System.Windows.Window owner = null)
        {
            MessageBox messageBox = null;
            Application.Current.Dispatcher.Invoke(new Action(() => {
                messageBox = CreateMessageBox(null, messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                SetButtonStatus(messageBox, MessageBoxButton.OK);
                SetImage(messageBox, MessageBoxImage.Error);
                SystemSounds.Asterisk.Play();
                if (owner != null)
                {
                    messageBox.Owner = owner;
                    messageBox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                }
                messageBox.ShowDialog();
            }));

            return messageBox._messageBoxResult;
        }

        /// <summary>
        ///     错误
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        public static MessageBoxResult Error(string messageBoxText, string windowIdentifier, string caption = "系统提示")
        {
            var owner = WindowBase.GetWindowBase(windowIdentifier);

            return Error(messageBoxText, caption, owner);
        }

        /// <summary>
        ///     严重
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        public static MessageBoxResult Fatal(string messageBoxText, string caption = "系统提示", System.Windows.Window owner = null)
        {
            MessageBox messageBox = null;
            Application.Current.Dispatcher.Invoke(new Action(() => {
                messageBox = CreateMessageBox(null, messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK);
                SetButtonStatus(messageBox, MessageBoxButton.OK);
                messageBox.ShowImage = true;
                messageBox.Image = Application.Current.TryFindResource("FatalGeometry") as Geometry;
                messageBox.ImageBrush = Application.Current.TryFindResource("PrimaryTextBrush") as Brush;

                SystemSounds.Asterisk.Play();
                if (owner != null)
                {
                    messageBox.Owner = owner;
                    messageBox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                }
                messageBox.ShowDialog();
            }));

            return messageBox._messageBoxResult;
        }

        /// <summary>
        ///     严重
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        public static MessageBoxResult Fatal(string messageBoxText, string windowIdentifier, string caption = "系统提示")
        {
            var owner = WindowBase.GetWindowBase(windowIdentifier);

            return Fatal(messageBoxText, caption, owner);
        }

        /// <summary>
        ///     询问
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        public static MessageBoxResult Ask(string messageBoxText, string caption = "系统提示", System.Windows.Window owner = null)
        {
            MessageBox messageBox = null;
            Application.Current.Dispatcher.Invoke(new Action(() => {
                messageBox = CreateMessageBox(null, messageBoxText, caption, MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);
                SetButtonStatus(messageBox, MessageBoxButton.OKCancel);
                SetImage(messageBox, MessageBoxImage.Question);
                SystemSounds.Asterisk.Play();
                if (owner != null)
                {
                    messageBox.Owner = owner;
                    messageBox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                }
                messageBox.ShowDialog();
            }));

            return messageBox._messageBoxResult;
        }

        /// <summary>
        ///     询问
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        public static MessageBoxResult Ask(string messageBoxText, string windowIdentifier, string caption = "系统提示")
        {
            var owner = WindowBase.GetWindowBase(windowIdentifier);

            return Ask(messageBoxText, caption, owner);
        }

        /// <summary>
        ///     询问
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        public static MessageBoxResult YesNo(string messageBoxText, string caption = "系统提示", System.Windows.Window owner = null)
        {
            MessageBox messageBox = null;
            Application.Current.Dispatcher.Invoke(new Action(() => {
                messageBox = CreateMessageBox(null, messageBoxText, caption, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                SetButtonStatus(messageBox, MessageBoxButton.YesNo);
                SetImage(messageBox, MessageBoxImage.Question);
                SystemSounds.Asterisk.Play();
                if (owner != null)
                {
                    messageBox.Owner = owner;
                    messageBox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                }
                messageBox.ShowDialog();
            }));

            return messageBox._messageBoxResult;
        }

        /// <summary>
        ///     询问
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        public static MessageBoxResult YesNo(string messageBoxText, string windowIdentifier, string caption = "系统提示")
        {
            var owner = WindowBase.GetWindowBase(windowIdentifier);

            return YesNo(messageBoxText, caption, owner);
        }

        /// <summary>
        ///     信息展示
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        /// <param name="button"></param>
        /// <param name="icon"></param>
        /// <param name="defaultResult"></param>
        /// <returns></returns>
        public static MessageBoxResult Show(string messageBoxText, string caption = null, 
            MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None,
            MessageBoxResult defaultResult = MessageBoxResult.None, ControlStatus? controlStatus = null) =>
            Show(null, messageBoxText, caption, button, icon, defaultResult, controlStatus);

        /// <summary>
        ///     信息展示
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        /// <param name="button"></param>
        /// <param name="icon"></param>
        /// <param name="defaultResult"></param>
        /// <returns></returns>
        public static MessageBoxResult Show(System.Windows.Window owner, string messageBoxText, string caption = "系统提示", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None, MessageBoxResult defaultResult = MessageBoxResult.None, ControlStatus? controlStatus = null)
        {
            MessageBox messageBox = null;
            Application.Current.Dispatcher.Invoke(new Action(() => {
                messageBox = CreateMessageBox(owner, messageBoxText, caption, button, icon, defaultResult);
                SetButtonStatus(messageBox, button);
                SetImage(messageBox, icon);
                SetStatus(messageBox, controlStatus);
                SystemSounds.Asterisk.Play();
                messageBox.ShowDialog();
            }));

            return messageBox._messageBoxResult;
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, string iconKey, string iconBrushKey,
          MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None,
          MessageBoxResult defaultResult = MessageBoxResult.None) =>
          Show(null, messageBoxText, caption, iconKey, iconBrushKey, button, icon, defaultResult);

        /// <summary>
        ///     信息展示
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        /// <param name="button"></param>
        /// <param name="icon"></param>
        /// <param name="defaultResult"></param>
        /// <returns></returns>
        public static MessageBoxResult Show(System.Windows.Window owner, string messageBoxText, string caption, string iconKey, string iconBrushKey, MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None, MessageBoxResult defaultResult = MessageBoxResult.None)
        {
            MessageBox messageBox = null;
            Application.Current.Dispatcher.Invoke(new Action(() => {
                messageBox = CreateMessageBox(owner, messageBoxText, caption, button, icon, defaultResult);
                SetButtonStatus(messageBox, button);
                messageBox.ShowImage = true;
                messageBox.Image = Application.Current.TryFindResource(iconKey) as Geometry;
                messageBox.ImageBrush = Application.Current.TryFindResource(iconBrushKey) as Brush;
                SystemSounds.Asterisk.Play();
                messageBox.ShowDialog();
            }));

            return messageBox._messageBoxResult;
        }

        private static MessageBox CreateMessageBox(
            System.Windows.Window owner,
            string messageBoxText,
            string caption,
            MessageBoxButton button,
            MessageBoxImage icon,
            MessageBoxResult defaultResult)
        {
            if (!IsValidMessageBoxButton(button))
            {
                throw new InvalidEnumArgumentException(nameof(button), (int)button, typeof(MessageBoxButton));
            }
            if (!IsValidMessageBoxImage(icon))
            {
                throw new InvalidEnumArgumentException(nameof(icon), (int)icon, typeof(MessageBoxImage));
            }
            if (!IsValidMessageBoxResult(defaultResult))
            {
                throw new InvalidEnumArgumentException(nameof(defaultResult), (int)defaultResult, typeof(MessageBoxResult));
            }

            var ownerWindow = owner ?? Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            var ownerIsNull = ownerWindow is null;

            return new MessageBox
            {
                Message = messageBoxText,
                Owner = ownerWindow,
                WindowStartupLocation = owner is null? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner,
                Title = caption ?? string.Empty,
                Topmost = ownerIsNull,
                _messageBoxResult = defaultResult
            };
        }

        private static void SetButtonStatus(MessageBox messageBox, MessageBoxButton messageBoxButton)
        {
            switch (messageBoxButton)
            {
                case MessageBoxButton.OK:
                    messageBox._showOk = true;
                    break;
                case MessageBoxButton.OKCancel:
                    messageBox._showOk = true;
                    messageBox._showCancel = true;
                    break;
                case MessageBoxButton.YesNo:
                    messageBox._showYes = true;
                    messageBox._showNo = true;
                    break;
                case MessageBoxButton.YesNoCancel:
                    messageBox._showYes = true;
                    messageBox._showNo = true;
                    messageBox._showCancel = true;
                    break;
            }
        }

        private static void SetImage(MessageBox messageBox, MessageBoxImage messageBoxImage)
        {
            var iconKey = string.Empty;
            var iconBrushKey = string.Empty;

            switch (messageBoxImage)
            {
                case MessageBoxImage.Error:
                    iconKey = "ErrorGeometry";
                    iconBrushKey = "DangerBrush";
                    break;
                case MessageBoxImage.Question:
                    iconKey = "QuestionGeometry";
                    break;
                case MessageBoxImage.Warning:
                    iconKey = "WarningGeometry";
                    iconBrushKey = "WarningBrush";
                    break;
                case MessageBoxImage.Information:
                    iconKey = "InfoGeometry";
                    iconBrushKey = "InfoBrush";
                    break;
            }

            if (string.IsNullOrEmpty(iconKey)) return;
            messageBox.ShowImage = true;
            messageBox.Image = Application.Current.TryFindResource(iconKey) as Geometry;

            if (!string.IsNullOrEmpty(iconBrushKey))
                messageBox.ImageBrush = Application.Current.TryFindResource(iconBrushKey) as Brush;

        }

        private static void SetStatus(MessageBox messageBox, ControlStatus? controlStatus)
        {
            if (controlStatus != null)
            {
                messageBox.SetValue(ControlAttach.StatusProperty, controlStatus);
            }
        }

        private static bool IsValidMessageBoxButton(MessageBoxButton value)
        {
            return value == MessageBoxButton.OK
                   || value == MessageBoxButton.OKCancel
                   || value == MessageBoxButton.YesNo
                   || value == MessageBoxButton.YesNoCancel;
        }

        private static bool IsValidMessageBoxImage(MessageBoxImage value)
        {
            return value == MessageBoxImage.Asterisk
                   || value == MessageBoxImage.Error
                   || value == MessageBoxImage.Exclamation
                   || value == MessageBoxImage.Hand
                   || value == MessageBoxImage.Information
                   || value == MessageBoxImage.None
                   || value == MessageBoxImage.Question
                   || value == MessageBoxImage.Stop
                   || value == MessageBoxImage.Warning;
        }

        private static bool IsValidMessageBoxResult(MessageBoxResult value)
        {
            return value == MessageBoxResult.Cancel
                   || value == MessageBoxResult.No
                   || value == MessageBoxResult.None
                   || value == MessageBoxResult.OK
                   || value == MessageBoxResult.Yes;
        }
    }
}