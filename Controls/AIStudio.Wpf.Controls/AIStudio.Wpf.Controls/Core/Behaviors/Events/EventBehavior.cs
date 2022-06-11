using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Helper;

namespace AIStudio.Wpf.Controls.Behaviors
{
    #region Mouse Enter

    public class MouseEnterBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public MouseEnterBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            selectableObject.MouseEnter += (sender, args) => {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class MouseEnter : EventBase<MouseEnterBehavior>
    {
    }

    #endregion

    #region Mouse Leave

    public class MouseLeaveBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public MouseLeaveBehavior(FrameworkElement obj)
            : base(obj)
        {
            obj.MouseLeave += (sender, args) => {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class MouseLeave : EventBase<MouseLeaveBehavior>
    {
    }

    #endregion

    #region Mouse Click

    public class MouseClickBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public MouseClickBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            selectableObject.MouseDown += (sender, args) => {
                if (args.RightButton == MouseButtonState.Pressed) return;
                FrameworkElement element = sender as FrameworkElement;
                if (element == null) return;
                element.CaptureMouse();
            };

            selectableObject.MouseUp += (sender, args) => {
                if (args.RightButton == MouseButtonState.Pressed) return;
                FrameworkElement element = sender as FrameworkElement;
                if (element == null) return;
                if (element.IsMouseCaptured)
                {
                    element.ReleaseMouseCapture();
                    if (element.IsMouseOver)
                    {
                        SetDefaultParameter(new object[] { sender, args });
                        ExecuteCommand();
                    }
                }
            };
        }
    }

    public class MouseClick : EventBase<MouseClickBehavior>
    {
    }

    #endregion

    #region Mouse Double Click

