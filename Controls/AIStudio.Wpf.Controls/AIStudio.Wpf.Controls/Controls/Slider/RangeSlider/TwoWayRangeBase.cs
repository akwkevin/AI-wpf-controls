﻿// Adapted from https://referencesource.microsoft.com/#PresentationFramework/src/Framework/System/Windows/Controls/Primitives/RangeBase.cs

using System.Windows;
using System.Windows.Controls;
using AIStudio.Wpf.Controls.Helper;

namespace AIStudio.Wpf.Controls
{
    public class TwoWayRangeBase : Control
    {
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
            "Minimum", typeof(double), typeof(TwoWayRangeBase),
            new PropertyMetadata(0d, OnMinimumChanged), ValidateHelper.IsInRangeOfDouble);

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (TwoWayRangeBase)d;

            ctrl.CoerceValue(MaximumProperty);
            ctrl.CoerceValue(ValueStartProperty);
            ctrl.CoerceValue(ValueEndProperty);
            ctrl.OnMinimumChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
        }

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            "Maximum", typeof(double), typeof(TwoWayRangeBase),
            new PropertyMetadata(0d, OnMaximumChanged, CoerceMaximum),
            ValidateHelper.IsInRangeOfDouble);

        private static object CoerceMaximum(DependencyObject d, object basevalue)
        {
            var ctrl = (TwoWayRangeBase)d;
            var min = ctrl.Minimum;
            if ((double)basevalue < min)
            {
                return min;
            }
            return basevalue;
        }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (TwoWayRangeBase)d;

            ctrl.CoerceValue(ValueStartProperty);
            ctrl.CoerceValue(ValueEndProperty);
            ctrl.OnMaximumChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
        }

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public static readonly DependencyProperty ValueStartProperty = DependencyProperty.Register(
            "ValueStart", typeof(double), typeof(TwoWayRangeBase),
            new FrameworkPropertyMetadata(0d,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
                OnValueStartChanged, ConstrainToRange), ValidateHelper.IsInRangeOfDouble);

        private static void OnValueStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (TwoWayRangeBase)d;
            ctrl.OnValueChanged(new TwoWayRange
            {
                ValueStart = (double)e.OldValue,
                ValueEnd = ctrl.ValueEnd
            }, new TwoWayRange
            {
                ValueStart = (double)e.NewValue,
                ValueEnd = ctrl.ValueEnd
            });
        }

        protected virtual void OnValueChanged(TwoWayRange oldValue, TwoWayRange newValue) => RaiseEvent(
            new RoutedPropertyChangedEventArgs<TwoWayRange>(oldValue, newValue) { RoutedEvent = ValueChangedEvent });

        public double ValueStart
        {
            get => (double)GetValue(ValueStartProperty);
            set => SetValue(ValueStartProperty, value);
        }

        public static readonly DependencyProperty ValueEndProperty = DependencyProperty.Register(
            "ValueEnd", typeof(double), typeof(TwoWayRangeBase),
            new FrameworkPropertyMetadata(0d,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
                OnValueEndChanged, ConstrainToRange), ValidateHelper.IsInRangeOfDouble);

        private static void OnValueEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (TwoWayRangeBase)d;
            ctrl.OnValueChanged(new TwoWayRange
            {
                ValueStart = ctrl.ValueStart,
                ValueEnd = (double)e.OldValue
            }, new TwoWayRange
            {
                ValueStart = ctrl.ValueStart,
                ValueEnd = (double)e.NewValue
            });
        }

        public double ValueEnd
        {
            get => (double)GetValue(ValueEndProperty);
            set => SetValue(ValueEndProperty, value);
        }

        internal static object ConstrainToRange(DependencyObject d, object value)
        {
            var ctrl = (TwoWayRangeBase)d;
            var min = ctrl.Minimum;
            var v = (double)value;
            if (v < min)
            {
                return min;
            }

            var max = ctrl.Maximum;
            if (v > max)
            {
                return max;
            }

            return value;
        }

        public static readonly DependencyProperty LargeChangeProperty = DependencyProperty.Register(
            "LargeChange", typeof(double), typeof(TwoWayRangeBase), new PropertyMetadata(1d),
            ValidateHelper.IsInRangeOfPosDoubleIncludeZero);

        public double LargeChange
        {
            get => (double)GetValue(LargeChangeProperty);
            set => SetValue(LargeChangeProperty, value);
        }

        public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register(
            "SmallChange", typeof(double), typeof(TwoWayRangeBase), new PropertyMetadata(0.1d),
            ValidateHelper.IsInRangeOfPosDoubleIncludeZero);

        public double SmallChange
        {
            get => (double)GetValue(SmallChangeProperty);
            set => SetValue(SmallChangeProperty, value);
        }

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<TwoWayRange>), typeof(TwoWayRangeBase));

        public event RoutedPropertyChangedEventHandler<TwoWayRange> ValueChanged
        {
            add => AddHandler(ValueChangedEvent, value);
            remove => RemoveHandler(ValueChangedEvent, value);
        }
    }

    public struct TwoWayRange
    {
        public double ValueStart
        {
            get; set;
        }

        public double ValueEnd
        {
            get; set;
        }
    }
}