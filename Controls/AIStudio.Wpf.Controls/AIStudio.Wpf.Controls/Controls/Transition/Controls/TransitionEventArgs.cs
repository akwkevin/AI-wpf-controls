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

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// Provides data for events involving a transition.
    /// </summary>
    public class TransitionEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new <see cref="TransitionEventArgs"/> instance.
        /// </summary>
        /// <param name="routedEvent">
        /// The routed event this data represents.
        /// </param>
        /// <param name="source">
        /// The source of the event.
        /// </param>
        /// <param name="transition">
        /// The transition involved in the event.
        /// </param>
        /// <param name="oldContent">
        /// The old content involved in the event.
        /// </param>
        /// <param name="newContent">
        /// The new content involved in the event.
        /// </param>
        public TransitionEventArgs(RoutedEvent routedEvent, object source, Transition transition, object oldContent, object newContent) : base(routedEvent, source)
        {
            // Validate
            if (transition == null) throw new ArgumentNullException("transition");

            // Store
            Transition = transition;
            OldContent = oldContent;
            NewContent = newContent;
        }

        /// <summary>
        /// Gets the new content involved in the event.
        /// </summary>
        /// <value>
        /// The new content involved in the event.
        /// </value>
        public object NewContent
        {
            get; private set;
        }

        /// <summary>
        /// Gets the old content involved in the event.
        /// </summary>
        /// <value>
        /// The old content involved in the event.
        /// </value>
        public object OldContent
        {
            get; private set;
        }

        /// <summary>
        /// Gets the transition involved in the event.
        /// </summary>
        /// <value>
        /// The transition involved in the event.
        /// </value>
        public Transition Transition
        {
            get; private set;
        }
    }

    /// <summary>
    /// The signature for a handler of events involving transitions.
    /// </summary>
    /// <param name="sender">
    /// The sender of the event.
    /// </param>
    /// <param name="e">
    /// A <see cref="TransitionEventArgs"/> containing the event data.
    /// </param>
    public delegate void TransitionEventHandler(object sender, TransitionEventArgs e);
}
