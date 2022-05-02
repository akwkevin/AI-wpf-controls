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
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace AIStudio.Wpf.Controls.Transitions
{
    // Transition with storyboards for the old and new content presenters
    [StyleTypedProperty(Property = "OldContentStyle", StyleTargetType = typeof(ContentPresenter))]
    [StyleTypedProperty(Property = "NewContentStyle", StyleTargetType = typeof(ContentPresenter))]
    [ComVisible(false)]
    public abstract class StoryboardTransition : Transition
    {
        public Style OldContentStyle
        {
            get
            {
                return (Style)GetValue(OldContentStyleProperty);
            }
            set
            {
                SetValue(OldContentStyleProperty, value);
            }
        }

        public static readonly DependencyProperty OldContentStyleProperty =
            DependencyProperty.Register("OldContentStyle",
                typeof(Style),
                typeof(StoryboardTransition),
                new UIPropertyMetadata(null));


        public Storyboard OldContentStoryboard
        {
            get
            {
                return (Storyboard)GetValue(OldContentStoryboardProperty);
            }
            set
            {
                SetValue(OldContentStoryboardProperty, value);
            }
        }

        public static readonly DependencyProperty OldContentStoryboardProperty =
           DependencyProperty.Register("OldContentStoryboard",
               typeof(Storyboard),
               typeof(StoryboardTransition),
               new UIPropertyMetadata(null));

        public Style NewContentStyle
        {
            get
            {
                return (Style)GetValue(NewContentStyleProperty);
            }
            set
            {
                SetValue(NewContentStyleProperty, value);
            }
        }

        public static readonly DependencyProperty NewContentStyleProperty =
            DependencyProperty.Register("NewContentStyle",
                typeof(Style),
                typeof(StoryboardTransition),
                new UIPropertyMetadata(null));

        public Storyboard NewContentStoryboard
        {
            get
            {
                return (Storyboard)GetValue(NewContentStoryboardProperty);
            }
            set
            {
                SetValue(NewContentStoryboardProperty, value);
            }
        }

        public static readonly DependencyProperty NewContentStoryboardProperty =
            DependencyProperty.Register("NewContentStoryboard",
                typeof(Storyboard),
                typeof(StoryboardTransition),
                new UIPropertyMetadata(null));


        protected internal override void BeginTransition(TransitionElement transitionElement, ContentPresenter oldContent, ContentPresenter newContent)
        {
            Storyboard oldStoryboard = OldContentStoryboard;
            Storyboard newStoryboard = NewContentStoryboard;

            if (oldStoryboard != null || newStoryboard != null)
            {
                oldContent.Style = OldContentStyle;
                newContent.Style = NewContentStyle;

                // Flag to determine when both storyboards are done
                bool done = oldStoryboard == null || newStoryboard == null;

                if (oldStoryboard != null)
                {
                    oldStoryboard = oldStoryboard.Clone();
                    oldContent.SetValue(OldContentStoryboardProperty, oldStoryboard);
                    oldStoryboard.Completed += delegate {
                        if (done)
                            EndTransition(transitionElement, oldContent, newContent);
                        done = true;
                    };
                    oldStoryboard.Begin(oldContent, true);
                }

                if (newStoryboard != null)
                {
                    newStoryboard = newStoryboard.Clone();
                    newContent.SetValue(NewContentStoryboardProperty, newStoryboard);
                    newStoryboard.Completed += delegate {
                        if (done)
                            EndTransition(transitionElement, oldContent, newContent);
                        done = true;
                    };
                    newStoryboard.Begin(newContent, true);
                }
            }
            else
            {
                EndTransition(transitionElement, oldContent, newContent);
            }
        }

        protected override void OnTransitionEnded(TransitionElement transitionElement, ContentPresenter oldContent, ContentPresenter newContent)
        {
            Storyboard oldStoryboard = (Storyboard)oldContent.GetValue(OldContentStoryboardProperty);
            if (oldStoryboard != null)
                oldStoryboard.Stop(oldContent);
            oldContent.ClearValue(ContentPresenter.StyleProperty);

            Storyboard newStoryboard = (Storyboard)newContent.GetValue(NewContentStoryboardProperty);
            if (newStoryboard != null)
                newStoryboard.Stop(newContent);
            newContent.ClearValue(ContentPresenter.StyleProperty);
        }
    }
}
