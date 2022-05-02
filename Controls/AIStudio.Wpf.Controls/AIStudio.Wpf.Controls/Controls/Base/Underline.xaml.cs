using System.Windows;
using System.Windows.Controls;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// 
    /// </summary>
    [TemplateVisualState(GroupName = "ActivationStates", Name = ActiveStateName)]
    [TemplateVisualState(GroupName = "ActivationStates", Name = InactiveStateName)]
    public partial class Underline : Control
    {
        public const string ActiveStateName = "Active";
        public const string InactiveStateName = "Inactive";

        static Underline()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Underline), new FrameworkPropertyMetadata(typeof(Underline)));
        }

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
            nameof(IsActive), typeof(bool), typeof(Underline),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, IsActivePropertyChangedCallback));

        private static void IsActivePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((Underline)dependencyObject).GotoVisualState(!TransitionAssist.GetDisableTransitions(dependencyObject));
        }

        public bool IsActive
        {
            get
            {
                return (bool)GetValue(IsActiveProperty);
            }
            set
            {
                SetValue(IsActiveProperty, value);
            }
        }

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius), typeof(CornerRadius), typeof(Underline),
            new FrameworkPropertyMetadata(new CornerRadius(0), FrameworkPropertyMetadataOptions.AffectsRender, null));

        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }
            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            GotoVisualState(false);
        }

        private void GotoVisualState(bool useTransitions)
        {
            VisualStateManager.GoToState(this, SelectStateName(), useTransitions);
        }

        private string SelectStateName()
        {
            return IsActive ? ActiveStateName : InactiveStateName;
        }
    }

    /// <summary>
    /// Allows transitions to be disabled where supported.
    /// </summary>
    public static class TransitionAssist
    {
        /// <summary>
        /// Allows transitions to be disabled where supported.  Note this is an inheritable property.
        /// </summary>
        public static readonly DependencyProperty DisableTransitionsProperty = DependencyProperty.RegisterAttached(
            "DisableTransitions", typeof(bool), typeof(TransitionAssist), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Allows transitions to be disabled where supported.  Note this is an inheritable property.
        /// </summary>
        public static void SetDisableTransitions(DependencyObject element, bool value)
        {
            element.SetValue(DisableTransitionsProperty, value);
        }

        /// <summary>
        /// Allows transitions to be disabled where supported.  Note this is an inheritable property.
        /// </summary>
        public static bool GetDisableTransitions(DependencyObject element)
        {
            return (bool)element.GetValue(DisableTransitionsProperty);
        }
    }
}
