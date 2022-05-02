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
    ///     Stores the XAML that defines the FadeAndGrowTransition
    /// </summary>
    [ComVisible(false)]
    public partial class FadeAndGrowTransitionFrameworkElement : FrameworkElement
    {
        public FadeAndGrowTransitionFrameworkElement()
        {
            InitializeComponent();
        }
    }

    /// <summary>
    ///     Represents the FadeAndGrowTransition
    /// </summary>
    [ComVisible(false)]
    public class FadeAndGrowTransition : StoryboardTransition
    {
        static private FadeAndGrowTransitionFrameworkElement frameworkElement = new FadeAndGrowTransitionFrameworkElement();

        static FadeAndGrowTransition()
        {
            AcceptsNullContentProperty.OverrideMetadata(typeof(FadeAndGrowTransition), new FrameworkPropertyMetadata(NullContentSupport.Both));
            IsNewContentTopmostProperty.OverrideMetadata(typeof(FadeAndGrowTransition), new FrameworkPropertyMetadata(false));
        }

        public FadeAndGrowTransition()
        {
            this.OldContentStyle = (Style)frameworkElement.FindResource("FadeAndGrowTransitionOldContentStyle");
            this.OldContentStoryboard = (Storyboard)frameworkElement.FindResource("FadeAndGrowTransitionOldContentStoryboard");
            this.NewContentStyle = (Style)frameworkElement.FindResource("FadeAndGrowTransitionNewContentStyle");
            this.NewContentStoryboard = (Storyboard)frameworkElement.FindResource("FadeAndGrowTransitionNewContentStoryboard");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.NotSupportedException.#ctor(System.String)")]
        protected override void OnDurationChanged(Duration oldDuration, Duration newDuration)
        {
            throw new System.NotSupportedException("CTP1 does not support changing the duration of this transition");
        }
    }
}
