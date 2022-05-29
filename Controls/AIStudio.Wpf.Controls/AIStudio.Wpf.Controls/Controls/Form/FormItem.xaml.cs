using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    public class FormItem : HeaderedContentControl
    {
        static FormItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FormItem), new FrameworkPropertyMetadata(typeof(FormItem), FrameworkPropertyMetadataOptions.Inherits));
        }

        public FormItem()
        {
        }

        //-------------------------------------------------------------------
        //
        //  Public Properties
        //
        //-------------------------------------------------------------------

        #region Public Properties

        /// <summary>
        ///     Indicates whether this FormItem is selected.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
                Selector.IsSelectedProperty.AddOwner(typeof(FormItem),
                        new FrameworkPropertyMetadata(false,
                                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
                                new PropertyChangedCallback(OnIsSelectedChanged)));

        /// <summary>
        ///     Indicates whether this FormItem is selected.
        /// </summary>
        [Bindable(true), Category("Appearance")]
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FormItem listItem = d as FormItem;
            bool isSelected = (bool)e.NewValue;

            if (isSelected)
            {
                listItem.OnSelected(new RoutedEventArgs(Selector.SelectedEvent, listItem));
            }
            else
            {
                listItem.OnUnselected(new RoutedEventArgs(Selector.UnselectedEvent, listItem));
            }
        }

        /// <summary>
        ///     Event indicating that the IsSelected property is now true.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected virtual void OnSelected(RoutedEventArgs e)
        {
            HandleIsSelectedChanged(true, e);
        }

        /// <summary>
        ///     Event indicating that the IsSelected property is now false.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected virtual void OnUnselected(RoutedEventArgs e)
        {
            HandleIsSelectedChanged(false, e);
        }

        private void HandleIsSelectedChanged(bool newValue, RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        #endregion

        #region Protected Methods

        //internal void ChangeVisualState(bool useTransitions)
        //{
        //    // Change to the correct state in the Interaction group
        //    if (!IsEnabled)
        //    {
        //        // [copied from SL code]
        //        // If our child is a control then we depend on it displaying a proper "disabled" state.  If it is not a control
        //        // (ie TextBlock, Border, etc) then we will use our visuals to show a disabled state.
        //        VisualStateManager.GoToState(this, Content is Control ? VisualStates.StateNormal : VisualStates.StateDisabled, useTransitions);
        //    }
        //    else if (IsMouseOver)
        //    {
        //        VisualStateManager.GoToState(this, VisualStates.StateMouseOver, useTransitions);
        //    }
        //    else
        //    {
        //        VisualStateManager.GoToState(this, VisualStates.StateNormal, useTransitions);
        //    }

        //    // Change to the correct state in the Selection group
        //    if (IsSelected)
        //    {
        //        if (Selector.GetIsSelectionActive(this))
        //        {
        //            VisualStateManager.GoToState(this, VisualStates.StateSelected, useTransitions);
        //        }
        //        else
        //        {
        //            VisualStates.GoToState(this, useTransitions, VisualStates.StateSelectedInactive, VisualStates.StateSelected);
        //        }
        //    }
        //    else
        //    {
        //        VisualStateManager.GoToState(this, VisualStates.StateUnselected, useTransitions);
        //    }

        //    if (IsKeyboardFocused)
        //    {
        //        VisualStateManager.GoToState(this, VisualStates.StateFocused, useTransitions);
        //    }
        //    else
        //    {
        //        VisualStateManager.GoToState(this, VisualStates.StateUnfocused, useTransitions);
        //    }
        //}

        /// <summary>
        ///     This is the method that responds to the MouseButtonEvent event.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                // turn this into a Selector.ItemClicked thing?
                //e.Handled = true;
                HandleMouseButtonDown(MouseButton.Left);
            }

            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        ///     This is the method that responds to the MouseButtonEvent event.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                // turn this into a Selector.ItemClicked thing?
                //e.Handled = true;
                HandleMouseButtonDown(MouseButton.Right);
            }
            base.OnMouseRightButtonDown(e);
        }

        private void HandleMouseButtonDown(MouseButton mouseButton)
        {
            if (Focus())
            {
                Form parent = ParentForm;
                if (parent != null)
                {
                    parent.NotifyListItemClicked(this, mouseButton);
                }
            }
        }

        /// <summary>
        /// Called when the visual parent of this element changes.
        /// </summary>
        /// <param name="oldParent"></param>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            ItemsControl oldItemsControl = null;

            if (VisualTreeHelper.GetParent(this) == null)
            {
                if (IsKeyboardFocusWithin)
                {
                    // This FormItem had focus but was removed from the tree.
                    // The normal behavior is for focus to become null, but we would rather that
                    // focus go to the parent ListBox.

                    // Use the oldParent to get a reference to the ListBox that this FormItem used to be in.
                    // The oldParent's ItemsOwner should be the ListBox.
                    oldItemsControl = ItemsControl.GetItemsOwner(oldParent);
                }
            }

            base.OnVisualParentChanged(oldParent);

            // If earlier, we decided to set focus to the old parent ListBox, do it here
            // after calling base so that the state for IsKeyboardFocusWithin is updated correctly.
            if (oldItemsControl != null)
            {
                oldItemsControl.Focus();
            }
        }


        #endregion

        //-------------------------------------------------------------------
        //
        //  Implementation
        //
        //-------------------------------------------------------------------

        #region Implementation

        internal Form ParentForm
        {
            get
            {
                return ParentSelector as Form;
            }
        }

        internal Selector ParentSelector
        {
            get
            {
                return ItemsControl.ItemsControlFromItemContainer(this) as Selector;
            }
        }

        #endregion
    }
}
