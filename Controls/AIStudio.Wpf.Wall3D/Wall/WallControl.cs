using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using AIStudio.Wpf.Wall3D.Utils;

namespace AIStudio.Wpf.Wall3D.Wall
{
    [TemplatePart(Name = PART_3dgrid, Type = typeof(Grid))]
    [TemplatePart(Name = PART_viewport3d, Type = typeof(Viewport3D))]
    [TemplatePart(Name = PART_camera, Type = typeof(PerspectiveCamera))]
    [TemplatePart(Name = PART_camTrans, Type = typeof(TranslateTransform3D))]
    [TemplatePart(Name = PART_camScale, Type = typeof(ScaleTransform3D))]
    [TemplatePart(Name = PART_camRotation, Type = typeof(AxisAngleRotation3D))]
    public class WallControl : Control
    {
        private const string PART_3dgrid = "PART_3dgrid";
        private const string PART_viewport3d = "PART_viewport3d";
        private const string PART_camera = "PART_camera";
        private const string PART_camTrans = "PART_camTrans";
        private const string PART_camScale = "PART_camScale";
        private const string PART_camRotation = "PART_camRotation";

        private Grid _3dgrid;
        private Viewport3D _viewport3d;
        private PerspectiveCamera _camera;
        private TranslateTransform3D _camTrans;
        private ScaleTransform3D _camScale;
        private AxisAngleRotation3D _camRotation;

        private Window _window;

        #region ItemsSource
        /// <summary>
        /// ItemsSource
        /// </summary>
        public IList ItemsSource
        {
            get
            {
                return (IList)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        ///// <summary>
        ///// ItemsSource
        ///// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList), typeof(WallControl), new FrameworkPropertyMetadata(null,
                (d, e) => {
                    if (d is WallControl)
                    {
                        (d as WallControl).LoadMedia();
                    }
                }));

        #endregion

        public bool FillAll { get; set; } = true;

        public Orientation Orientation { get; set; } = Orientation.Horizontal;

        public WallControl()
        {
            //获取资源文件信息
            ResourceDictionary rd = new ResourceDictionary();
            rd.Source = new Uri("Util.3DWall;component/Wall/WallControl.xaml", UriKind.Relative);
            if (!this.Resources.MergedDictionaries.Any(p => p.Source == rd.Source))
            {
                this.Resources.MergedDictionaries.Add(rd);
            }


            Loaded += MianWall_Loaded;

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _3dgrid = GetTemplateChild(PART_3dgrid) as Grid;
            _viewport3d = GetTemplateChild(PART_viewport3d) as Viewport3D;
            _camera = GetTemplateChild(PART_camera) as PerspectiveCamera;
            _camTrans = GetTemplateChild(PART_camTrans) as TranslateTransform3D;
            _camScale = GetTemplateChild(PART_camScale) as ScaleTransform3D;
            _camRotation = GetTemplateChild(PART_camRotation) as AxisAngleRotation3D;

            SetSize();
            CreatWall();
            AddMouseEvent();
            LoadMedia();
        }

        void MianWall_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MianWall_Loaded;
            _window = ExtendUtils.FindVisualParent<Window>(this);
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)//
        {
            if (_offsetX > 2)
                MoveCamera(.75);
            else if (_offsetX < -2)
                MoveCamera(-.75);
        }

        #region 鼠标操作
        private Point Start;
        private Point _prePoint; //鼠标点
        private double _offsetX = 5d; //位移量

