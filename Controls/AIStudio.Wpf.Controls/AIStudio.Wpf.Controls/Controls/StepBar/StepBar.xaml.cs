﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using AIStudio.Wpf.Controls.Event;
using AIStudio.Wpf.Controls.Helper;

namespace AIStudio.Wpf.Controls
{
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(StepBarItem))]
    [DefaultEvent("StepChanged")]
    [TemplatePart(Name = ElementProgressBarBack, Type = typeof(ProgressBar))]
    public class StepBar : ItemsControl
    {
        private ProgressBar _progressBarBack;

        #region Constants

        private const string ElementProgressBarBack = "PART_ProgressBarBack";

        #endregion Constants

        public StepBar()
        {
            ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                var count = Items.Count;
                if (count <= 0) return;

                for (var i = 0; i < count; i++)
                {
                    if (ItemContainerGenerator.ContainerFromIndex(i) is StepBarItem stepBarItem)
                    {
                        stepBarItem.Index = i + 1;
                    }
                }

                if (!UseItemStatus)
                {
                    if (StepIndex < count)
                    {
                        for (var i = 0; i < StepIndex; i++)
                        {
                            if (ItemContainerGenerator.ContainerFromIndex(i) is StepBarItem stepBarItem)
                            {
                                stepBarItem.Status = StepStatus.Complete;
                            }
                        }
                        if (ItemContainerGenerator.ContainerFromIndex(StepIndex) is StepBarItem item)
                        {
                            item.Status = StepStatus.UnderWay;
                        }
                    }

                }
            }
        }

        protected override bool IsItemItsOwnContainerOverride(object item) => item is StepBarItem;

        protected override DependencyObject GetContainerForItemOverride() => new StepBarItem();

        /// <summary>
        ///     步骤改变事件
        /// </summary>
        public static readonly RoutedEvent StepChangedEvent =
            EventManager.RegisterRoutedEvent("StepChanged", RoutingStrategy.Bubble,
                typeof(EventHandler<FunctionEventArgs<int>>), typeof(StepBar));

        /// <summary>
        ///     步骤改变事件
        /// </summary>
        [Category("Behavior")]
        public event EventHandler<FunctionEventArgs<int>> StepChanged
        {
            add => AddHandler(StepChangedEvent, value);
            remove => RemoveHandler(StepChangedEvent, value);
        }

        public static readonly DependencyProperty StepIndexProperty = DependencyProperty.Register(
            "StepIndex", typeof(int), typeof(StepBar), new PropertyMetadata(0));

        public int StepIndex
        {
            get => (int)GetValue(StepIndexProperty);
            protected set => SetValue(StepIndexProperty, value);
        }

        public static readonly DependencyProperty UseItemStatusProperty = DependencyProperty.Register(
          "UseItemStatus", typeof(bool), typeof(StepBar), new PropertyMetadata(false));

        public bool UseItemStatus
        {
            get => (bool)GetValue(UseItemStatusProperty);
            set => SetValue(UseItemStatusProperty, value);
        }

        public static readonly DependencyProperty DockProperty = DependencyProperty.Register(
            "Dock", typeof(Dock), typeof(StepBar), new PropertyMetadata(Dock.Top));

        public Dock Dock
        {
            get => (Dock)GetValue(DockProperty);
            set => SetValue(DockProperty, value);
        }

        public static readonly DependencyProperty IsDotProperty =
            DependencyProperty.Register("IsDot", typeof(bool), typeof(StepBar), new PropertyMetadata(false));

        public bool IsDot
        {
            get => (bool)GetValue(IsDotProperty);
            set => SetValue(IsDotProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _progressBarBack = GetTemplateChild(ElementProgressBarBack) as ProgressBar;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var colCount = Items.Count;
            if (_progressBarBack == null || colCount <= 0) return;
            _progressBarBack.Maximum = colCount - 1;
            _progressBarBack.Value = StepIndex;

            if (Dock == Dock.Top || Dock == Dock.Bottom)
            {
                _progressBarBack.Width = (colCount - 1) * (ActualWidth / colCount);
            }
            else
            {
                _progressBarBack.Height = (colCount - 1) * (ActualHeight / colCount);
            }
        }


        public void Next()
        {
            StepIndex++;
            if (StepIndex >= Items.Count)
            {
                StepIndex = Items.Count - 1;
                return;
            }
            RaiseEvent(new FunctionEventArgs<int>(StepChangedEvent, this)
            {
                Info = StepIndex
            });
            if (ItemContainerGenerator.ContainerFromIndex(StepIndex - 1) is StepBarItem stepItemFinished)
                stepItemFinished.Status = StepStatus.Complete;
            if (ItemContainerGenerator.ContainerFromIndex(StepIndex) is StepBarItem stepItemSelected)
                stepItemSelected.Status = StepStatus.UnderWay;
            _progressBarBack?.BeginAnimation(RangeBase.ValueProperty, AnimationHelper.CreateAnimation(StepIndex));
        }

        public void Prev()
        {
            StepIndex--;
            if (StepIndex < 0)
            {
                StepIndex = 0;
                return;
            }
            RaiseEvent(new FunctionEventArgs<int>(StepChangedEvent, this)
            {
                Info = StepIndex
            });
            if (ItemContainerGenerator.ContainerFromIndex(StepIndex + 1) is StepBarItem stepItemWaiting)
                stepItemWaiting.Status = StepStatus.Waiting;
            if (ItemContainerGenerator.ContainerFromIndex(StepIndex) is StepBarItem stepItemSelected)
                stepItemSelected.Status = StepStatus.UnderWay;
            _progressBarBack?.BeginAnimation(RangeBase.ValueProperty, AnimationHelper.CreateAnimation(StepIndex));
        }

        public void TrySetStepIndex(StepBarItem item)
        {
            //int maxindex = this.FindChildren<StepBarItem>().Select((p, ind) => new { Index = ind, Item = p }).Where(p => p.Item.Status == StepStatus.Complete || p.Item.Status == StepStatus.Error).Max(p => p.Index);
            //if (index > maxindex)
            {
                StepIndex = ItemContainerGenerator.IndexFromContainer(item) + 1;
                _progressBarBack?.BeginAnimation(RangeBase.ValueProperty, AnimationHelper.CreateAnimation(StepIndex));
            }
        }
    }
}