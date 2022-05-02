#region License Revision: 0 Last Revised: 3/29/2006 8:21 AM
/******************************************************************************
Copyright (c) Microsoft Corporation.  All rights reserved.


This file is licensed under the Microsoft Public License (Ms-PL). A copy of the Ms-PL should accompany this file. 
If it does not, you can obtain a copy from: 

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx
******************************************************************************/
#endregion // License
using System.Windows;

namespace AIStudio.Wpf.Controls.Transitions
{
    public enum TransitionDirection
    {
        Forward,
        Backward,
    }

    //Choose between a forward and backward transition based on the Direction property
    [System.Runtime.InteropServices.ComVisible(false)]
    public class TwoWayTransitionSelector : TransitionSelector
    {
        public TwoWayTransitionSelector()
        {
        }

        public Transition ForwardTransition
        {
            get
            {
                return (Transition)GetValue(ForwardTransitionProperty);
            }
            set
            {
                SetValue(ForwardTransitionProperty, value);
            }
        }

        public static readonly DependencyProperty ForwardTransitionProperty =
            DependencyProperty.Register("ForwardTransition", typeof(Transition), typeof(TwoWayTransitionSelector), new UIPropertyMetadata(null));

        public Transition BackwardTransition
        {
            get
            {
                return (Transition)GetValue(BackwardTransitionProperty);
            }
            set
            {
                SetValue(BackwardTransitionProperty, value);
            }
        }

        public static readonly DependencyProperty BackwardTransitionProperty =
            DependencyProperty.Register("BackwardTransition", typeof(Transition), typeof(TwoWayTransitionSelector), new UIPropertyMetadata(null));


        public TransitionDirection Direction
        {
            get
            {
                return (TransitionDirection)GetValue(DirectionProperty);
            }
            set
            {
                SetValue(DirectionProperty, value);
            }
        }

        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(TransitionDirection), typeof(TwoWayTransitionSelector), new UIPropertyMetadata(TransitionDirection.Forward));


        public override Transition SelectTransition(object oldContent, object newContent)
        {
            return Direction == TransitionDirection.Forward ? ForwardTransition : BackwardTransition;
        }
    }
}
