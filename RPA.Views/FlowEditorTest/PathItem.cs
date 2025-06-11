using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace RPA.Views.FlowEditor
{
    /// <summary>
    /// 线对象
    /// </summary>
    internal class PathItem : BaseFlowNode
    {
        internal PathItem()
        {
            //DeleteCommand = new RelayCommand(Delete);
        }

        internal PathItem(WorkflowEditor editorParent) : this()
        {
            EditorParent = editorParent;
        }

        internal static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register(nameof(StartPoint), typeof(Point), typeof(PathItem), new PropertyMetadata((sender, e) =>
            {
                if (sender is PathItem pathItem && e.NewValue is Point point)
                {
                    Canvas.SetLeft(pathItem, point.X);
                    Canvas.SetTop(pathItem, point.Y);
                }
            }));

        internal static readonly DependencyProperty EndPointProperty =
            DependencyProperty.Register(nameof(EndPoint), typeof(Point), typeof(PathItem));

        internal static readonly DependencyProperty Point0Property =
            DependencyProperty.Register(nameof(Point0), typeof(Point), typeof(PathItem));

        internal static readonly DependencyProperty Point1Property =
            DependencyProperty.Register(nameof(Point1), typeof(Point), typeof(PathItem));

        internal static readonly DependencyProperty Point2Property =
            DependencyProperty.Register(nameof(Point2), typeof(Point), typeof(PathItem));

        internal static readonly DependencyProperty Point3Property =
            DependencyProperty.Register(nameof(Point3), typeof(Point), typeof(PathItem));

        internal static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(nameof(Points), typeof(PointCollection), typeof(PathItem));

        internal static readonly DependencyProperty StartEllipseItemProperty =
            DependencyProperty.Register(nameof(StartEllipseItem), typeof(EllipseItem), typeof(PathItem));

        internal static readonly DependencyProperty EndEllipseItemProperty =
            DependencyProperty.Register(nameof(EndEllipseItem), typeof(EllipseItem), typeof(PathItem));

        internal static readonly DependencyProperty IsCurveProperty =
            DependencyProperty.Register(nameof(IsCurve), typeof(bool), typeof(PathItem), new PropertyMetadata(true, (sender, e) =>
            {
                var pathItem = (PathItem)sender;
                if (pathItem.StartEllipseItem != null && pathItem.EndEllipseItem != null)
                {
                    var startPoint = pathItem.StartEllipseItem.GetPoint(pathItem.EditorParent);
                    var endPoint = pathItem.EndEllipseItem.GetPoint(pathItem.EditorParent);
                    pathItem.UpdateCurveAngle(startPoint, endPoint);
                }
            }));

        internal static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register(nameof(StrokeDashArray), typeof(DoubleCollection), typeof(PathItem));

        internal Point StartPoint
        {
            get => (Point)GetValue(StartPointProperty);
            set => SetValue(StartPointProperty, value);
        }

        internal Point EndPoint
        {
            get => (Point)GetValue(EndPointProperty);
            set => SetValue(EndPointProperty, value);
        }

        internal Point Point0
        {
            get => (Point)GetValue(Point0Property);
            set => SetValue(Point0Property, value);
        }

        internal Point Point1
        {
            get => (Point)GetValue(Point1Property);
            set => SetValue(Point1Property, value);
        }

        internal Point Point2
        {
            get => (Point)GetValue(Point2Property);
            set => SetValue(Point2Property, value);
        }

        internal Point Point3
        {
            get => (Point)GetValue(Point3Property);
            set => SetValue(Point3Property, value);
        }

        /// <summary>
        /// Gets or sets a collection that contains the vertex points of the polygon.
        /// </summary>
        internal PointCollection Points
        {
            get => (PointCollection)GetValue(PointsProperty);
            set => SetValue(PointsProperty, value);
        }

        internal EllipseItem StartEllipseItem
        {
            get => (EllipseItem)GetValue(StartEllipseItemProperty);
            set => SetValue(StartEllipseItemProperty, value);
        }

        internal EllipseItem EndEllipseItem
        {
            get => (EllipseItem)GetValue(EndEllipseItemProperty);
            set => SetValue(EndEllipseItemProperty, value);
        }

        internal bool IsCurve
        {
            get => (bool)GetValue(IsCurveProperty);
            set => SetValue(IsCurveProperty, value);
        }

        internal DoubleCollection StrokeDashArray
        {
            get => (DoubleCollection)GetValue(StrokeDashArrayProperty);
            set => SetValue(StrokeDashArrayProperty, value);
        }

        internal WorkflowEditor EditorParent { get; set; }

        public ICommand DeleteCommand { get; }

        internal void UpdateBezierCurve(Point startPoint, Point endPoint)
        {
            Point startPointTemp = new Point();
            Point endPointTemp = new Point();

            startPointTemp.X = Math.Min(startPoint.X, endPoint.X);
            startPointTemp.Y = Math.Min(startPoint.Y, endPoint.Y);
            endPointTemp.X = Math.Max(startPoint.X, endPoint.X);
            endPointTemp.Y = Math.Max(startPoint.Y, endPoint.Y);

            StartPoint = startPointTemp;
            EndPoint = endPointTemp;

            UpdateCurveAngle(startPoint, endPoint);
        }

        private void UpdateCurveAngle(Point startPoint, Point endPoint)
        {
            startPoint.X -= StartPoint.X;
            startPoint.Y -= StartPoint.Y;
            endPoint.X -= StartPoint.X;
            endPoint.Y -= StartPoint.Y;
            Point0 = startPoint;
            if (IsCurve)
            {
                Point1 = new Point((startPoint.X + endPoint.X) / 2, endPoint.Y);
                Point2 = new Point((startPoint.X + endPoint.X) / 2, startPoint.Y);
            }
            else
            {
                Vector vector = endPoint - startPoint;
                Point1 = startPoint + vector;
                Point2 = endPoint - vector;
            }
            Point3 = endPoint;
            UpdateArrow(Point2, endPoint);
        }

        private void UpdateArrow(Point point2, Point endPoint)
        {
            double arrowLength = 14;
            double arrowWidth = 10;

            // 计算箭头的方向
            Vector direction = endPoint - point2;
            direction.Normalize();

            // 算出箭头的两个角点
            Point arrowPoint1 = endPoint - direction * arrowLength + new Vector(-direction.Y, direction.X) * arrowWidth / 2;
            Point arrowPoint2 = endPoint - direction * arrowLength - new Vector(-direction.Y, direction.X) * arrowWidth / 2;

            // 更新箭头形状的点
            Points = new PointCollection(new Point[] { endPoint, arrowPoint1, arrowPoint2 });
        }

        internal void Delete()
        {
            StartEllipseItem.RemoveStep();
            StartEllipseItem.PathItem = null;
            StartEllipseItem = null;
            EndEllipseItem.PathItem = null;
            EndEllipseItem = null;
            ContextMenu = null;
            BindingOperations.ClearAllBindings(this);
            EditorParent.Children.Remove(this);
        }
    }
}
