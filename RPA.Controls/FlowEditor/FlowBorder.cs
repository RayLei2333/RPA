using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RPA.Controls.FlowEditor
{
    internal class FlowBorder : Border
    {
        public static readonly DependencyProperty ShapeTypeProperty =
            DependencyProperty.Register(nameof(ShapeType), typeof(ShapeType), typeof(FlowBorder));

        public static readonly DependencyProperty FlowNodeTypeProperty =
            DependencyProperty.Register(nameof(FlowNodeType), typeof(FlowNodeType), typeof(FlowBorder));

        public ShapeType ShapeType
        {
            get => (ShapeType)GetValue(ShapeTypeProperty);
            set => SetValue(ShapeTypeProperty, value);
        }

        public FlowNodeType FlowNodeType
        {
            get => (FlowNodeType)GetValue(FlowNodeTypeProperty);
            set => SetValue(FlowNodeTypeProperty, value);
        }


        protected override void OnRender(DrawingContext dc)
        {
            //base.OnRender(dc);
            var pen = new Pen(BorderBrush, new double[] { BorderThickness.Left, BorderThickness.Top, BorderThickness.Right, BorderThickness.Bottom }.Max());

            double num = pen.Thickness * 0.5;
            Rect rect = new Rect(new Point(num, num), new Point(ActualWidth - num, ActualHeight - num));

            switch (ShapeType)
            {
                case ShapeType.Circle:
                case ShapeType.Rectangle:
                case ShapeType.Ellipse:
                    base.OnRender(dc);
                    break;

                case ShapeType.Diamond:

                    DrawPolygon(dc, Background, pen, new Point(ActualWidth / 2, num), new Point(ActualWidth - num, ActualHeight / 2), new Point(ActualWidth / 2, ActualHeight - num), new Point(num, ActualHeight / 2));
                    break;
            }
        }

        //矩形和圆形
        private void DrawRoundedRectangle(DrawingContext dc, Brush brush, Pen pen, Rect rect)
        {
            var geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open())
            {
                var minX = rect.Width / 2;
                var minY = rect.Height / 2;
                //CornerRadius cornerRadius = new CornerRadius(CornerRadius);
                //cornerRadius.TopLeft = Math.Min(Math.Min(cornerRadius.TopLeft, minX), minY);
                //cornerRadius.TopRight = Math.Min(Math.Min(cornerRadius.TopRight, minX), minY);
                //cornerRadius.BottomRight = Math.Min(Math.Min(cornerRadius.BottomRight, minX), minY);
                //cornerRadius.BottomLeft = Math.Min(Math.Min(cornerRadius.BottomLeft, minX), minY);

                ctx.BeginFigure(new Point(rect.Left + CornerRadius.TopLeft, rect.Top), true, true);

                // Top line and top-right corner
                ctx.LineTo(new Point(rect.Right - CornerRadius.TopRight, rect.Top), true, false);
                ctx.ArcTo(new Point(rect.Right, rect.Top + CornerRadius.TopRight), new Size(CornerRadius.TopRight, CornerRadius.TopRight), 0, false, SweepDirection.Clockwise, true, false);

                // Right line and bottom-right corner
                ctx.LineTo(new Point(rect.Right, rect.Bottom - CornerRadius.BottomRight), true, false);
                ctx.ArcTo(new Point(rect.Right - CornerRadius.BottomRight, rect.Bottom), new Size(CornerRadius.BottomRight, CornerRadius.BottomRight), 0, false, SweepDirection.Clockwise, true, false);

                // Bottom line and bottom-left corner
                ctx.LineTo(new Point(rect.Left + CornerRadius.BottomLeft, rect.Bottom), true, false);
                ctx.ArcTo(new Point(rect.Left, rect.Bottom - CornerRadius.BottomLeft), new Size(CornerRadius.BottomLeft, CornerRadius.BottomLeft), 0, false, SweepDirection.Clockwise, true, false);

                // Left line and top-left corner
                ctx.LineTo(new Point(rect.Left, rect.Top + CornerRadius.TopLeft), true, false);
                ctx.ArcTo(new Point(rect.Left + CornerRadius.TopLeft, rect.Top), new Size(CornerRadius.TopLeft, CornerRadius.TopLeft), 0, false, SweepDirection.Clockwise, true, false);
            }
            geometry.Freeze();
            //Geometry = geometry;
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
            dc.DrawGeometry(brush, pen, geometry);
        }
    }
}
