#region License Revision: 0 Last Revised: 3/29/2006 8:21 AM
/******************************************************************************
Copyright (c) Microsoft Corporation.  All rights reserved.


This file is licensed under the Microsoft Public License (Ms-PL). A copy of the Ms-PL should accompany this file. 
If it does not, you can obtain a copy from: 

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx
******************************************************************************/
#endregion // License
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Animation;

namespace AIStudio.Wpf.Controls.Transitions
{
    /// <summary>
    ///     Stores the XAML that defines the RotateWipeTransition
    /// </summary>
    [ComVisible(false)]
    public partial class RotateWipeTransitionFrameworkElement : FrameworkElement
    {
        public RotateWipeTransitionFrameworkElement()
        {
            InitializeComponent();
        }
    }

    /// <summary>
    ///     Represents the RotateWipeTransition
    /// </summary>
    [ComVisible(false)]
    public class RotateWipeTransition : StoryboardTransition
    {
        static private RotateWipeTransitionFrameworkElement frameworkElement = new RotateWipeTransitionFrameworkElement();

        public RotateWipeTransition()
        {
            this.NewContentStyle = (Style)frameworkElement.FindResource("RotateWipeTransitionNewContentStyle");
            this.NewContentStoryboard = (Storyboard)frameworkElement.FindResource("RotateWipeTransitionNewContentStoryboard");
            this.Duration = new Duration(TimeSpan.FromSeconds(0.5));
        }

        protected override void OnDurationChanged(Duration oldDuration, Duration newDuration)
        {
            if (this.NewContentStoryboard != null && this.NewContentStoryboard.Children.Count > 0)
            {
                this.NewContentStoryboard.Children[0].Duration = newDuration;
            }
        }
    }
}
