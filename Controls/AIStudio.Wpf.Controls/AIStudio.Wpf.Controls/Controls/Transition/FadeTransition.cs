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
using System.Windows.Media.Animation;

namespace AIStudio.Wpf.Controls.Transitions
{
    // Simple transition that fades out the old content
    [System.Runtime.InteropServices.ComVisible(false)]
    public class FadeTransition : Transition
    {
        static FadeTransition()
        {
            AcceptsNullContentProperty.OverrideMetadata(typeof(FadeTransition), new FrameworkPropertyMetadata(NullContentSupport.New));
            IsNewContentTopmostProperty.OverrideMetadata(typeof(FadeTransition), new FrameworkPropertyMetadata(false));
        }

        public FadeTransition()
        {
            this.Duration = new Duration(TimeSpan.FromSeconds(0.5));
        }

        protected internal override void BeginTransition(TransitionElement transitionElement, ContentPresenter oldContent, ContentPresenter newContent)
        {
            DoubleAnimation da = new DoubleAnimation(0, Duration);
            da.Completed += delegate {
                EndTransition(transitionElement, oldContent, newContent);
            };
            oldContent.BeginAnimation(UIElement.OpacityProperty, da);
        }

        protected override void OnTransitionEnded(TransitionElement transitionElement, ContentPresenter oldContent, ContentPresenter newContent)
        {
            oldContent.BeginAnimation(UIElement.OpacityProperty, null);
        }
    }
}
