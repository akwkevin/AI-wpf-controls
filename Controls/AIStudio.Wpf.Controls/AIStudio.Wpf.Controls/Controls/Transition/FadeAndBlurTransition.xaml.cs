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
    ///     Stores the XAML that defines the FadeAndBlurTransition
    /// </summary>
    [ComVisible(false)]
    public partial class FadeAndBlurTransitionFrameworkElement : FrameworkElement
    {
        public FadeAndBlurTransitionFrameworkElement()
        {
            InitializeComponent();
        }
    }

    /// <summary>
    ///     Represents the FadeAndBlurTransition
    /// </summary>
    [ComVisible(false)]
    public class FadeAndBlurTransition : StoryboardTransition
    {
        static private FadeAndBlurTransitionFrameworkElement frameworkElement = new FadeAndBlurTransitionFrameworkElement();

        static FadeAndBlurTransition()
        {
            AcceptsNullContentProperty.OverrideMetadata(typeof(FadeAndBlurTransition), new FrameworkPropertyMetadata(NullContentSupport.New));
            IsNewContentTopmostProperty.OverrideMetadata(typeof(FadeAndBlurTransition), new FrameworkPropertyMetadata(false));
        }

        public FadeAndBlurTransition()
        {
            this.OldContentStyle = (Style)frameworkElement.FindResource("FadeAndBlurTransitionOldContentStyle");
            this.OldContentStoryboard = (Storyboard)frameworkElement.FindResource("FadeAndBlurTransitionOldContentStoryboard");
            this.NewContentStyle = (Style)frameworkElement.FindResource("FadeAndBlurTransitionNewContentStyle");
            this.NewContentStoryboard = (Storyboard)frameworkElement.FindResource("FadeAndBlurTransitionNewContentStoryboard");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.NotSupportedException.#ctor(System.String)")]
        protected override void OnDurationChanged(Duration oldDuration, Duration newDuration)
        {
            throw new System.NotSupportedException("CTP1 does not support changing the duration of this transition");
        }
    }
}
