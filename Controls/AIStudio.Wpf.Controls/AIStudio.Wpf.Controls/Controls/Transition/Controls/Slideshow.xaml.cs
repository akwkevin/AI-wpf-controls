#region License Revision: 0 Last Revised: 3/29/2006 8:21 AM
/******************************************************************************
Copyright (c) Microsoft Corporation.  All rights reserved.


This file is licensed under the Microsoft Public License (Ms-PL). A copy of the Ms-PL should accompany this file. 
If it does not, you can obtain a copy from: 

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx
******************************************************************************/
#endregion // License
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for Slideshow.xaml
    /// </summary>
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(SlideshowItem))]
    public partial class Slideshow : Selector
    {
        #region Static Version
        #region Constants
        /************************************************
		 * Constants
		 ***********************************************/
        static public readonly DependencyProperty AutoAdvanceProperty = DependencyProperty.Register("AutoAdvanceProperty", typeof(bool), typeof(Slideshow), new FrameworkPropertyMetadata(false, OnAutoAdvanceChanged));
        static public readonly DependencyProperty AutoAdvanceDurationProperty = DependencyProperty.Register("AutoAdvanceDurationProperty", typeof(Duration), typeof(Slideshow), new FrameworkPropertyMetadata(new Duration(TimeSpan.FromSeconds(3)), OnAutoAdvanceDurationChanged), ValidateAutoAdvanceDuration);
        static public readonly DependencyProperty IsContinuousProperty = DependencyProperty.Register("IsContinuous", typeof(bool), typeof(Slideshow), new FrameworkPropertyMetadata(true, OnIsContinuousChanged));
        static public readonly DependencyProperty ShuffleProperty = DependencyProperty.Register("Shuffle", typeof(bool), typeof(Slideshow), new FrameworkPropertyMetadata(false, OnShuffleChanged));
        public static readonly DependencyProperty TransitionProperty = DependencyProperty.Register("Transition", typeof(Transition), typeof(Slideshow), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnTransitionChanged));
        public static readonly DependencyProperty TransitionSelectorProperty = DependencyProperty.Register("TransitionSelector", typeof(TransitionSelector), typeof(Slideshow), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnTransitionSelectorChanged));
        #endregion // Constants

        #region Constructors
        /************************************************
		 * Constructors
		 ***********************************************/
        /// <summary>
        /// Initializes the static implementation of <see cref="Slideshow"/>.
        /// </summary>
        static Slideshow()
        {
            // Override the item template
            ItemsPanelTemplate template = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(SlideshowPanel)));
            template.Seal();
            ItemsControl.ItemsPanelProperty.OverrideMetadata(typeof(Slideshow), new FrameworkPropertyMetadata(template));
        }
        #endregion // Constructors
        #region Internal Methods
        /************************************************
		 * Internal Methods
		 ***********************************************/
        static private bool ValidateAutoAdvanceDuration(object value)
        {
            // It must be a duration
            if (!(value is Duration)) { return false; }

            // It must specify time (can't be automatic or infinite)
            if (!((Duration)value).HasTimeSpan) { return false; }

            return true;
        }
        #endregion // Internal Methods

        #region Overrides / Event Handlers
        /************************************************
		 * Overrides / Event Handlers
		 ***********************************************/
        private static void OnAutoAdvanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Slideshow)d).OnAutoAdvanceChanged(e);
        }

        private static void OnAutoAdvanceDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Slideshow)d).OnAutoAdvanceDurationChanged(e);
        }

        private static void OnIsContinuousChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Slideshow)d).OnIsContinuousChanged(e);
        }

        private static void OnShuffleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Slideshow)d).OnShuffleChanged(e);
        }

        private static void OnTransitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Slideshow)d).OnTransitionChanged(e);
        }
        private static void OnTransitionSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Slideshow)d).OnTransitionSelectorChanged(e);
        }
        #endregion // Overrides / Event Triggers

        #endregion // Static Version

        #region Instance Version
        #region Member Variables
        /************************************************
		 * Member Variables
		 ***********************************************/
        private DispatcherTimer autoTransitionTimer;
        private Random random = new Random();
        #endregion // Member Variables
        #region Constructors
        /************************************************
		 * Constructors
		 ***********************************************/
        /// <summary>
        /// Initializes a new <see cref="Slideshow"/> instance
        /// </summary>
        public Slideshow()
        {
            InitializeComponent();

            // Setup automatic transition
            autoTransitionTimer = new DispatcherTimer();
            autoTransitionTimer.IsEnabled = false;
            autoTransitionTimer.Interval = AutoAdvanceDuration.TimeSpan;
            autoTransitionTimer.Tick += new EventHandler(autoTransitionTimer_Tick);
            autoTransitionTimer.IsEnabled = AutoAdvance;
        }
        #endregion // Constructors

        #region Overrides / Event Handlers
        /************************************************
		 * Overrides / Event Handlers
		 ***********************************************/
        private void autoTransitionTimer_Tick(object sender, EventArgs e)
        {
            // Just try to go to the next item
            TransitionNext();
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new SlideshowItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is SlideshowItem);
        }
        #endregion // Overrides / Event Triggers

        #region Overridables / Event Triggers
        /************************************************
		 * Overridables / Event Triggers
		 ***********************************************/
        /// <summary>
        /// Occurs when the value of the <see cref="AutoAdvance"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnAutoAdvanceChanged(DependencyPropertyChangedEventArgs e)
        {
            autoTransitionTimer.IsEnabled = (bool)e.NewValue;
        }

        /// <summary>
        /// Occurs when the value of the <see cref="AutoAdvanceDuration"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnAutoAdvanceDurationChanged(DependencyPropertyChangedEventArgs e)
        {
            autoTransitionTimer.Interval = ((Duration)e.NewValue).TimeSpan;
        }

        /// <summary>
        /// Occurs when the value of the <see cref="IsContinuous"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnIsContinuousChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Occurs when the value of the <see cref="Shuffle"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnShuffleChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Occurs when the value of the <see cref="Transition"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnTransitionChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Occurs when the value of the <see cref="TransitionSelector"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnTransitionSelectorChanged(DependencyPropertyChangedEventArgs e)
        {

        }
        #endregion // Overridables / Event Triggers

        /// <summary>
        /// Causes the <see cref="Slideshow"/> to transition to the next item.
        /// </summary>
        /// <remarks>
        /// When determining the item to transition to, this method takes into account the 
        /// values of the <see cref="Shuffle"/> and <see cref="IsContinuous"/> properties. No 
        /// exception is thrown if there is no item to transition next.
        /// </remarks>
        public void TransitionNext()
        {
            // We operate differently in random mode than in sequential
            if (Shuffle)
            {
                // In shuffle mode, just transition to any random item
                SelectedIndex = random.Next(0, Items.Count);
            }
            else
            {
                // If we're on the last item we have some extra work to do
                if (SelectedIndex == (Items.Count - 1))
                {
                    // If we are operating in continuous mode we can just go back to the first item
                    if (IsContinuous)
                    {
                        SelectedIndex = 0;
                    }
                    // Otherwise we shoudl stop auto advance if it's running
                    else
                    {
                        AutoAdvance = false;
                    }
                }
                // Not the last item, so just go to the next
                else
                {
                    SelectedIndex = SelectedIndex + 1;
                }
            }
        }

        /// <summary>
        /// Causes the <see cref="Slideshow"/> to transition to the previous item.
        /// </summary>
        /// <remarks>
        /// This item always transitions to the logical previous item in the list, 
        /// regardless of the value of the <see cref="Shuffle"/> property. However, 
        /// this method will wrap around from the first logical item to the last logical 
        /// item if the <see cref="IsContinuous"/> property is set to <c>true</c> (default).
        /// </remarks>
        public void TransitionPrevious()
        {
            throw new NotImplementedException();
        }

        #region Public Properties
        /************************************************
		 * Public Properties
		 ***********************************************/
        /// <summary>
        /// Gets or sets a value that indicates if <see cref="Slideshow"/> will automatically 
        /// advance to the next item.
        /// <see cref="Slideshow"/> advances to the next item.
        /// </summary>
        /// <value>
        /// <c>true</c> if <see cref="Slideshow"/> will automatically advance to the next item; 
        /// otherwise <c>false</c>.
        /// </value>
        /// <remarks>
        /// The amount of time that <see cref="Slideshow"/> will wait before advancing to the 
        /// next item can be specified using the <see cref="AutoAdvanceDuration"/> property. 
        /// This property defaults to <c>false</c>.
        /// </remarks>
        [DefaultValue(false)]
        public bool AutoAdvance
        {
            get
            {
                return (bool)GetValue(AutoAdvanceProperty);
            }
            set
            {
                SetValue(AutoAdvanceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="Duration"/> that indicates how much time must pass before 
        /// <see cref="Slideshow"/> advances to the next item.
        /// </summary>
        /// <value>
        /// A <see cref="Duration"/> that indicates how much time must pass.
        /// </value>
        /// <remarks>
        /// This property defaults to 3 seconds and has no effect if <see cref="AutoAdvance"/> 
        /// is set to <c>false</c>.
        /// </remarks>
        public Duration AutoAdvanceDuration
        {
            get
            {
                return (Duration)GetValue(AutoAdvanceDurationProperty);
            }
            set
            {
                SetValue(AutoAdvanceDurationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates if <see cref="Slideshow"/> will treat the 
        /// items in the list as a continuous loop. 
        /// </summary>
        /// <value>
        /// <c>true</c> if <see cref="Slideshow"/> if <see cref="Slideshow"/> will treat the 
        /// items in the list as a continuous loop; otherwise <c>false</c>. The default 
        /// is <c>true</c>.
        /// </value>
        /// <remarks>
        /// When this property is set to <c>true</c> (the default) and the last item in the 
        /// list is displayed, <see cref="Slideshow"/> will transition to the first item 
        /// in the list. Similarly, if the first item is displayed and a request is made 
        /// to display the previous item, <see cref="Slideshow"/> will automatically 
        /// transition to the last item in the list.
        /// </remarks>
        [DefaultValue(true)]
        public bool IsContinuous
        {
            get
            {
                return (bool)GetValue(IsContinuousProperty);
            }
            set
            {
                SetValue(IsContinuousProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates if <see cref="Slideshow"/> will transition 
        /// randomly between items in the list.
        /// </summary>
        /// <value>
        /// <c>true</c> if <see cref="Slideshow"/> will transition randomly between items in 
        /// the list; otherwise <c>false</c>. The default is <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool Shuffle
        {
            get
            {
                return (bool)GetValue(ShuffleProperty);
            }
            set
            {
                SetValue(ShuffleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a single <see cref="Transition"/> that will be used to switch between content.
        /// </summary>
        /// <value>
        /// A single <see cref="Transition"/> that will be used to switch between content.
        /// </value>
        /// <remarks>
        /// To use more than one transition, see <see cref="TransitionSelector"/>.
        /// </remarks>
        [DefaultValue(null)]
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
        /// Gets or sets a class instance that will provide transitions to the <see cref="Slideshow"/>.
        /// </summary>
        /// <value>
        /// A <see cref="TransitionSelector"/> instance that will provide transitions.
        /// </value>
        /// <remarks>
        /// To use only a single transition, see the <see cref="Transition"/> property.
        /// </remarks>
        [DefaultValue(null)]
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
        #endregion // Public Properties
        #endregion // Instance Version
    }
}