        void AddMouseEvent()
        {
            _3dgrid.MouseLeftButtonDown += MianWall_MouseLeftButtonDown;
        }
        /// <summary>
        /// 点中
        /// </summary>
        /// <typeparam name="T">类型</typeparam>                                                          
        /// <param name="point">触激点</param>
        /// <returns></returns>
        public T HitObject<T>(Point point) where T : DependencyObject
        {
            PointHitTestParameters pointparams = new PointHitTestParameters(point);
            HitTestResult hRresult = VisualTreeHelper.HitTest(this, point);
            if (null != hRresult)
            {
                if (hRresult.VisualHit.GetType() == typeof(T))
                {
                    return hRresult.VisualHit as T;
                }

            }
            return null;
        }
        void WHD_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(this);
            _offsetX = _prePoint.X - p.X;
            MoveCamera(_offsetX);
            _prePoint = p;

        }
        Block3d _block3d;
        void WHD_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)//鼠标立起来
        {
            _window.MouseLeftButtonUp -= WHD_MouseLeftButtonUp;
            _window.MouseMove -= WHD_MouseMove;
            this.MouseLeftButtonUp -= WHD_MouseLeftButtonUp;
            this.MouseMove -= WHD_MouseMove;
            _3dgrid.MouseLeftButtonDown += MianWall_MouseLeftButtonDown;
            for (int i = -3; i < 4; i++)
            {
                if (e.GetPosition(this).X == Start.X)
                {
                    _block3d = HitObject<Block3d>(e.GetPosition(this));
                    if (_block3d != null)
                    {
                        if (ItemClick != null)
                        {
                            ItemClick(this, new ItemclickEventArg() { Data = _block3d.Info });
                        }
                    }
                    break;
                }
            }
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            CompositionTarget.Rendering += CompositionTarget_Rendering;

        }




        void MianWall_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)//鼠标按下
        {

            _offsetX = 0d;
            _3dgrid.MouseLeftButtonDown -= MianWall_MouseLeftButtonDown;
            _window.MouseMove += WHD_MouseMove;
            _window.MouseLeftButtonUp += WHD_MouseLeftButtonUp;
            this.MouseMove += WHD_MouseMove;
            this.MouseLeftButtonUp += WHD_MouseLeftButtonUp;
            Start = e.GetPosition(this);
            _prePoint = e.GetPosition(this);

        }

        #endregion

        /// <summary>
        /// 设置尺寸
        /// </summary>
        private void SetSize()
        {
            _camera.FieldOfView = 36d;
        }

        /// <summary>
        /// 移动镜头
        /// </summary>
        /// <param name="deltaX">偏移量</param>
        private void MoveCamera(double deltaX)
        {
            _camRotation.Angle += deltaX * .0588;
        }

        List<Block3d> _BlockList = new List<Block3d>();
        public static int MESH_COUNT = 87;  //块个数
        private void CreatWall()//创建墙体
        {
            double radianIncrement = -.217; // mesh 位置分配增量（度）
            double startDegrees = -270;  //环形起始度数
            double startRadians = DegreesToRadians(startDegrees); //度数转换为弧度
            double radians = startRadians;  // 弧度
            double radius = 20; //半径
            double height = 15;  //最底层块高度
            Block3d block3d;
            LocationInfo loacation;

            Block3d[,] block3Ds = new Block3d[3, 29];
            int j = 0, k = 0;
            for (int i = 0; i < MESH_COUNT; i++)//
            {
                Point3D p = CalculateRingPoint(radians, radius, height);
                double degrees = -1 * (RadiansToDegrees(radians) - startDegrees);
                loacation = new LocationInfo(p, degrees, i);
                block3d = new Block3d(loacation);
                //_BlockList.Add(block3d);
                block3Ds[k, j] = block3d;
                radians += radianIncrement;
                if ((i + 1) % 29 == 0)
                {
                    radians = startRadians;
                    height += 3.5;
                    j = 0;
                    k++;
                }
                else
                {
                    j++;
                }
            }

            if (Orientation == Orientation.Horizontal)
            {
                _BlockList = block3Ds.Cast<Block3d>().ToList();
            }
            else
            {
                _BlockList = Rotate(block3Ds).Cast<Block3d>().ToList();
            }

            foreach (var block in _BlockList)
            {
                _viewport3d.Children.Add(block);
            }
        }

        static Block3d[,] Rotate(Block3d[,] array)
        {
            int x = array.GetUpperBound(0); // 一维
            int y = array.GetUpperBound(1); // 二维
            Block3d[,] newArray = new Block3d[y + 1, x + 1]; // 构造转置二维数组
            for (int i = 0; i <= x; i++)
            {
                for (int j = 0; j <= y; j++)
                {
                    newArray[j, i] = array[i, j];
                }
            }
            return newArray;
        }

        private void LoadMedia()//载入数据
        {
            if (ItemsSource == null || ItemsSource.Count == 0)
            {
                for (int i = 0; i < _BlockList.Count; i++)
                {
                    _BlockList[i].Info = null;
                }
                return;
            }

            if (FillAll)
            {
                for (int i = 0; i < _BlockList.Count; i++)
                {
                    _BlockList[i].Info = ItemsSource[i % ItemsSource.Count];
                }
            }
            else
            {
                for (int i = 0; i < _BlockList.Count; i++)
                {
                    if (i < ItemsSource.Count)
                    {
                        _BlockList[i].Info = ItemsSource[i];
                    }
                    else
                    {
                        _BlockList[i].Info = null;
                    }
                }
            }
        }
        #region<<弧形墙算法

        //弧度转换为度
        private double RadiansToDegrees(double radians)
        {
            return radians * 57.2957795;
        }

        //度转换为弧度
        private double DegreesToRadians(double degrees)
        {
            return degrees / 57.2957795;
        }

        private Point3D CalculateRingPoint(double radians, double radius, double height)//计算小块位置
        {
            double x = Math.Cos(radians) * radius;
            double y = Math.Sin(radians) * radius;
            return new Point3D(x, height, y);
        }


        #endregion


        #region<<事件

        public class ItemclickEventArg : EventArgs
        {
            public object Data;
        }

        public delegate void ItemClickeventHandler(object sender, ItemclickEventArg e);

        public event ItemClickeventHandler ItemClick;
        #endregion
    }
}
