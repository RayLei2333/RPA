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


    }
}
