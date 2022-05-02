#region License Revision: 0 Last Revised: 3/29/2006 8:21 AM
/******************************************************************************
Copyright (c) Microsoft Corporation.  All rights reserved.


This file is licensed under the Microsoft Public License (Ms-PL). A copy of the Ms-PL should accompany this file. 
If it does not, you can obtain a copy from: 

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx
******************************************************************************/
#endregion // License
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// A panel that can display only a single item. If the panel is associated with a selector, 
    /// only the selected item is displayed. When the active or selected item is replaced, a 
    /// transition occurs from the old item to the new one.
    /// </summary>
    internal class SlideshowPanel : VirtualizingPanel
    {
        #region Member Variables
        /************************************************
		 * Member Variables
		 ***********************************************/
        private int currentIndex = -1;
        private bool panelAdded;
        private Selector selector;
        private Slideshow slideShow;
        private TransitionElement transitionElement;

        // TODO: Need collection of transitions as a DependencyProperty.
        #endregion // Member Variables

        #region Constructors
        /************************************************
		 * Constructors
		 ***********************************************/
        /// <summary>
        /// Initializes a new <see cref="SingleItemPanel"/>.
        /// </summary>
        public SlideshowPanel()
        {
            Initialize();
        }
        #endregion // Constructors

        #region Internal Methods
        /************************************************
		 * Internal Methods
		 ***********************************************/
        /// <summary>
        /// Handles the selector changing.
        /// </summary>
        private void HandleSelectorChange()
        {
            // If a previous selector exists, unsubscribe.
            if (selector != null)
            {
                selector.SelectionChanged -= new SelectionChangedEventHandler(selector_SelectionChanged);
            }

            // Try to get new selector from parent
            ItemsControl ic = ItemsControl.GetItemsOwner(this) as Selector;
            bool isHost = this.IsItemsHost;
            ItemsPresenter presenter = TemplatedParent as ItemsPresenter;
            selector = ItemsControl.GetItemsOwner(this) as Selector;

            // The selector should probably only be a Slideshow, but to keep the logic
            // separate we'll have a second variable.
            slideShow = selector as Slideshow;

            // If we have a new selector, subscribe to events
            if (selector != null)
            {
                selector.SelectionChanged += new SelectionChangedEventHandler(selector_SelectionChanged);
            }
        }

        /// <summary>
        /// Ensures that the transition panel has been created and added to the controls
        /// child collection.
        /// </summary>
        private void EnsureTransitionPanel()
        {
            // Add it as a visual child
            if (!panelAdded)
            {
                try
                {
                    AddInternalChild(transitionElement);
                    panelAdded = true;
                }
                catch (Exception ex)
                {
                    string g = ex.Message;
                }
            }
        }

        /// <summary>
        /// Initializes the control and child controls.
        /// </summary>
        private void Initialize()
        {
            // Access internal children, which forces instantiation of the generator
            UIElementCollection children = base.InternalChildren;

            // We are an items host
            IsItemsHost = true; // TODO: Should do based on parent container or selector?

            // Create the transition element
            transitionElement = new TransitionElement();
        }
        #endregion // Internal Methods

        #region Overrides / Event Handlers
        /************************************************
		 * Overrides / Event Handlers
		 ***********************************************/
        protected override Size ArrangeOverride(Size finalSize)
        {
            /*
            // We only have one child to arrange but it may not be created yet
            if (transitionElement != null)
            {
                transitionElement.Arrange(new Rect(new Point(), finalSize));
            }
             * */

            foreach (UIElement e in Children)
            {
                // TODO: Should we use e.DesiredSize instead and center it?
                e.Arrange(new Rect(new Point(0, 0), finalSize));
            }

            // Return the final size, which is the recommended size passed in
            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size resultSize = new Size(0, 0);

            foreach (UIElement child in Children)
            {
                child.Measure(availableSize);
                resultSize.Width = Math.Max(resultSize.Width, child.DesiredSize.Width);
                resultSize.Height = Math.Max(resultSize.Height, child.DesiredSize.Height);
            }

            resultSize.Width = double.IsPositiveInfinity(availableSize.Width) ? resultSize.Width : availableSize.Width;
            resultSize.Height = double.IsPositiveInfinity(availableSize.Height) ? resultSize.Height : availableSize.Height;

            return resultSize;
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            // Only allow our special child control
            if (visualAdded != transitionElement)
            {
                // throw new NotSupportedException("This child contents of this control cannot be directly manipulated.");
            }
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            HandleSelectorChange();
        }

        private void selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If there is a new item, realize it and start a transition.
            // If there is an old item, see if it's realized. If it is, unrealize it.

            // The transition panel cannot be created any earlier or data binding 
            // may not behave as expected. We must ensure it is created here so that 
            // we can use it to transition content.
            EnsureTransitionPanel();

            // We must access the InternalChildren collection every time 
            // before we can access the generator. This may be a bug in 
            // the framework.
            UIElementCollection children = base.InternalChildren;

            // Get the generator
            IItemContainerGenerator generator = ItemContainerGenerator;

            // If there is no generator we can't realize or virtualize so just bail
            if (generator == null) { return; }

            // If old item exists, mark it for virtualization
            if (currentIndex > -1)
            {
                GeneratorPosition currentPosition = generator.GeneratorPositionFromIndex(currentIndex);
                generator.Remove(currentPosition, 1);
                currentIndex = -1;
            }

            // Get the newly selected item index
            currentIndex = selector.SelectedIndex;

            // Only try to add new content if we have a selection
            if (currentIndex > -1)
            {
                // Get the generator position for the index
                GeneratorPosition newPosition = generator.GeneratorPositionFromIndex(currentIndex);

                // Realize the new object
                DependencyObject newVisual = null;
                using (generator.StartAt(newPosition, GeneratorDirection.Forward))
                {
                    newVisual = generator.GenerateNext();
                }

                // Tell the selector to use the current list of transitions and duration
                if (slideShow != null)
                {
                    transitionElement.Transition = slideShow.Transition;
                    transitionElement.TransitionSelector = slideShow.TransitionSelector;
                }
                else
                {
                    transitionElement.Transition = null;
                    transitionElement.TransitionSelector = null;
                }

                // Set content into the transition element
                transitionElement.Content = newVisual;

                // Prepare the item for its container
                // Must be called after the element has been added to the visual tree, 
                // so that resource references and inherited properties work correctly.
                generator.PrepareItemContainer(newVisual);
            }

            // Changing the selected item may change the desired size so we 
            // must re-measure
            InvalidateMeasure();
            InvalidateArrange();
        }
        #endregion // Overrides / Event Triggers
    }
}
