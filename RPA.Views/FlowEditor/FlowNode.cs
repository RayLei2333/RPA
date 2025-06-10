using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RPA.Views.FlowEditor
{
    /// <summary>
    /// 流程节点
    /// </summary>
    public class FlowNode : ContentControl
    {
        /// <summary>
        /// 是否被选中
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// 是否可拖动
        /// </summary>
        public bool IsDraggable { get; set; }

        /// <summary>
        /// 节点形状
        /// </summary>
        public ShapeType ShapeType { get; set; }

        /// <summary>
        /// 圆角范围
        /// </summary>
        public double BorderRadius { get; set; }

        /// <summary>
        /// 倾斜度
        /// </summary>
        public double Shear { get; set; } = 20;

        /// <summary>
        /// 多选模式下，鼠标点下的坐标
        /// </summary>
        internal Point MouseDownControlPoint { get; set; }

        public Geometry Geometry { get; set; }

        public object LastStep
        {
            get;
            set;
        }

        public object FromStep
        {
            get;
            set;
        }

        public object JumpStep
        {
            get;
            set;
        }

        public object NextStep
        {
            get;
            set;
        }

        public event RoutedEventHandler Selected;

        public event RoutedEventHandler Unselected;

        private EllipseItem EllipseLeft;
        private EllipseItem EllipseTop;
        private EllipseItem EllipseRight;
        private EllipseItem EllipseBottom;
        internal Dictionary<Dock, EllipseItem> EllipseItems { get; private set; }

        public WorkflowEditor EditorParent { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            EllipseLeft = GetTemplateChild("EllipseLeft") as EllipseItem;
            EllipseTop = GetTemplateChild("EllipseTop") as EllipseItem;
            EllipseRight = GetTemplateChild("EllipseRight") as EllipseItem;
            EllipseBottom = GetTemplateChild("EllipseBottom") as EllipseItem;
            EllipseItems = new Dictionary<Dock, EllipseItem>();

            EllipseItems.Add(EllipseLeft.Dock, EllipseLeft);
            EllipseItems.Add(EllipseTop.Dock, EllipseTop);
            EllipseItems.Add(EllipseRight.Dock, EllipseRight);
            EllipseItems.Add(EllipseBottom.Dock, EllipseBottom);
            foreach (var item in EllipseItems.Values)
            {
                item.WorkflowParent = this;
            }
            //IsInit = true;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var background = Background;
            var pen = new Pen(BorderBrush, new double[] { BorderThickness.Left, BorderThickness.Top, BorderThickness.Right, BorderThickness.Bottom }.Max());

            double num = pen.Thickness * 0.5;
            switch (ShapeType)
            {
                case ShapeType.Rectangle:
                    Rect rect = new Rect(new Point(num, num), new Point(ActualWidth - num, ActualHeight - num));
                    DrawRoundedRectangle(drawingContext, background, pen, rect);
                    break;
                case ShapeType.Circle:
                    break;
                case ShapeType.Ellipse:
                    break;
                case ShapeType.Diamond:
                    DrawPolygon(drawingContext, background, pen, new Point(ActualWidth / 2, num), new Point(ActualWidth - num, ActualHeight / 2), new Point(ActualWidth / 2, ActualHeight - num), new Point(num, ActualHeight / 2));
                    break;
                case ShapeType.Parallelogram:
                    DrawPolygon(drawingContext, background, pen, new Point(Math.Min(Shear, ActualWidth), num), new Point(ActualWidth - num, num), new Point(Math.Max(0, ActualWidth - Shear), ActualHeight - num), new Point(num, ActualHeight - num));
                    break;
            }
        }

        private void DrawRoundedRectangle(DrawingContext dc, Brush brush, Pen pen, Rect rect)
        {
            var geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open())
            {
                var minX = rect.Width / 2;
                var minY = rect.Height / 2;
                CornerRadius cornerRadius = new CornerRadius(BorderRadius);
                cornerRadius.TopLeft = Math.Min(Math.Min(cornerRadius.TopLeft, minX), minY);
                cornerRadius.TopRight = Math.Min(Math.Min(cornerRadius.TopRight, minX), minY);
                cornerRadius.BottomRight = Math.Min(Math.Min(cornerRadius.BottomRight, minX), minY);
                cornerRadius.BottomLeft = Math.Min(Math.Min(cornerRadius.BottomLeft, minX), minY);

                ctx.BeginFigure(new Point(rect.Left + cornerRadius.TopLeft, rect.Top), true, true);

                // Top line and top-right corner
                ctx.LineTo(new Point(rect.Right - cornerRadius.TopRight, rect.Top), true, false);
                ctx.ArcTo(new Point(rect.Right, rect.Top + cornerRadius.TopRight), new Size(cornerRadius.TopRight, cornerRadius.TopRight), 0, false, SweepDirection.Clockwise, true, false);

                // Right line and bottom-right corner
                ctx.LineTo(new Point(rect.Right, rect.Bottom - cornerRadius.BottomRight), true, false);
                ctx.ArcTo(new Point(rect.Right - cornerRadius.BottomRight, rect.Bottom), new Size(cornerRadius.BottomRight, cornerRadius.BottomRight), 0, false, SweepDirection.Clockwise, true, false);

                // Bottom line and bottom-left corner
                ctx.LineTo(new Point(rect.Left + cornerRadius.BottomLeft, rect.Bottom), true, false);
                ctx.ArcTo(new Point(rect.Left, rect.Bottom - cornerRadius.BottomLeft), new Size(cornerRadius.BottomLeft, cornerRadius.BottomLeft), 0, false, SweepDirection.Clockwise, true, false);

                // Left line and top-left corner
                ctx.LineTo(new Point(rect.Left, rect.Top + cornerRadius.TopLeft), true, false);
                ctx.ArcTo(new Point(rect.Left + cornerRadius.TopLeft, rect.Top), new Size(cornerRadius.TopLeft, cornerRadius.TopLeft), 0, false, SweepDirection.Clockwise, true, false);
            }
            geometry.Freeze();
            Geometry = geometry;
            dc.DrawGeometry(brush, pen, geometry);
        }

        private void DrawPolygon(DrawingContext dc, Brush brush, Pen pen, params Point[] points)
        {
            if (!points.Any())
            {
                return;
            }

            var geometry = new StreamGeometry();
            using (var geometryContext = geometry.Open())
            {
                geometryContext.BeginFigure(points[0], true, true);
                for (var i = 1; i < points.Length; i++)
                {
                    geometryContext.LineTo(points[i], true, false);
                }
            }
            geometry.Freeze();
            Geometry = geometry;
            dc.DrawGeometry(brush, pen, geometry);
        }
        //private static void OnWorkflowItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    ((FlowNode)d).OnWorkflowItemChanged(e);
        //}

        //protected virtual void OnWorkflowItemChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    UpdateCurve();
        //}
        public void UpdateCurve()
        {
           
            if (LastStep != null)
            {
                UpdateCurve(EllipseItems[Dock.Top].PathItem, FirstOrDefault(LastStep).EllipseItems[Dock.Bottom], EllipseItems[Dock.Top]);
            }
            if (NextStep != null)
            {
                UpdateCurve(EllipseItems[Dock.Bottom].PathItem, EllipseItems[Dock.Bottom], FirstOrDefault(NextStep).EllipseItems[Dock.Top]);
            }
            if (FromStep != null)
            {
                UpdateCurve(EllipseItems[Dock.Left].PathItem, FirstOrDefault(FromStep).EllipseItems[Dock.Right], EllipseItems[Dock.Left]);
            }
            if (JumpStep != null)
            {
                UpdateCurve(EllipseItems[Dock.Right].PathItem, EllipseItems[Dock.Right], FirstOrDefault(JumpStep).EllipseItems[Dock.Left]);
            }
        }

        private void UpdateCurve(PathItem pathItem, EllipseItem startEllipseItem, EllipseItem endEllipseItem)
        {
            if (!startEllipseItem.IsVisible || !endEllipseItem.IsVisible)
            {
                return;
            }

            if (pathItem == null)
            {
                pathItem = new PathItem(EditorParent);
                if (DataContext is not FlowNode)
                {
                    pathItem.Content = DataContext;
                    pathItem.ContentTemplate = EditorParent.PathTemplate;
                    pathItem.ContentTemplateSelector = EditorParent.PathTemplateSelector;
                }
                EditorParent.Children.Add(pathItem);
                startEllipseItem.PathItem = pathItem;
                endEllipseItem.PathItem = pathItem;
                pathItem.StartEllipseItem = startEllipseItem;
                pathItem.EndEllipseItem = endEllipseItem;
            }
            pathItem.UpdateBezierCurve(pathItem.StartEllipseItem.GetPoint(EditorParent), pathItem.EndEllipseItem.GetPoint(EditorParent));
        }

        internal FlowNode FirstOrDefault(object item)
        {
            return EditorParent.FirstOrDefault(item);
        }
        //    private void UpdateCurve(PathItem pathItem, EllipseItem startEllipseItem, EllipseItem endEllipseItem)
        //    {
        //        if (!startEllipseItem.IsVisible || !endEllipseItem.IsVisible)
        //        {
        //            return;
        //        }

        //        if (pathItem == null)
        //        {
        //            pathItem = new PathItem(EditorParent);
        //            if (DataContext is not WorkflowItem)
        //            {
        //                pathItem.Content = DataContext;
        //                pathItem.ContentTemplate = EditorParent.PathTemplate;
        //                pathItem.ContentTemplateSelector = EditorParent.PathTemplateSelector;
        //            }
        //            EditorParent.Children.Add(pathItem);
        //            startEllipseItem.PathItem = pathItem;
        //            endEllipseItem.PathItem = pathItem;
        //            pathItem.StartEllipseItem = startEllipseItem;
        //            pathItem.EndEllipseItem = endEllipseItem;
        //        }
        //        pathItem.UpdateBezierCurve(pathItem.StartEllipseItem.GetPoint(EditorParent), pathItem.EndEllipseItem.GetPoint(EditorParent));
        //    }

    }
}
