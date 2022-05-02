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
    ///     Stores the XAML that defines the DotsTransition
    /// </summary>
    [ComVisible(false)]
    public partial class DotsTransitionFrameworkElement : FrameworkElement
    {
        public DotsTransitionFrameworkElement()
        {
            InitializeComponent();
        }
    }

    /// <summary>
    ///     Represents the DotsTransition
    /// </summary>
    [ComVisible(false)]
    public class DotsTransition : StoryboardTransition
    {
        static private DotsTransitionFrameworkElement frameworkElement = new DotsTransitionFrameworkElement();

        public DotsTransition()
        {
            this.NewContentStyle = (Style)frameworkElement.FindResource("DotsTransitionNewContentStyle");
            this.NewContentStoryboard = (Storyboard)frameworkElement.FindResource("DotsTransitionNewContentStoryboard");
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
