#region File Header

// -------------------------------------------------------------------------------
// 
// This file is part of the WPFSpark project: http://wpfspark.codeplex.com/
//
// Author: Ratish Philip
// 
// WPFSpark v1.1
//
// -------------------------------------------------------------------------------

#endregion

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AIStudio.Wpf.Panels
{
    /// <summary>
    /// Class which helps in the layout management for the FluidWrapPanel
    /// </summary>
    internal sealed class FluidLayoutManager
    {
        #region Fields

        private Size panelSize;
        private Size cellSize;
        private Orientation panelOrientation;
        private Int32 cellsPerLine;

        #endregion

        #region APIs

        /// <summary>
        /// Calculates the initial location of the child in the FluidWrapPanel
        /// when the child is added.
        /// </summary>
        /// <param name="index">Index of the child in the FluidWrapPanel</param>
        /// <returns></returns>
        internal Point GetInitialLocationOfChild(int index)
        {
            Point result = new Point();

            int row, column;

            GetCellFromIndex(index, out row, out column);

            int maxRows = (Int32)Math.Floor(panelSize.Height / cellSize.Height);
            int maxCols = (Int32)Math.Floor(panelSize.Width / cellSize.Width);

            bool isLeft = true;
            bool isTop = true;
            bool isCenterHeight = false;
            bool isCenterWidth = false;

            int halfRows = 0;
            int halfCols = 0;

            halfRows = (int)((double)maxRows / (double)2);

            // Even number of rows
            if ((maxRows % 2) == 0)
            {
                isTop = row < halfRows;
            }
            // Odd number of rows
            else
            {
                if (row == halfRows)
                {
                    isCenterHeight = true;
                    isTop = false;
                }
                else
                {
                    isTop = row < halfRows;
                }
            }

            halfCols = (int)((double)maxCols / (double)2);

            // Even number of columns
            if ((maxCols % 2) == 0)
            {
                isLeft = column < halfCols;
            }
            // Odd number of columns
            else
            {
                if (column == halfCols)
                {
                    isCenterWidth = true;
                    isLeft = false;
                }
                else
                {
                    isLeft = column < halfCols;
                }
            }

            if (isCenterHeight && isCenterWidth)
            {
                double posX = (halfCols) * cellSize.Width;
                double posY = (halfRows + 2) * cellSize.Height;

                return new Point(posX, posY);
            }

            if (isCenterHeight)
            {
                if (isLeft)
                {
                    double posX = ((halfCols - column) + 1) * cellSize.Width;
                    double posY = (halfRows) * cellSize.Height;

                    result = new Point(-posX, posY);
                }
                else
                {
                    double posX = ((column - halfCols) + 1) * cellSize.Width;
                    double posY = (halfRows) * cellSize.Height;

                    result = new Point(panelSize.Width + posX, posY);
                }

                return result;
            }

            if (isCenterWidth)
            {
                if (isTop)
                {
                    double posX = (halfCols) * cellSize.Width;
                    double posY = ((halfRows - row) + 1) * cellSize.Height;

                    result = new Point(posX, -posY);
                }
                else
                {
                    double posX = (halfCols) * cellSize.Width;
                    double posY = ((row - halfRows) + 1) * cellSize.Height;

                    result = new Point(posX, panelSize.Height + posY);
                }

                return result;
            }

            if (isTop)
            {
                if (isLeft)
                {
                    double posX = ((halfCols - column) + 1) * cellSize.Width;
                    double posY = ((halfRows - row) + 1) * cellSize.Height;

                    result = new Point(-posX, -posY);
                }
                else
                {
                    double posX = ((column - halfCols) + 1) * cellSize.Width;
                    double posY = ((halfRows - row) + 1) * cellSize.Height;

                    result = new Point(posX + panelSize.Width, -posY);
                }
            }
            else
            {
                if (isLeft)
                {
                    double posX = ((halfCols - column) + 1) * cellSize.Width;
                    double posY = ((row - halfRows) + 1) * cellSize.Height;

                    result = new Point(-posX, panelSize.Height + posY);
                }
                else
                {
                    double posX = ((column - halfCols) + 1) * cellSize.Width;
                    double posY = ((row - halfRows) + 1) * cellSize.Height;

                    result = new Point(posX + panelSize.Width, panelSize.Height + posY);
                }
            }

            return result;
        }

        /// <summary>
        /// Initializes the FluidLayoutManager
        /// </summary>
        /// <param name="panelWidth">Width of the FluidWrapPanel</param>
        /// <param name="panelHeight">Height of the FluidWrapPanel</param>
        /// <param name="cellWidth">Width of each child in the FluidWrapPanel</param>
        /// <param name="cellHeight">Height of each child in the FluidWrapPanel</param>
        /// <param name="orientation">Orientation of the panel - Horizontal or Vertical</param>
        internal void Initialize(double panelWidth, double panelHeight, double cellWidth, double cellHeight, Orientation orientation)
        {
            if (panelWidth <= 0.0d)
                panelWidth = cellWidth;
            if (panelHeight <= 0.0d)
                panelHeight = cellHeight;
            if ((cellWidth <= 0.0d) || (cellHeight <= 0.0d))
            {
                cellsPerLine = 0;
                return;
            }

            if ((panelSize.Width != panelWidth) ||
                (panelSize.Height != panelHeight) ||
                (cellSize.Width != cellWidth) ||
                (cellSize.Height != cellHeight))
            {
                panelSize = new Size(panelWidth, panelHeight);
                cellSize = new Size(cellWidth, cellHeight);
                panelOrientation = orientation;

                // Calculate the number of cells that can be fit in a line
                CalculateCellsPerLine();
            }
        }

        /// <summary>
        /// Provides the index of the child (in the FluidWrapPanel's children) from the given row and column
        /// </summary>
        /// <param name="row">Row</param>
        /// <param name="column">Column</param>
        /// <returns>Index</returns>
        internal int GetIndexFromCell(int row, int column)
        {
            int result = -1;

            if ((row >= 0) && (column >= 0))
            {
                switch (panelOrientation)
                {
                    case Orientation.Horizontal:
                        result = (cellsPerLine * row) + column;
                        break;
                    case Orientation.Vertical:
                        result = (cellsPerLine * column) + row;
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Provides the index of the child (in the FluidWrapPanel's children) from the given point
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        internal int GetIndexFromPoint(Point p)
        {
            int result = -1;
            if ((p.X > 0.00D) &&
                (p.X < panelSize.Width) &&
                (p.Y > 0.00D) &&
                (p.Y < panelSize.Height))
            {
                int row;
                int column;

                GetCellFromPoint(p, out row, out column);
                result = GetIndexFromCell(row, column);
            }

            return result;
        }

        /// <summary>
        /// Provides the row and column of the child based on its index in the FluidWrapPanel.Children
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="row">Row</param>
        /// <param name="column">Column</param>
        internal void GetCellFromIndex(int index, out int row, out int column)
        {
            row = column = -1;

            if (index >= 0)
            {
                switch (panelOrientation)
                {
                    case Orientation.Horizontal:
                        row = (int)(index / (double)cellsPerLine);
                        column = (int)(index % (double)cellsPerLine);
                        break;
                    case Orientation.Vertical:
                        column = (int)(index / (double)cellsPerLine);
                        row = (int)(index % (double)cellsPerLine);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Provides the row and column of the child based on its location in the FluidWrapPanel
        /// </summary>
        /// <param name="p">Location of the child in the parent</param>
        /// <param name="row">Row</param>
        /// <param name="column">Column</param>
        internal void GetCellFromPoint(Point p, out int row, out int column)
        {
            row = column = -1;

            if ((p.X < 0.00D) ||
                (p.X > panelSize.Width) ||
                (p.Y < 0.00D) ||
                (p.Y > panelSize.Height))
            {
                return;
            }

            row = (int)(p.Y / cellSize.Height);
            column = (int)(p.X / cellSize.Width);
        }

        /// <summary>
        /// Provides the location of the child in the FluidWrapPanel based on the given row and column
        /// </summary>
        /// <param name="row">Row</param>
        /// <param name="column">Column</param>
        /// <returns>Location of the child in the panel</returns>
        internal Point GetPointFromCell(int row, int column)
        {
            Point result = new Point();

            if ((row >= 0) && (column >= 0))
            {
                result = new Point(cellSize.Width * column, cellSize.Height * row);
            }

            return result;
        }

        /// <summary>
        /// Provides the location of the child in the FluidWrapPanel based on the given row and column
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Location of the child in the panel</returns>
        internal Point GetPointFromIndex(int index)
        {
            Point result = new Point();

            if (index >= 0)
            {
                int row;
                int column;

                GetCellFromIndex(index, out row, out column);
                result = GetPointFromCell(row, column);
            }

            return result;
        }

        /// <summary>
        /// Creates a TransformGroup based on the given Translation, Scale and Rotation
        /// </summary>
        /// <param name="transX">Translation in the X-axis</param>
        /// <param name="transY">Translation in the Y-axis</param>
        /// <param name="scaleX">Scale factor in the X-axis</param>
        /// <param name="scaleY">Scale factor in the Y-axis</param>
        /// <param name="rotAngle">Rotation</param>
        /// <returns>TransformGroup</returns>
        internal TransformGroup CreateTransform(double transX, double transY, double scaleX, double scaleY, double rotAngle = 0.0D)
        {
            TranslateTransform translation = new TranslateTransform();
            translation.X = transX;
            translation.Y = transY;

            ScaleTransform scale = new ScaleTransform();
            scale.ScaleX = scaleX;
            scale.ScaleY = scaleY;

            //RotateTransform rotation = new RotateTransform();
            //rotation.Angle = rotAngle;

            TransformGroup transform = new TransformGroup();
            // THE ORDER OF TRANSFORM IS IMPORTANT
            // First, scale, then rotate and finally translate
            transform.Children.Add(scale);
            //transform.Children.Add(rotation);
            transform.Children.Add(translation);

            return transform;
        }

        /// <summary>
        /// Creates the storyboard for animating a child from its old location to the new location.
        /// The Translation and Scale properties are animated.
        /// </summary>
        /// <param name="element">UIElement for which the storyboard has to be created</param>
        /// <param name="newLocation">New location of the UIElement</param>
        /// <param name="period">Duration of animation</param>
        /// <param name="easing">Easing function</param>
        /// <returns>Storyboard</returns>
        internal Storyboard CreateTransition(UIElement element, Point newLocation, TimeSpan period, EasingFunctionBase easing)
        {
            Duration duration = new Duration(period);

            // Animate X
            DoubleAnimation translateAnimationX = new DoubleAnimation();
            translateAnimationX.To = newLocation.X;
            translateAnimationX.Duration = duration;
            if (easing != null)
                translateAnimationX.EasingFunction = easing;

            Storyboard.SetTarget(translateAnimationX, element);
            Storyboard.SetTargetProperty(translateAnimationX,
                new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.X)"));

            // Animate Y
            DoubleAnimation translateAnimationY = new DoubleAnimation();
            translateAnimationY.To = newLocation.Y;
            translateAnimationY.Duration = duration;
            if (easing != null)
                translateAnimationY.EasingFunction = easing;

            Storyboard.SetTarget(translateAnimationY, element);
            Storyboard.SetTargetProperty(translateAnimationY,
                new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.Y)"));

            // Animate ScaleX
            DoubleAnimation scaleAnimationX = new DoubleAnimation();
            scaleAnimationX.To = 1.0D;
            scaleAnimationX.Duration = duration;
            if (easing != null)
                scaleAnimationX.EasingFunction = easing;

            Storyboard.SetTarget(scaleAnimationX, element);
            Storyboard.SetTargetProperty(scaleAnimationX,
                new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));

            // Animate ScaleY
            DoubleAnimation scaleAnimationY = new DoubleAnimation();
            scaleAnimationY.To = 1.0D;
            scaleAnimationY.Duration = duration;
            if (easing != null)
                scaleAnimationY.EasingFunction = easing;

            Storyboard.SetTarget(scaleAnimationY, element);
            Storyboard.SetTargetProperty(scaleAnimationY,
                new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));

            Storyboard sb = new Storyboard();
            sb.Duration = duration;
            sb.Children.Add(translateAnimationX);
            sb.Children.Add(translateAnimationY);
            sb.Children.Add(scaleAnimationX);
            sb.Children.Add(scaleAnimationY);

            return sb;
        }

        /// <summary>
        /// Gets the total size taken up by the children after the Arrange Layout Phase
        /// </summary>
        /// <param name="childrenCount">Number of children</param>
        /// <param name="finalSize">Available size provided by the FluidWrapPanel</param>
        /// <returns>Total size</returns>
        internal Size GetArrangedSize(int childrenCount, Size finalSize)
        {
            if ((cellsPerLine == 0.0) || (childrenCount == 0))
                return finalSize;

            int numLines = (Int32)(childrenCount / (double)cellsPerLine);
            int modLines = childrenCount % cellsPerLine;
            if (modLines > 0)
                numLines++;

            if (panelOrientation == Orientation.Horizontal)
            {
                return new Size(cellsPerLine * cellSize.Width, numLines * cellSize.Height);
            }

            return new Size(numLines * cellSize.Width, cellsPerLine * cellSize.Height);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Calculates the number of child items that can be accommodated in a single line
        /// </summary>
        private void CalculateCellsPerLine()
        {
            double count = (panelOrientation == Orientation.Horizontal) ? panelSize.Width / cellSize.Width :
                                                                          panelSize.Height / cellSize.Height;
            cellsPerLine = (Int32)Math.Floor(count);
            if ((1.0D + cellsPerLine - count) < Double.Epsilon)
                cellsPerLine++;
        }

        #endregion
    }
}
