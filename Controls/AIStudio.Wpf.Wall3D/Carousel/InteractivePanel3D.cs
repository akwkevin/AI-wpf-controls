using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using _3DTools;

namespace AIStudio.Wpf.Wall3D
{
    public class InteractivePanel3D : InteractiveVisual3D, IDisposable
    {
        public double Degree = 0;

        private int Index = 0;

        private bool _IsVisible = false;

        public bool IsVisible
        {
            get
            {
                return _IsVisible;
            }
            set
            {
                _IsVisible = value;
                if (value && Visual != null)
                {
                    //AnimImage oPersonItem = this.Visual as AnimImage;
                    //if (oPersonItem != null)
                    //    oPersonItem.LoadUiImmediate();
                }
            }
        }

        private object _info;
        public object Info
        {
            set
            {
                _info = value;
                AnimImage item = Visual as AnimImage;
                item.Content = value;
            }
            get
            {
                return _info;
            }
        }

        public InteractivePanel3D(object info, int nIndex)
        {

            this.Index = nIndex;

            Geometry = CreateModel();
            Transform = this.CreateTransform();
            Visual = this.CreateVisual();
            this.Info = info;
            IsBackVisible = true;
        }

        private Transform3D CreateTransform()
        {
            var trans3DGroup = new Transform3DGroup();
            var scale3D = new ScaleTransform3D();
            trans3DGroup.Children.Add(scale3D);
            return trans3DGroup;
        }

        internal const double y = 2.38;
        internal const double x = 3.8;

        private MeshGeometry3D CreateModel()
        {
            var geometry3D = new MeshGeometry3D();
            geometry3D.Positions.Add(new Point3D(-x, y, 0));
            geometry3D.Positions.Add(new Point3D(-x, -y, 0));
            geometry3D.Positions.Add(new Point3D(x, -y, 0));
            geometry3D.Positions.Add(new Point3D(x, y, 0));

            var iCol = new[] { 0, 1, 2, 0, 2, 3 };
            var pCol = new[] { new Point(0, 0), new Point(0, 1), new Point(1, 1), new Point(1, 0) };
            var vCol = new[]
            {new Vector3D(0, 1, 0), new Vector3D(0, 1, 0), new Vector3D(0, 1, 0), new Vector3D(0, 1, 0)};

            geometry3D.TriangleIndices = new Int32Collection(iCol);
            geometry3D.TextureCoordinates = new PointCollection(pCol);
            geometry3D.Normals = new Vector3DCollection(vCol);

            return geometry3D;
        }

        private Visual CreateVisual()
        {
            AnimImage oItem = new AnimImage();
            oItem.MouseLeftButtonUp += OItem_MouseLeftButtonUp;
            return oItem;
        }

        private void OItem_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBox.Show(" Raise your Route Events");
        }

        public void Dispose()
        {
            Geometry = null;
            AnimImage oPersonItem = this.Visual as AnimImage;
            if (oPersonItem != null)
                oPersonItem.Dispose();
            Visual = null;
        }
    }
}
