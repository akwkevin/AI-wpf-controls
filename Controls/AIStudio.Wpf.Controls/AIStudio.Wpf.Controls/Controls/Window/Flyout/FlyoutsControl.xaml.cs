using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Helper;
namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// A FlyoutsControl is for displaying flyouts in a MetroWindow.
    /// <see cref="MetroWindow"/>
    /// </summary>
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(Flyout))]
    public class FlyoutsControl : ItemsControl
    {
        public static readonly DependencyProperty OverrideExternalCloseButtonProperty = DependencyProperty.Register("OverrideExternalCloseButton", typeof(MouseButton?), typeof(FlyoutsControl), new PropertyMetadata(null));
        public static readonly DependencyProperty OverrideIsPinnedProperty = DependencyProperty.Register("OverrideIsPinned", typeof(bool), typeof(FlyoutsControl), new PropertyMetadata(false));

        /// <summary>
        /// Gets/sets whether <see cref="AIStudio.Wpf.Controls.Flyout.ExternalCloseButton"/> is ignored and all flyouts behave as if it was set to the value of this property.
        /// </summary>
        public MouseButton? OverrideExternalCloseButton
        {
            get
            {
                return (MouseButton?)GetValue(OverrideExternalCloseButtonProperty);
            }
            set
            {
                SetValue(OverrideExternalCloseButtonProperty, value);
            }
        }

        /// <summary>
        /// Gets/sets whether <see cref="AIStudio.Wpf.Controls.Flyout.IsPinned"/> is ignored and all flyouts behave as if it was set false.
        /// </summary>
        public bool OverrideIsPinned
        {
            get
            {
                return (bool)GetValue(OverrideIsPinnedProperty);
            }
            set
            {
                SetValue(OverrideIsPinnedProperty, value);
            }
        }

        public FlyoutsControl()
        {

        }

        static FlyoutsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(FlyoutsControl), new FrameworkPropertyMetadata(typeof(FlyoutsControl)));
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new Flyout();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is Flyout;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            this.AttachHandlers((Flyout)element);
        }

        private void AttachHandlers(Flyout flyout)
        {
            flyout.IsOpenChanged += Flyout_IsOpenChanged;
        }

        private void Flyout_IsOpenChanged(object sender, RoutedEventArgs e)
        {
            var flyout = this.GetFlyout(sender); //Get the flyout that raised the handler.

            this.HandleFlyoutStatusChange(flyout, this.TryFindParent<WindowBase>());
        }


        internal void HandleFlyoutStatusChange(Flyout flyout, WindowBase parentWindow)
        {
            if (flyout == null || parentWindow == null)
            {
                return;
            }

            this.ReorderZIndices(flyout);

            var visibleFlyouts = this.GetFlyouts(this.Items).Where(i => i.IsOpen).OrderBy(Panel.GetZIndex);
            parentWindow.HandleFlyoutStatusChange(flyout, visibleFlyouts);
        }

        private Flyout GetFlyout(object item)
        {
            var flyout = item as Flyout;
            if (flyout != null)
            {
                return flyout;
            }

            return (Flyout)this.ItemContainerGenerator.ContainerFromItem(item);
        }

        internal IEnumerable<Flyout> GetFlyouts()
        {
            return GetFlyouts(this.Items);
        }

        private IEnumerable<Flyout> GetFlyouts(IEnumerable items)
        {
            return from object item in items select this.GetFlyout(item);
        }

        private void ReorderZIndices(Flyout lastChanged)
        {
            var openFlyouts = this.GetFlyouts(this.Items).Where(i => i.IsOpen && i != lastChanged).OrderBy(Panel.GetZIndex);
            var index = 0;
            foreach (var openFlyout in openFlyouts)
            {
                Panel.SetZIndex(openFlyout, index);
                index++;
            }

            if (lastChanged.IsOpen)
            {
                Panel.SetZIndex(lastChanged, index);
            }
        }
    }
}
