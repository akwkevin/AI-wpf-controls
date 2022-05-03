using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    public interface IFilteringSupportColumn
    {
        event Action<IFilteringSupportColumn, string> FilterChanged;

        string[] GetFilteredDescribe();
        void ClearFilter();
        string GetFilterTitle();

        /// <summary>
        /// 是否从已经过滤过的数据源进行过滤
        /// </summary>
        bool FromFilteredSource
        {
            get; set;
        }
    }

    public class DGFilterPanelContainer : Grid
    {
        public enum DGFilterPanelVisibility
        {
            /// <summary>
            /// 始终可见
            /// </summary>
            Visible,

            /// <summary>
            /// 不显示，保留空间
            /// </summary>
            Hidden,

            /// <summary>
            /// 不显示，不保留空间
            /// </summary>
            Collapsed,

            /// <summary>
            /// 自动显示/隐藏
            /// </summary>
            Auto,
        }

        private readonly StackPanel stackPanelFilterButtons;
        private readonly ScrollViewer scrollViewerButtonPanel;
        private readonly Dictionary<IFilteringSupportColumn, UIElement> ColFilterEleInpanel;

        public DGFilterPanelContainer()
        {
            this.ColFilterEleInpanel = new Dictionary<IFilteringSupportColumn, UIElement>();
            this.stackPanelFilterButtons = new StackPanel() { Orientation = Orientation.Horizontal };
            this.scrollViewerButtonPanel = new ScrollViewer()
            {
                Content = this.stackPanelFilterButtons,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            };

            this.Children.Add(this.scrollViewerButtonPanel);

            this.Loaded += this.DGFilterPanelContainer_Loaded;
        }

        public DataGrid DataGridObject
        {
            get
            {
                return (DataGrid)GetValue(DataGridObjectProperty);
            }
            set
            {
                SetValue(DataGridObjectProperty, value);
            }
        }

        public static readonly DependencyProperty DataGridObjectProperty =
            DependencyProperty.Register(nameof(DataGridObject), typeof(DataGrid), typeof(DGFilterPanelContainer), new PropertyMetadata(null));

        private static void FilterPanelVisibilityCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DGFilterPanelContainer).scrollViewerButtonPanel.Visibility = (Visibility)e.NewValue;
        }

        public Style BorderStyle
        {
            get
            {
                return (Style)GetValue(BorderStyleProperty);
            }
            set
            {
                SetValue(BorderStyleProperty, value);
            }
        }

        public static readonly DependencyProperty BorderStyleProperty =
            DependencyProperty.Register(nameof(BorderStyle), typeof(Style), typeof(DGFilterPanelContainer), new PropertyMetadata(null));

        public Style TextStyle
        {
            get
            {
                return (Style)GetValue(TextStyleProperty);
            }
            set
            {
                SetValue(TextStyleProperty, value);
            }
        }

        public static readonly DependencyProperty TextStyleProperty =
            DependencyProperty.Register(nameof(TextStyle), typeof(Style), typeof(DGFilterPanelContainer), new PropertyMetadata(null));


        public Style ButtonStyle
        {
            get
            {
                return (Style)GetValue(ButtonStyleProperty);
            }
            set
            {
                SetValue(ButtonStyleProperty, value);
            }
        }

        public static readonly DependencyProperty ButtonStyleProperty =
            DependencyProperty.Register(nameof(ButtonStyle), typeof(Style), typeof(DGFilterPanelContainer), new PropertyMetadata(null));


        public DGFilterPanelVisibility FilterPanelVisibility
        {
            get
            {
                return (DGFilterPanelVisibility)GetValue(FilterPanelVisibilityProperty);
            }
            set
            {
                SetValue(FilterPanelVisibilityProperty, value);
            }
        }

        public static readonly DependencyProperty FilterPanelVisibilityProperty =
            DependencyProperty.Register(nameof(FilterPanelVisibility), typeof(DGFilterPanelVisibility), typeof(DGFilterPanelContainer), new PropertyMetadata(DGFilterPanelVisibility.Auto));


        private void DGFilterPanelContainer_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.RowDefinitions.Clear();
            this.ColumnDefinitions.Clear();

            this.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            this.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });


            Grid.SetRow(this.scrollViewerButtonPanel, 1);

            if (this.DataGridObject != null)
            {
                if (this.DataGridObject.Parent == null)
                {
                    this.Children.Add(this.DataGridObject);

                    Grid.SetRow(DataGridObject, 0);
                }
            }


            this.SetFilterHandlerEvents();
            this.SetStackPanelFilterButtonsVisibility();
        }

        private void SetFilterHandlerEvents()
        {
            if (this.DataGridObject == null)
                return;

            foreach (var col in this.DataGridObject.Columns)
            {
                var filterCol = col as IFilteringSupportColumn;
                if (filterCol != null)
                {
                    filterCol.FromFilteredSource = true;
                    filterCol.FilterChanged -= this.FilterCol_FilterChanged;
                    filterCol.FilterChanged += this.FilterCol_FilterChanged;
                }
            }
        }

        private void FilterCol_FilterChanged(IFilteringSupportColumn obj, string status)
        {
            if (obj == null) return;
            var describes = obj.GetFilteredDescribe();

            if (describes == null || describes.Length == 0 || status == "ColorAll_State")
            {
                this.RemoveFilterTitle(obj);
                this.ColFilterEleInpanel.Remove(obj);
            }
            else
            {
                UIElement element;
                if (!ColFilterEleInpanel.TryGetValue(obj, out element))
                {
                    ColFilterEleInpanel[obj] = element = CreateFilterElement(obj);
                    this.stackPanelFilterButtons.Children.Add(element);
                }
                else
                {
                    SetTitleContents(element, obj);
                }
            }

            this.SetStackPanelFilterButtonsVisibility();
        }

        private void SetStackPanelFilterButtonsVisibility()
        {
            switch (this.FilterPanelVisibility)
            {
                case DGFilterPanelVisibility.Collapsed:
                    this.stackPanelFilterButtons.Visibility = Visibility.Collapsed;
                    break;
                case DGFilterPanelVisibility.Hidden:
                    this.stackPanelFilterButtons.Visibility = Visibility.Hidden;
                    break;
                case DGFilterPanelVisibility.Visible:
                    this.stackPanelFilterButtons.Visibility = Visibility.Visible;
                    break;
                case DGFilterPanelVisibility.Auto:
                    if (this.stackPanelFilterButtons.Children.Count > 0)
                        this.stackPanelFilterButtons.Visibility = Visibility.Visible;
                    else
                        this.stackPanelFilterButtons.Visibility = Visibility.Collapsed;
                    break;
            }

        }

        private UIElement CreateFilterElement(IFilteringSupportColumn column)
        {
            var border = new Border()
            {
                Tag = column,
            };
            if (this.BorderStyle != null)
                border.Style = this.BorderStyle;

            var grid = new StackPanel() { Background = Brushes.Transparent, Orientation = Orientation.Horizontal };
            border.Child = grid;

            var textTitle = new TextBlock()
            {
                Text = string.Format("{0} [{1}]", column.GetFilterTitle(), string.Join("; ", column.GetFilteredDescribe())),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center,
            };

            if (this.TextStyle != null)
                textTitle.Style = this.TextStyle;

            grid.Children.Add(textTitle);
            grid.Tag = textTitle;

            var buttonRemoveFilter = new Button()
            {
                Visibility = Visibility.Collapsed,
                VerticalAlignment = VerticalAlignment.Center,
                Content = "×",
                ToolTip = "取消筛选",
                Tag = border,
                Padding = new Thickness(0),
                MinHeight= 0,
            };
            if (this.ButtonStyle != null)
                buttonRemoveFilter.Style = this.ButtonStyle;
            buttonRemoveFilter.Click += this.ButtonRemoveFilter_Click;

            grid.Children.Add(buttonRemoveFilter);

            border.MouseEnter += this.Border_MouseEnter;
            border.MouseLeave += this.Border_MouseLeave;

            return border;
        }

        private void SetTitleContents(UIElement element, IFilteringSupportColumn column)
        {
            var border = element as Border;
            if (border == null) return;
            var grid = border.Child as Grid;
            if (grid == null) return;
            var text = grid.Tag as TextBlock;
            if (text == null) return;

            text.Text = string.Format("{0} [{1}]", column.GetFilterTitle(), string.Join("; ", column.GetFilteredDescribe()));
        }

        private void Border_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var border = sender as Border;
            var btns = (border.Child as Panel).Children.OfType<Button>();
            if (btns != null)
            {
                foreach (var btn in btns)
                {
                    btn.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void Border_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var border = sender as Border;
            var btns = (border.Child as Panel).Children.OfType<Button>();
            if (btns != null)
            {
                foreach (var btn in btns)
                {
                    btn.Visibility = Visibility.Visible;
                }
            }
        }

        private void ButtonRemoveFilter_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;
            var btnTag = button.Tag as Border;
            if (btnTag == null) return;
            var col = btnTag.Tag as IFilteringSupportColumn;
            if (col != null)
            {
                col.ClearFilter();
            }

            this.stackPanelFilterButtons.Children.Remove(btnTag);
            this.ColFilterEleInpanel.Remove(col);

            this.SetStackPanelFilterButtonsVisibility();
        }

        private void RemoveFilterTitle(IFilteringSupportColumn column)
        {
            foreach (var item in this.stackPanelFilterButtons.Children)
            {
                var border = item as Border;
                if (border == null) continue;

                if (border.Tag == column)
                {
                    stackPanelFilterButtons.Children.Remove(border);
                    break;
                }
            }
        }
    }
}
