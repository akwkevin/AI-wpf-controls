#region License Revision: 0 Last Revised: 3/29/2006 8:21 AM
/******************************************************************************
Copyright (c) Microsoft Corporation.  All rights reserved.


This file is licensed under the Microsoft Public License (Ms-PL). A copy of the Ms-PL should accompany this file. 
If it does not, you can obtain a copy from: 

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx
******************************************************************************/
#endregion // License
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// An element that can display exactly one piece of visual content. When the content is changed, a 
    /// transition is used to switch between the old and the new.
    /// </summary>
    [System.Windows.Markup.ContentProperty("Content")]
    [System.Runtime.InteropServices.ComVisible(false)]
    // QUESTION: Why derive from FrameworkElement instead of ContentControl?
    public class TransitionElement : FrameworkElement
    {
        private static Lazy<Dictionary<string, Type>> _transitionTypes = new Lazy<Dictionary<string, Type>>();

        /// <summary>
        /// Initializes the static version of <see cref="TransitionElement"/>.
        /// </summary>
        static TransitionElement()
        {
            // TraceSwitches.Transitions.Level = TraceLevel.Verbose;

            defaultNullContentTemplate = new DataTemplate();
            FrameworkElementFactory rectangle = new FrameworkElementFactory(typeof(Rectangle));
            rectangle.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            rectangle.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Stretch);
            rectangle.SetValue(Shape.FillProperty, SystemColors.WindowBrush /*new TemplateBindingExtension(Control.ForegroundProperty)?*/);
            defaultNullContentTemplate.VisualTree = rectangle;
            defaultNullContentTemplate.Seal();

            NullContentTemplateProperty.OverrideMetadata(typeof(TransitionElement), new FrameworkPropertyMetadata(defaultNullContentTemplate));

            ClipToBoundsProperty.OverrideMetadata(typeof(TransitionElement), new FrameworkPropertyMetadata(null, CoerceClipToBounds));

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                // Must not already exist
                if (_transitionTypes.Value.ContainsValue(type)) { continue; }

                // Must not be abstract.
                if ((typeof(Transition).IsAssignableFrom(type)) && (!type.IsAbstract))
                {
                    _transitionTypes.Value.Add(type.Name, type);
                }
            }
        }



        /// <summary>
        /// Initializes the <see cref="TransitionElement"/> instance.
        /// </summary>
        public TransitionElement()
        {
            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose, "TransitionElement - Constructor Entry");
            this.children = new UIElementCollection(this, null);

            hiddenGrid = new Grid();
            hiddenGrid.Visibility = Visibility.Hidden;
            children.Add(hiddenGrid);

            this._newContentPresenter = new ContentPresenter();
            this.children.Add(_newContentPresenter);

            this._oldContentPresenter = new ContentPresenter();


        }

        /// <summary>
        /// Identifies the <see cref="TransitionBeginning"/> routed event.
        /// </summary>
        public static readonly RoutedEvent TransitionBeginningEvent = EventManager.RegisterRoutedEvent(
            "TransitionBeginning",
            RoutingStrategy.Bubble, // QUESTION: is this the correct strategy?
            typeof(TransitionEventHandler),
            typeof(TransitionElement));

        /// <summary>
        /// Occurs when the curent transition is starting.
        /// </summary>
        public event TransitionEventHandler TransitionBeginning
        {
            add
            {
                AddHandler(TransitionElement.TransitionBeginningEvent, value);
            }
            remove
            {
                RemoveHandler(TransitionElement.TransitionBeginningEvent, value);
            }
        }

        /// <summary>
        /// Raises the <see cref="TransitionBeginning"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="TransitionEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnTransitionBeginning(TransitionEventArgs e)
        {
            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose, "TransitionElement - OnTransitionBeginning Entry");
            RaiseEvent(e);
        }

        /// <summary>
        /// Identifies the <see cref="TransitionEnded"/> routed event.
        /// </summary>
        public static readonly RoutedEvent TransitionEndedEvent = EventManager.RegisterRoutedEvent(
            "TransitionEnded",
            RoutingStrategy.Bubble, // QUESTION: is this the correct strategy?
            typeof(TransitionEventHandler),
            typeof(TransitionElement));

        /// <summary>
        /// Occurs when the current transition has completed.
        /// </summary>
        public event TransitionEventHandler TransitionEnded
        {
            add
            {
                AddHandler(TransitionElement.TransitionEndedEvent, value);
            }
            remove
            {
                RemoveHandler(TransitionElement.TransitionEndedEvent, value);
            }
        }

        /// <summary>
        /// Raises the <see cref="TransitionEnded"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="TransitionEventArgs"/> containing the event data.
        /// </param>
        protected virtual void OnTransitionEnded(TransitionEventArgs e)
        {
            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose, "TransitionElement - OnTransitionEnded Entry");
            RaiseEvent(e);
        }

        // Force clip to be true if the active Transition requires it
        private static object CoerceClipToBounds(object element, object value)
        {
            TransitionElement te = (TransitionElement)element;
            bool clip = (bool)value;
            if (!clip && te.IsTransitioning)
            {
                Transition transition = te.Transition;
                if (transition.ClipToBounds)
                    return true;
            }
            return value;
        }

        /// <summary>
        /// Gets or sets the content that is presented in the <see cref="TransitionElement"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// The content that is presented in the <see cref="TransitionElement"/>.
        /// </value>
        /// <remarks>
        /// If a transition is specified on the <see cref="Transition"/> property, changing the 
        /// value of this property will automatically cause the transition to begin.
        /// </remarks>
        public object Content
        {
            get
            {
                return (object)GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="Content"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content",
                typeof(object),
                typeof(TransitionElement),
                new UIPropertyMetadata(null, OnContentChanged, CoerceContent));

        // Don't update direct content until done transitioning
        private static object CoerceContent(object element, object value)
        {
            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose, "TransitionElement - CoerceContent Entry");
            TransitionElement te = element as TransitionElement;
            if (te != null && te.IsTransitioning)
            {
                Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose, "TransitionElement - CoerceContent returning te.CurrentContentPresenter.Content");
                return te.NewContentPresenter.Content;
            }
            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose, "TransitionElement - CoerceContent returning normal value");
            return value;
        }

        /// <summary>
        /// Handles a change to the Content property.
        /// </summary>
        private static void OnContentChanged(object element, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose, "TransitionElement - OnContentChanged Entry");
            TransitionElement te = element as TransitionElement;
            if (te != null)
            {
                ContentPresenter contentPresenter = te.Content as ContentPresenter;
                FrameworkElement parentFE = te.Parent as FrameworkElement;
                Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose, "TransitionElement - OnContentChanged Beginning Transition");
                te.BeginTransition();
            }
        }

        /// <summary>
        /// Gets or sets the data template used to display the content of the <see cref="TransitionElement"/>. 
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Set this property to a <see cref="DataTemplate"/> to specify the appearance of the <see cref="TransitionElement"/>. 
        /// For more information on data templates, see 
        /// <see href="http://msdn2.microsoft.com/en-us/library/ms742521.aspx">Data Templating Overview</see>.
        /// </remarks>
        public DataTemplate ContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ContentTemplateProperty);
            }
            set
            {
                SetValue(ContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="ContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate",
                typeof(DataTemplate),
                typeof(TransitionElement),
                new UIPropertyMetadata(null, OnContentTemplateChanged));

        private static void OnContentTemplateChanged(object element, DependencyPropertyChangedEventArgs e)
        {
            TransitionElement te = (TransitionElement)element;
            te.NewContentPresenter.ContentTemplate = (DataTemplate)e.NewValue;
        }

        /// <summary>
        /// Gets or sets a template selector that enables an application writer to provide custom template-selection logic. 
        /// This is a dependency property.
        /// </summary>
        public DataTemplateSelector ContentTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty);
            }
            set
            {
                SetValue(ContentTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="ContentTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateSelectorProperty =
            DependencyProperty.Register("ContentTemplateSelector",
                typeof(DataTemplateSelector),
                typeof(TransitionElement),
                new UIPropertyMetadata(null, OnContentTemplateSelectorChanged));

        private static void OnContentTemplateSelectorChanged(object element, DependencyPropertyChangedEventArgs e)
        {
            TransitionElement te = (TransitionElement)element;
            te.NewContentPresenter.ContentTemplateSelector = (DataTemplateSelector)e.NewValue;
        }

        /// <summary>
        /// Identifies the <see cref="TransitionsEnabled"/> dependency property.
        /// </summary>
        static public readonly DependencyProperty TransitionsEnabledProperty = DependencyProperty.Register("TransitionsEnabled", typeof(bool), typeof(TransitionElement), new FrameworkPropertyMetadata(true, OnTransitionsEnabledChanged));

        private static void OnTransitionsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TransitionElement)d).HandleTransitionsEnabledChanged(e);
        }

        private void HandleTransitionsEnabledChanged(DependencyPropertyChangedEventArgs e)
        {

            // Notify
            OnTransitionsEnabledChanged(e);
        }

        /// <summary>
        /// Occurs when the value of the <see cref="TransitionsEnabled"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnTransitionsEnabledChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Gets or sets a value that indicats if transitions are enabled. This is a dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if transitions are enabled; otherwise <c>false</c>. The default is <c>true</c>.
        /// </value>
        public bool TransitionsEnabled
        {
            get
            {
                return (bool)GetValue(TransitionsEnabledProperty);
            }
            set
            {
                SetValue(TransitionsEnabledProperty, value);
            }
        }


        /// <summary>
        /// Gets a value that indicates if the selected transition is currently running. This is a dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if the transition is running; otherwise <c>false</c>.
        /// </value>
        public bool IsTransitioning
        {
            get
            {
                return (bool)GetValue(IsTransitioningProperty);
            }
            private set
            {
                SetValue(IsTransitioningPropertyKey, value);
            }
        }

        private static readonly DependencyPropertyKey IsTransitioningPropertyKey =
            DependencyProperty.RegisterReadOnly("IsTransitioning",
                typeof(bool),
                typeof(TransitionElement),
                new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsTransitioning"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTransitioningProperty =
            IsTransitioningPropertyKey.DependencyProperty;


        /// <summary>
        /// Gets or sets the currently selected transition. This is a dependency property.
        /// </summary>
        /// <value>
        /// The currently selected <see cref="Transition"/>.
        /// </value>
        /// <remarks>
        /// This transition will be used to animate between old content and new content 
        /// whenever the value of the <see cref="Content"/> property has changed.
        /// </remarks>
        public Transition Transition
        {
            get
            {
                return (Transition)GetValue(TransitionProperty);
            }
            set
            {
                SetValue(TransitionProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="Transition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TransitionProperty =
            DependencyProperty.Register("Transition", typeof(Transition), typeof(TransitionElement), new UIPropertyMetadata(null, null, CoerceTransition));

        private static object CoerceTransition(object element, object value)
        {
            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose, "TransitionElement - CoerceTransition Entry");
            TransitionElement te = (TransitionElement)element;
            if (te.IsTransitioning)
            {
                Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose, "TransitionElement - CoerceTransition returning active transition");
                return te.activeTransition;
            }
            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose, "TransitionElement - CoerceTransition returning normal current transition");
            return value;
        }

        public TransitionSwitchType TransitionSwitchType
        {
            get
            {
                return (TransitionSwitchType)GetValue(TransitionSwitchTypeProperty);
            }
            set
            {
                SetValue(TransitionSwitchTypeProperty, value);
            }
        }

        public static readonly DependencyProperty TransitionSwitchTypeProperty =
            DependencyProperty.Register("TransitionSwitchType", typeof(TransitionSwitchType), typeof(TransitionElement), new UIPropertyMetadata(TransitionSwitchType.None, TransitionSwitchTypeChanged));

        private static void TransitionSwitchTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TransitionElement te = (TransitionElement)d;
            if ((TransitionSwitchType)e.NewValue == TransitionSwitchType.None)
            {
                te.Transition = null;
            }
            else
            {
                te.Transition = (Transition)Activator.CreateInstance(_transitionTypes.Value[e.NewValue.ToString()]);
            }
        }

        /// <summary>
        /// Identifies the <see cref="NullContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NullContentTemplateProperty =
            DependencyProperty.Register(
                "NullContentTemplate",
                typeof(DataTemplate),
                typeof(TransitionElement)
            );

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> that should be displayed whenever the <see cref="Content"/> property 
        /// is set to <see langword="null"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// A <see cref="DataTemplate"/> to display when no content is available; otherwise <see langword="null"/>.
        /// </value>
        /// <remarks>
        /// The value of the <see cref="TransitionToNullIgnored"/> impacts whether this template is transitioned.
        /// </remarks>
        public DataTemplate NullContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(NullContentTemplateProperty);
            }
            set
            {
                SetValue(NullContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a transition selector that enables an application writer to provide custom transition 
        /// selection logic. This is a dependency property.
        /// </summary>
        public TransitionSelector TransitionSelector
        {
            get
            {
                return (TransitionSelector)GetValue(TransitionSelectorProperty);
            }
            set
            {
                SetValue(TransitionSelectorProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="TransitionSelectorProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TransitionSelectorProperty =
            DependencyProperty.Register("TransitionSelector", typeof(TransitionSelector), typeof(TransitionElement), new UIPropertyMetadata(null));

        /// <summary>
        /// Starts the selected <see cref="Transition"/>.
        /// </summary>
        public void BeginTransition()
        {
            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose, "TransitionElement - BeginTransition Entry");

            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose, "TransitionElement - effectiveContent using direct content");
            object newContent = this.Content;

            ContentPresenter existingContentPresenter = this.NewContentPresenter;
            object oldContent = existingContentPresenter.Content;

            TransitionSelector transitionSelector = this.TransitionSelector;

            Transition transition = this.Transition;
            if (transitionSelector != null)
            {
                transition = transitionSelector.SelectTransition(oldContent, newContent);
            }

            bool transitioningToNullContent = newContent == null;
            bool transitioningFromNullContent = oldContent == null;

            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose && transition != null, "TransitionElement - BeginTransition transition is set");
            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose && transition == null, "TransitionElement - BeginTransition transition is null");

            bool shouldTransition = (transition != null) && (TransitionsEnabled) && (!SkipTransition(transition, existingContentPresenter, transitioningToNullContent, transitioningFromNullContent));

            if (shouldTransition)
            {
                Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose, "TransitionElement - BeginTransition Swapping content presenters");
                // Swap content presenters
                ContentPresenter temp = this._oldContentPresenter;
                this._oldContentPresenter = this._newContentPresenter;
                this._newContentPresenter = temp;
            }

            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose, "TransitionElement - BeginTransition Updating current content presenter's content");
            ContentPresenter newContentPresenter = this.NewContentPresenter;
            // Set the current content
            newContentPresenter.Content = newContent;
            newContentPresenter.ContentTemplate = this.ContentTemplate;
            newContentPresenter.ContentTemplateSelector = this.ContentTemplateSelector;

            if (shouldTransition)
            {
                ContentPresenter oldContentPresenter = this.OldContentPresenter;

                if (oldContent == null && this.NullContentTemplate != null)
                {
                    oldContentPresenter.ContentTemplate = this.NullContentTemplate;
                }
                if (newContent == null && this.NullContentTemplate != null)
                {
                    newContentPresenter.ContentTemplate = this.NullContentTemplate;
                }

                if (transition.IsNewContentTopmost)
                {
                    this.Children.Add(this._newContentPresenter);
                }
                else
                {
                    this.Children.Insert(0, this._newContentPresenter);
                }

                Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose, "TransitionElement - BeginTransition Setting up for transition");
                this.IsTransitioning = true;
                this.activeTransition = transition;
                CoerceValue(TransitionProperty);
                CoerceValue(ClipToBoundsProperty);
                OnTransitionBeginning(new TransitionEventArgs(TransitionBeginningEvent, this, transition, oldContent, newContent));
                Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose, "TransitionElement - BeginTransition Calling transition's BeginTransition");
                transition.BeginTransition(this, oldContentPresenter, newContentPresenter);
            }
        }

        private bool SkipTransition(Transition transition, ContentPresenter existingContentPresenter,
                                    bool transitioningToNullContent, bool transitioningFromNullContent)
        {
            Debug.Assert(transition != null);

            if (transitioningToNullContent && (transition.NullContentSupport == NullContentSupport.Old ||
                                               transition.NullContentSupport == NullContentSupport.None))
            {
                if (this.ContentTemplate == null &&
                    this.ContentTemplateSelector == null &&
                    this.NullContentTemplate == null)
                {
                    return true;
                }
            }
            if (transitioningFromNullContent && (transition.NullContentSupport == NullContentSupport.New ||
                                                 transition.NullContentSupport == NullContentSupport.None))
            {
                if (existingContentPresenter.ContentTemplate == null &&
                    existingContentPresenter.ContentTemplateSelector == null &&
                    this.NullContentTemplate == null)
                {
                    return true;
                }
            }
            return false;
        }

        public void HideContent(ContentPresenter content)
        {
            if (children.Contains(content))
            {
                children.Remove(content);
                hiddenGrid.Children.Add(content);
            }
        }

        // Clean up after the transition is complete
        internal void OnTransitionCompleted(Transition transition, object oldContent, object newContent)
        {
            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose, "TransitionElement - OnTransitionCompleted Entry");

            // The parameters passed here are what the transition animated (which is a content presenter) 
            // and not the actual content presented.
            object actualOldContent = _oldContentPresenter.Content;
            object actualNewContent = _newContentPresenter.Content;

            // If the newContentPresenter has content and the presenter is removed from the child collection, 
            // any VisualBrush targeting the presenter could cause an InvalidOperationException deep in the 
            // core of WPF (NotifyPartitionIsZombie). The easiest way to work around this WPF issue is to 
            // disassociate the presenter with it's content while changing parents in the visual tree.

            // Disassociate content from the presenter
            _newContentPresenter.Content = null;

            // Clear the child collection (transitions may have added other things like a Viewport3D)
            this.children.Clear();

            // Clear the hidden grid collection too because we'll reparent anything here on the next transition
            this.hiddenGrid.Children.Clear();

            // Add the new content presenter and the hidden grid back into the child collectoin
            children.Add(_newContentPresenter);
            children.Add(hiddenGrid);

            // Restore the content back into the presenter now that it's safely reparented.
            _newContentPresenter.Content = actualNewContent;

            // Clear out old content. It should no longer be rooted by the transition element.
            _oldContentPresenter.Content = null;

            // Done transitioning
            this.IsTransitioning = false;
            this.activeTransition = null;

            // Update measurements
            CoerceValue(TransitionProperty);
            CoerceValue(ClipToBoundsProperty);
            CoerceValue(ContentProperty);

            // Notify listeners of completion
            OnTransitionEnded(new TransitionEventArgs(TransitionEndedEvent, this, transition, actualOldContent, actualNewContent));
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            this._newContentPresenter.Measure(availableSize);
            return this._newContentPresenter.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement uie in this.children)
            {
                uie.Arrange(new Rect(finalSize));
            }
            return finalSize;
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return this.children.Count;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= children.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            return this.children[index];
        }

        internal UIElementCollection Children
        {
            get
            {
                return this.children;
            }
        }

        private ContentPresenter OldContentPresenter
        {
            get
            {
                return _oldContentPresenter;
            }
        }

        // TODO: May have to make this public because in Acropolis PartPane and TabLayoutPane used it
        private ContentPresenter NewContentPresenter
        {
            get
            {
                return _newContentPresenter;
            }
        }

        private UIElementCollection children;

        private ContentPresenter _oldContentPresenter;
        private ContentPresenter _newContentPresenter;

        private Transition activeTransition;

        private Grid hiddenGrid;

        private static DataTemplate defaultNullContentTemplate;
    }
}