    public class MouseDoubleClickBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public MouseDoubleClickBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            if (selectableObject is Control)
            {
                (selectableObject as Control).MouseDoubleClick += (sender, args) =>
                {
                    if (sender is DataGrid && args.OriginalSource != null && args.OriginalSource is DependencyObject)
                    {
                        if (VisualHelper.GetChild<DataGridColumnHeader>(
                                args.OriginalSource as DependencyObject) != null)
                        {
                            return;
                        }
                    }
                    if (args.RightButton == MouseButtonState.Pressed) return;
                    SetDefaultParameter(new object[] { sender, args });
                    ExecuteCommand();
                    args.Handled = true;
                };
            }
        }
    }

    public class MouseDoubleClick : EventBase<MouseDoubleClickBehavior>
    {
    }

    #endregion

    #region Preview Mouse Double Click

    public class PreviewMouseDoubleClickBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public PreviewMouseDoubleClickBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            if (selectableObject is Control)
            {
                (selectableObject as Control).PreviewMouseDoubleClick += (sender, args) => {
                    if (sender is DataGrid && args.OriginalSource != null && args.OriginalSource is DependencyObject)
                    {
                        try
                        {
                            if (VisualHelper.FindChild<DataGridColumnHeader>(args.OriginalSource as DependencyObject) != null)
                            {
                                return;
                            }
                        }
                        catch (Exception)
                        {
                            return;
                        }
                    }
                    if (args.RightButton == MouseButtonState.Pressed) return;
                    SetDefaultParameter(new object[] { sender, args });
                    ExecuteCommand();
                    args.Handled = true;
                };
            }
        }
    }

    public class PreviewMouseDoubleClick : EventBase<PreviewMouseDoubleClickBehavior>
    {
    }

    #endregion

    #region MouseLeftButtonClickTwice

    public class MouseLeftButtonClickTwiceBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public MouseLeftButtonClickTwiceBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            if (selectableObject is Control)
            {
                (selectableObject as Control).MouseLeftButtonDown += (sender, args) => {
                    if (args.ClickCount < 2) return;
                    SetDefaultParameter(new object[] { sender, args });
                    ExecuteCommand();
                };
            }
            else if (selectableObject is Border)
            {
                (selectableObject as Border).MouseLeftButtonDown += (sender, args) => {
                    if (args.ClickCount < 2) return;
                    SetDefaultParameter(new object[] { sender, args });
                    ExecuteCommand();
                };
            }
        }
    }

    public class MouseLeftButtonClickTwice : EventBase<MouseLeftButtonClickTwiceBehavior>
    {
    }

    #endregion

    #region PreviewMouseLeftButtonClickTwice

    public class PreviewMouseLeftButtonClickTwiceBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public PreviewMouseLeftButtonClickTwiceBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            if (selectableObject is Control)
            {
                (selectableObject as Control).PreviewMouseLeftButtonDown += (sender, args) => {
                    if (args.ClickCount < 2) return;
                    SetDefaultParameter(new object[] { sender, args });
                    ExecuteCommand();
                };
            }
        }
    }

    public class PreviewMouseLeftButtonClickTwice : EventBase<PreviewMouseLeftButtonClickTwiceBehavior>
    {
    }

    #endregion

    #region MouseLeftButtonDown

    public class MouseLeftButtonDownBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public MouseLeftButtonDownBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            selectableObject.MouseLeftButtonDown += (sender, args) =>
            {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class MouseLeftButtonDown : EventBase<MouseLeftButtonDownBehavior>
    {
    }

    #endregion

    #region PreviewMouseLeftButtonDown

    public class PreviewMouseLeftButtonDownBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public PreviewMouseLeftButtonDownBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            selectableObject.PreviewMouseLeftButtonDown += (sender, args) => {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class PreviewMouseLeftButtonDown : EventBase<PreviewMouseLeftButtonDownBehavior>
    {
    }

    #endregion

    #region MouseLeftButtonUp

    public class MouseLeftButtonUpBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public MouseLeftButtonUpBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            selectableObject.MouseLeftButtonUp += (sender, args) => {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class MouseLeftButtonUp : EventBase<MouseLeftButtonUpBehavior>
    {
    }

    #endregion

    #region MouseRightButtonDown

    public class MouseRightButtonDownBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public MouseRightButtonDownBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            selectableObject.MouseRightButtonDown += (sender, args) => {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class MouseRightButtonDown : EventBase<MouseRightButtonDownBehavior>
    {
    }

    #endregion

    #region MouseRightButtonUp

    public class MouseRightButtonUpBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public MouseRightButtonUpBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            selectableObject.MouseRightButtonUp += (sender, args) => {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class MouseRightButtonUp : EventBase<MouseRightButtonUpBehavior>
    {
    }

    #endregion

    #region MouseWheel

    public class MouseWheelBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public MouseWheelBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            selectableObject.PreviewMouseWheel += (sender, args) => {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class MouseWheel : EventBase<MouseWheelBehavior>
    {
    }

    #endregion

    #region SelectionChanged

    public class SelectionChangedBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public SelectionChangedBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            if (selectableObject is ComboBox)
            {
                (selectableObject as ComboBox).SelectionChanged += (sender, args) => {
                    SetDefaultParameter(new object[] { sender, args });
                    ExecuteCommand();
                };
            }
        }
    }

    public class SelectionChanged : EventBase<SelectionChangedBehavior>
    {
    }

    #endregion

    #region Keyboard Enter Input

    public class EnterInputBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public EnterInputBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {

            selectableObject.KeyDown += (sender, args) =>
            {
                if (args.Key == Key.Enter)
                {
                    SetDefaultParameter(new object[] { sender, args });
                    ExecuteCommand();
                }
            };
        }
    }

    public class EnterInput : EventBase<EnterInputBehavior>
    {
    }

    #endregion

    #region LostFocus

    public class LostFocusBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public LostFocusBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {

            selectableObject.LostFocus += (sender, args) => {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class LostFocus : EventBase<LostFocusBehavior>
    {
    }

    #endregion

    #region TextBox TextChanged

    public class TextChangedBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public TextChangedBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {

            TextBox textbox = selectableObject as TextBox;
            if (textbox == null)
                return;

            textbox.TextChanged += (sender, args) =>
            {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class TextChanged : EventBase<TextChangedBehavior>
    {
    }

    #endregion

    #region DatePicker CalendarOpened

    public class CalendarOpenedBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public CalendarOpenedBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            DatePicker datePicker = selectableObject as DatePicker;
            if (datePicker == null)
                return;

            datePicker.CalendarOpened += (sender, args) => {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class CalendarOpened : EventBase<CalendarOpenedBehavior>
    {
    }

    #endregion

    #region FrameworkElement GotFocus

    public class GotFocusBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public GotFocusBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            if (selectableObject == null)
                return;

            selectableObject.GotFocus += (sender, args) => {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class GotFocus : EventBase<GotFocusBehavior>
    {
    }

    #endregion

    #region FrameworkElement GotMouseCapture

    public class GotMouseCaptureBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public GotMouseCaptureBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            if (selectableObject == null)
                return;

            selectableObject.GotMouseCapture += (sender, args) => {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class GotMouseCapture : EventBase<GotMouseCaptureBehavior>
    {

    }

    #endregion

    #region FrameworkElement KeyDown

    public class KeyDownBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public KeyDownBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            if (selectableObject == null)
                return;

            selectableObject.KeyDown += (sender, args) => {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class KeyDown : EventBase<KeyDownBehavior>
    {
    }

    #endregion


    #region FrameworkElement KeyUp

    public class KeyUpBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public KeyUpBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            if (selectableObject == null)
                return;

            selectableObject.KeyUp += (sender, args) => {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class KeyUp : EventBase<KeyUpBehavior>
    {
    }

    #endregion


    #region FrameworkElement PreviewKeyDown

    public class PreviewKeyDownBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public PreviewKeyDownBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            if (selectableObject == null)
                return;

            selectableObject.PreviewKeyDown += (sender, args) => {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class PreviewKeyDown : EventBase<PreviewKeyDownBehavior>
    {
    }

    #endregion

    #region DataGrid LoadingRow

    public class DataGridLoadingRowBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public DataGridLoadingRowBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            if (selectableObject is object)
            {
                (selectableObject as DataGrid).LoadingRow += (sender, args) => {
                    SetDefaultParameter(new object[] { sender, args });
                    ExecuteCommand();
                };
            }
        }
    }

    public class LoadingRow : EventBase<DataGridLoadingRowBehavior>
    {
    }

    #endregion

    #region Mouse Down

    public class MouseDownBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public MouseDownBehavior(FrameworkElement obj)
            : base(obj)
        {
            obj.MouseDown += (sender, args) => {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class MouseDown : EventBase<MouseDownBehavior>
    {
    }

    #endregion

    #region Mouse Up

    public class MouseUpBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public MouseUpBehavior(FrameworkElement obj)
            : base(obj)
        {
            obj.MouseUp += (sender, args) => {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class MouseUp : EventBase<MouseUpBehavior>
    {
    }

    #endregion

    #region Mouse Move

    public class MouseMoveBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public MouseMoveBehavior(FrameworkElement obj)
            : base(obj)
        {
            obj.MouseMove += (sender, args) => {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class MouseMove : EventBase<MouseMoveBehavior>
    {
    }

    #endregion

    #region PreviewMouseMove

    public class PreviewMouseMoveBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public PreviewMouseMoveBehavior(FrameworkElement obj)
            : base(obj)
        {
            obj.PreviewMouseMove += (sender, args) => {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class PreviewMouseMove : EventBase<PreviewMouseMoveBehavior>
    {
    }

    #endregion

    #region Drop Event

    public class DropBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public DropBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            selectableObject.Drop += (sender, args) => {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class Drop : EventBase<DropBehavior>
    {
    }

    #endregion

    #region PreviewDrop

    public class PreviewDropBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public PreviewDropBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            selectableObject.PreviewDrop += (sender, args) => {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class PreviewDrop : EventBase<PreviewDropBehavior>
    {
    }
    #endregion

    #region FrameworkElement DragEnter

    public class DragEnterBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public DragEnterBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            selectableObject.DragEnter += (sender, args) => {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class DragEnter : EventBase<DragEnterBehavior>
    {
    }

    #endregion

    #region FrameworkElement DragLeave

    public class DragLeaveBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public DragLeaveBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            selectableObject.DragLeave += (sender, args) => {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class DragLeave : EventBase<DragLeaveBehavior>
    {
    }

    #endregion

    #region FrameworkElement DragOver

    public class DragOverBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public DragOverBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            selectableObject.DragOver += (sender, args) => {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class DragOver : EventBase<DragOverBehavior>
    {
    }

    #endregion

    #region SizeChanged

    public class SizeChangedBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public SizeChangedBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            selectableObject.SizeChanged += (sender, args) => {
                SetDefaultParameter(new object[] { sender, args });
                ExecuteCommand();
            };
        }
    }

    public class SizeChanged : EventBase<SizeChangedBehavior>
    {
    }

    #endregion

    #region MenuItemClick

    public class MenuItemClickBehavior : CommandBehaviorBase<FrameworkElement>
    {
        public MenuItemClickBehavior(FrameworkElement selectableObject)
            : base(selectableObject)
        {
            if (selectableObject != null && selectableObject is MenuItem)
            {
                (selectableObject as MenuItem).PreviewMouseLeftButtonDown += (sender, args) => {
                    SetDefaultParameter(new object[] { sender, args });
                    ExecuteCommand();
                };
            }
        }
    }

    public class MenuItemClick : EventBase<MenuItemClickBehavior>
    {
    }

    #endregion
}
