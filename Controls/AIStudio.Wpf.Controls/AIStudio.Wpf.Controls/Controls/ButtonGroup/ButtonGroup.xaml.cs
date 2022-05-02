using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    public class ButtonGroup : ItemsControl
    {
        //protected override bool IsItemItsOwnContainerOverride(object item) => item is FrameworkElement;

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            nameof(Orientation), typeof(Orientation), typeof(ButtonGroup), new PropertyMetadata(default(Orientation)));

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        #region CornerRadiusProperty Border圆角
        /// <summary>
        /// Border圆角
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius), typeof(CornerRadius), typeof(ButtonGroup), new PropertyMetadata(new CornerRadius(5), OnCornerRadiusChanged));


        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ButtonGroup group)
            {
                group.ReloadChildProperty();
            }
        }
        #endregion

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            ReloadChildProperty();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            ReloadChildProperty();
        }

        private void ReloadChildProperty()
        {
            var count = Items.Count;
            if (count == 1)
            {
                var item = Items[0] as FrameworkElement;
                if (item != null)
                {
                    item.SetValue(ControlAttach.CornerRadiusProperty, CornerRadius);
                    item.SetValue(BorderThicknessProperty, BorderThickness);
                    if (BorderBrush != null)
                    {
                        item.SetValue(BorderBrushProperty, BorderBrush);
                    }

                    //var binding = new Binding();
                    //binding.Path = new PropertyPath(ControlAttach.StatusProperty);
                    //binding.Mode = BindingMode.OneWay;
                    //binding.Source = this;
                    //item.SetBinding(ControlAttach.StatusProperty, binding);
                }
            }
            else
            {
                for (var i = 0; i < count; i++)
                {
                    var item = Items[i] as FrameworkElement;
                    if (item != null)
                    {
                        if (BorderBrush != null)
                        {
                            item.SetValue(BorderBrushProperty, BorderBrush);
                        }
                        if (Orientation == Orientation.Horizontal)
                        {
                            if (i == 0)
                            {
                                item.SetValue(ControlAttach.CornerRadiusProperty, new CornerRadius(CornerRadius.TopLeft, 0, 0, CornerRadius.BottomLeft));
                                item.SetValue(BorderThicknessProperty, BorderThickness);
                                item.SetValue(MarginProperty, new Thickness());
                            }
                            else if (i == count - 1)
                            {
                                item.SetValue(ControlAttach.CornerRadiusProperty, new CornerRadius(0, CornerRadius.TopRight, CornerRadius.BottomRight, 0));
                                item.SetValue(BorderThicknessProperty, BorderThickness);
                                item.SetValue(MarginProperty, new Thickness(-1, 0, 0, 0));
                            }
                            else
                            {
                                item.SetValue(ControlAttach.CornerRadiusProperty, new CornerRadius());
                                item.SetValue(BorderThicknessProperty, BorderThickness);
                                item.SetValue(MarginProperty, new Thickness(-1, 0, 0, 0));
                            }
                        }
                        else
                        {
                            if (i == 0)
                            {
                                item.SetValue(ControlAttach.CornerRadiusProperty, new CornerRadius(CornerRadius.TopLeft, CornerRadius.TopRight, 0, 0));
                                item.SetValue(BorderThicknessProperty, BorderThickness);
                                item.SetValue(MarginProperty, new Thickness());

                            }
                            else if (i == count - 1)
                            {
                                item.SetValue(ControlAttach.CornerRadiusProperty, new CornerRadius(0, 0, CornerRadius.BottomRight, CornerRadius.BottomLeft));
                                item.SetValue(BorderThicknessProperty, BorderThickness);
                                item.SetValue(MarginProperty, new Thickness(0, -1, 0, 0));
                            }
                            else
                            {
                                item.SetValue(ControlAttach.CornerRadiusProperty, new CornerRadius());
                                item.SetValue(BorderThicknessProperty, BorderThickness);
                                item.SetValue(MarginProperty, new Thickness(0, -1, 0, 0));
                            }
                        }

                        //var binding = new Binding();
                        //binding.Path = new PropertyPath(ControlAttach.StatusProperty);
                        //binding.Mode = BindingMode.OneWay;
                        //binding.Source = this;
                        //item.SetBinding(ControlAttach.StatusProperty, binding);
                    }
                }
            }
        }
    }
}
