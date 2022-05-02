using System.Windows;
using System.Windows.Controls;
using AIStudio.Wpf.Controls.Helper;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    ///     步骤条单元项
    /// </summary>
    public class StepBarItem : ContentControl
    {
        /// <summary>
        ///     步骤编号
        /// </summary>
        public static readonly DependencyProperty IndexProperty = DependencyProperty.Register(
            "Index", typeof(int), typeof(StepBarItem), new PropertyMetadata(-1));

        /// <summary>
        ///     步骤编号
        /// </summary>
        public int Index
        {
            get => (int)GetValue(IndexProperty);
            internal set => SetValue(IndexProperty, value);
        }

        /// <summary>
        ///     步骤状态
        /// </summary>
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(
            "Status", typeof(StepStatus), typeof(StepBarItem), new PropertyMetadata(StepStatus.Waiting, OnStepStatusChanged));

        /// <summary>
        ///     步骤状态
        /// </summary>
        public StepStatus Status
        {
            get => (StepStatus)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        private static void OnStepStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (StepBarItem)d;
            if (instance.Status == StepStatus.Complete)
            {
                StepBar stepBar = instance.TryFindParent<StepBar>();
                if (stepBar != null)
                {
                    stepBar.TrySetStepIndex(instance);
                }
            }
        }
    }
}