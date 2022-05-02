using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using AIStudio.Wpf.Panels.Helpers;

namespace AIStudio.Wpf.Panels
{
    [TemplatePart(Name = PART_DragThumb_V, Type = typeof(Thumb))]
    [TemplatePart(Name = PART_DragThumb_H, Type = typeof(Thumb))]
    public class ResizableItem : ContentControl
    {
        protected const string PART_DragThumb_V = "PART_DragThumb_V";
        protected const string PART_DragThumb_H = "PART_DragThumb_H";

        protected Thumb _dragThumb_V;
        protected Thumb _dragThumb_H;
        internal ResizableItemsControl Container
        {
            get; set;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _dragThumb_H = GetTemplateChild(PART_DragThumb_H) as Thumb;
            _dragThumb_H.DragDelta += DragThumb_H_DragDelta;
            _dragThumb_H.DragCompleted += _dragThumb_H_DragCompleted;

            _dragThumb_V = GetTemplateChild(PART_DragThumb_V) as Thumb;
            _dragThumb_V.DragDelta += DragThumb_V_DragDelta;
            _dragThumb_V.DragCompleted += _dragThumb_V_DragCompleted;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            InitWidthAndHeight();
        }

        public virtual void InitWidthAndHeight()
        {
            //先不处理了，自动布局吧。以后有需求在改。
        }

        private void _dragThumb_H_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            SetWidthChanged(_width);
        }

        private void _dragThumb_V_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            SetHeightChanged(_height);
        }

        protected virtual void SetWidthChanged(double width)
        {
            SetWidthPixChanged(width);
            SetWidthMaximizedRatio(width);
        }

        protected virtual void SetHeightChanged(double height)
        {
            SetHeightPixChanged(height);
            SetHeightMaximizedRatio(height);
        }

        protected virtual void SetWidthPixChanged(double width)
        {
            if (Container.PanelType == PanelType.TilePanel)
            {
                double widthpix = Math.Ceiling((width - this.Margin.Left - this.Margin.Right) / Container.TileWidth);
                if (!double.IsNaN(widthpix) && widthpix >= 1)
                {
                    //在属性上绑定了，需要给DataContext赋值
                    this.SetPropertyValue("WidthPix", (int)widthpix);
                }
            }
        }

        protected virtual void SetHeightPixChanged(double height)
        {
            if (Container.PanelType == PanelType.TilePanel)
            {
                double heightpix = Math.Ceiling((height - this.Margin.Top - this.Margin.Bottom) / Container.TileHeight);
                if (!double.IsNaN(heightpix) && heightpix >= 1)
                {
                    //在属性上绑定了，需要给DataContext赋值
                    this.SetPropertyValue("HeightPix", (int)heightpix);
                }
            }
        }

        protected virtual void SetWidthMaximizedRatio(double width)
        {
            if (Container.PanelType == PanelType.MaximizedTilePanel)
            {
                if (Container.MaximizedTileLocation == Location.Left)
                {
                    if (object.Equals(Container.Items[0], this.DataContext))
                    {
                        Container.MaximizedRatio = width / (Container.ActualWidth - (Container.TileMargin.Left + Container.TileMargin.Right) * 3);
                    }
                    else
                    {
                        Container.MaximizedRatio = 1 - width / (Container.ActualWidth - (Container.TileMargin.Left + Container.TileMargin.Right) * 3);
                    }
                    this.Width = double.NaN;
                }
                else if (Container.MaximizedTileLocation == Location.Right)
                {
                    if (object.Equals(Container.Items[Container.Items.Count - 1], this.DataContext))
                    {
                        Container.MaximizedRatio = width / (Container.ActualWidth - (Container.TileMargin.Left + Container.TileMargin.Right) * 3);
                    }
                    else
                    {
                        Container.MaximizedRatio = 1 - width / (Container.ActualWidth - (Container.TileMargin.Left + Container.TileMargin.Right) * 3);
                    }
                    this.Width = double.NaN;
                }
            }
        }

        protected virtual void SetHeightMaximizedRatio(double height)
        {
            if (Container.PanelType == PanelType.MaximizedTilePanel)
            {
                if (Container.MaximizedTileLocation == Location.Top)
                {
                    if (object.Equals(Container.Items[0], this.DataContext))
                    {
                        Container.MaximizedRatio = height / (Container.ActualHeight - (Container.TileMargin.Top + Container.TileMargin.Bottom) * 3);
                    }
                    else
                    {
                        Container.MaximizedRatio = 1 - height / (Container.ActualHeight - (Container.TileMargin.Top + Container.TileMargin.Bottom) * 3);
                    }
                    this.Height = double.NaN;
                }
                else if (Container.MaximizedTileLocation == Location.Bottom)
                {
                    if (object.Equals(Container.Items[Container.Items.Count - 1], this.DataContext))
                    {
                        Container.MaximizedRatio = height / (Container.ActualHeight - (Container.TileMargin.Top + Container.TileMargin.Bottom) * 3);
                    }
                    else
                    {
                        Container.MaximizedRatio = 1 - height / (Container.ActualHeight - (Container.TileMargin.Top + Container.TileMargin.Bottom) * 3);
                    }
                    this.Height = double.NaN;
                }
            }
        }

        private double _width;
        private double _height;

        protected virtual void DragThumb_H_DragDelta(object sender, DragDeltaEventArgs e)
        {
            this.Width = this.ActualWidth;
            if (this.Width + e.HorizontalChange > this.MinWidth)
            {
                this.Width += e.HorizontalChange;
            }
            _width = this.Width;
        }

        protected virtual void DragThumb_V_DragDelta(object sender, DragDeltaEventArgs e)
        {
            this.Height = this.ActualHeight;
            if (this.Height + e.VerticalChange > this.MinHeight)
            {
                this.Height += e.VerticalChange;
            }
            _height = this.Height;
        }
    }
}
