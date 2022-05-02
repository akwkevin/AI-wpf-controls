#region License Revision: 0 Last Revised: 3/29/2006 8:21 AM
/******************************************************************************
Copyright (c) Microsoft Corporation.  All rights reserved.


This file is licensed under the Microsoft Public License (Ms-PL). A copy of the Ms-PL should accompany this file. 
If it does not, you can obtain a copy from: 

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx
******************************************************************************/
#endregion // License
using System.Windows;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// Allows different transitions to run based on the old and new contents.
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public abstract class TransitionSelector : DependencyObject
    {
        /// <summary>
        /// When overridden in a derived class, returns a <see cref="Transition"/> based on custom logic.
        /// </summary>
        /// <param name="oldContent">
        /// The old content that is currently displayed.
        /// </param>
        /// <param name="newContent">
        /// The new content that is to be displayed.
        /// </param>
        /// <returns>
        /// The transition used to display the content or <see langword="null"/> if a 
        /// transition should not be used.
        /// </returns>
        public virtual Transition SelectTransition(object oldContent, object newContent)
        {
            return null;
        }
    }
}
