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
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace AIStudio.Wpf.Controls.Transitions
{
    [System.Runtime.InteropServices.ComVisible(false)]
    public class FlipTransition : Transition3D
    {
        static FlipTransition()
        {
            AcceptsNullContentProperty.OverrideMetadata(typeof(FlipTransition), new FrameworkPropertyMetadata(NullContentSupport.None));
        }

        public FlipTransition()
        {
            this.Duration = new Duration(TimeSpan.FromSeconds(0.5));
        }

        public RotateDirection Direction
        {
            get
            {
                return (RotateDirection)GetValue(DirectionProperty);
            }
            set
            {
                SetValue(DirectionProperty, value);
            }
        }

        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(RotateDirection), typeof(FlipTransition), new UIPropertyMetadata(RotateDirection.Left));


        protected override void BeginTransition3D(TransitionElement transitionElement, ContentPresenter oldContent, ContentPresenter newContent, Viewport3D viewport)
        {
            Size size = transitionElement.RenderSize;

            // Create a rectangle
            MeshGeometry3D mesh = CreateMesh(new Point3D(),
                new Vector3D(size.Width, 0, 0),
                new Vector3D(0, size.Height, 0),
                1,
                1,
                new Rect(0, 0, 1, 1));

            GeometryModel3D geometry = new GeometryModel3D();
            geometry.Geometry = mesh;
            VisualBrush clone = new VisualBrush(oldContent);
            geometry.Material = new DiffuseMaterial(clone);

            ModelVisual3D model = new ModelVisual3D();
            model.Content = geometry;

            // Replace old content in visual tree with new 3d model
            transitionElement.HideContent(oldContent);
            viewport.Children.Add(model);

            Vector3D axis;
            Point3D center = new Point3D();
            switch (Direction)
            {
                case RotateDirection.Left:
                    axis = new Vector3D(0, 1, 0);
                    break;
                case RotateDirection.Right:
                    axis = new Vector3D(0, -1, 0);
                    center = new Point3D(size.Width, 0, 0);
                    break;
                case RotateDirection.Up:
                    axis = new Vector3D(-1, 0, 0);
                    break;
                default:
                    axis = new Vector3D(1, 0, 0);
                    center = new Point3D(0, size.Height, 0);
                    break;
            }
            AxisAngleRotation3D rotation = new AxisAngleRotation3D(axis, 0);
            model.Transform = new RotateTransform3D(rotation, center);

            DoubleAnimation da = new DoubleAnimation(0, Duration);
            clone.BeginAnimation(Brush.OpacityProperty, da);

            da = new DoubleAnimation(90, Duration);
            da.Completed += delegate {
                EndTransition(transitionElement, oldContent, newContent);
            };
            rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, da);
        }
    }
}
