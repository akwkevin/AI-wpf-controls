#region License Revision: 0 Last Revised: 3/29/2006 8:21 AM
/******************************************************************************
Copyright (c) Microsoft Corporation.  All rights reserved.


This file is licensed under the Microsoft Public License (Ms-PL). A copy of the Ms-PL should accompany this file. 
If it does not, you can obtain a copy from: 

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx
******************************************************************************/
#endregion // License
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Animation;

namespace AIStudio.Wpf.Controls.Transitions
{
    /// <summary>
    ///     Stores the XAML that defines the DoubleRotateWipeTransition
    /// </summary>
    [ComVisible(false)]
    public partial class DoubleRotateWipeTransitionFrameworkElement : FrameworkElement
    {
        public DoubleRotateWipeTransitionFrameworkElement()
        {
            InitializeComponent();
        }
    }

    /// <summary>
    ///     Represents the DoubleRotateWipeTransition
    /// </summary>
    [ComVisible(false)]
    public class DoubleRotateWipeTransition : StoryboardTransition
    {
        static private DoubleRotateWipeTransitionFrameworkElement frameworkElement = new DoubleRotateWipeTransitionFrameworkElement();

        public DoubleRotateWipeTransition()
        {
            this.NewContentStyle = (Style)frameworkElement.FindResource("DoubleRotateWipeTransitionNewContentStyle");
            this.NewContentStoryboard = (Storyboard)frameworkElement.FindResource("DoubleRotateWipeTransitionNewContentStoryboard");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.NotSupportedException.#ctor(System.String)")]
        protected override void OnDurationChanged(Duration oldDuration, Duration newDuration)
        {
            throw new System.NotSupportedException("CTP1 does not support changing the duration of this transition");
        }
    }
}
