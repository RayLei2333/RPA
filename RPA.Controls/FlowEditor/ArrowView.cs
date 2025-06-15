using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RPA.Controls.FlowEditor
{
    public class ArrowView : Shape
    {
        #region Dependency Properties

        public static readonly DependencyProperty X1Property = DependencyProperty.Register("X1", typeof(double), typeof(ArrowView), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty Y1Property = DependencyProperty.Register("Y1", typeof(double), typeof(ArrowView), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty X2Property = DependencyProperty.Register("X2", typeof(double), typeof(ArrowView), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty Y2Property = DependencyProperty.Register("Y2", typeof(double), typeof(ArrowView), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty HeadWidthProperty = DependencyProperty.Register("HeadWidth", typeof(double), typeof(ArrowView), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty HeadHeightProperty = DependencyProperty.Register("HeadHeight", typeof(double), typeof(ArrowView), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        #endregion

        #region CLR Properties

        [TypeConverter(typeof(LengthConverter))]
        public double X1
        {
            get { return (double)base.GetValue(X1Property); }
            set { base.SetValue(X1Property, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double Y1
        {
            get { return (double)base.GetValue(Y1Property); }
            set { base.SetValue(Y1Property, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double X2
        {
            get { return (double)base.GetValue(X2Property); }
            set { base.SetValue(X2Property, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double Y2
        {
            get { return (double)base.GetValue(Y2Property); }
            set { base.SetValue(Y2Property, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double HeadWidth
        {
            get { return (double)base.GetValue(HeadWidthProperty); }
            set { base.SetValue(HeadWidthProperty, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double HeadHeight
        {
            get { return (double)base.GetValue(HeadHeightProperty); }
            set { base.SetValue(HeadHeightProperty, value); }
        }

        private int LineType = 0;

        public WorkflowItem FromNode;

        public WorkflowItem ToNode;
        #endregion

        #region Overrides

        protected override Geometry DefiningGeometry
        {
            get
            {
                // Create a StreamGeometry for describing the shape
                StreamGeometry geometry = new StreamGeometry();
                geometry.FillRule = FillRule.EvenOdd;

                using (StreamGeometryContext context = geometry.Open())
                {
                    InternalDrawArrowGeometry(context);
                }

                // Freeze the geometry for performance benefits
                geometry.Freeze();

                return geometry;
            }
        }

        #endregion

        #region Privates

        private void InternalDrawArrowGeometry(StreamGeometryContext context)
        {

            switch (LineType)
            {
                case 0:
                    {
                        DrawLineArrow(context);
                    }
                    break;
                default:
                    {
                        DrawFoldLineArrow(context, LineType);
                    }
                    break;
            }
        }

        private void DrawLineArrow(StreamGeometryContext context)
        {
            double theta = Math.Atan2(Y1 - Y2, X1 - X2);
            double sint = Math.Sin(theta);
            double cost = Math.Cos(theta);

            Point pt1 = new Point(X1, this.Y1);
            Point pt2 = new Point(X2, this.Y2);

            Point pt3 = new Point(
                X2 + (HeadWidth * cost - HeadHeight * sint),
                Y2 + (HeadWidth * sint + HeadHeight * cost));

            Point pt4 = new Point(
                X2 + (HeadWidth * cost + HeadHeight * sint),
                Y2 - (HeadHeight * cost - HeadWidth * sint));
            context.BeginFigure(pt1, true, false);
            context.LineTo(pt2, true, true);
            context.LineTo(pt3, true, true);
            context.LineTo(pt2, true, true);
            context.LineTo(pt4, true, true);
        }

        private void DrawFoldLineArrow(StreamGeometryContext context, int lineStyle)
        {
            Point pt5;
            Point pt6;
            if (lineStyle == 1)
            {
                pt5 = new Point(X1, Y1 + HeadHeight + 5);
                pt6 = new Point(X2, Y1 + HeadHeight + 5);
            }
            else
            {
                pt5 = new Point(X1, Y2 - HeadHeight - 5);
                pt6 = new Point(X2, Y2 - HeadHeight - 5);
            }
            double theta = Math.Atan2(pt6.Y - Y2, pt6.X - X2);
            double sint = Math.Sin(theta);
            double cost = Math.Cos(theta);
            Point pt1 = new Point(X1, this.Y1);
            Point pt2 = new Point(X2, this.Y2);

            Point pt3 = new Point(
                X2 + (HeadWidth * cost - HeadHeight * sint),
                Y2 + (HeadWidth * sint + HeadHeight * cost));

            Point pt4 = new Point(
                X2 + (HeadWidth * cost + HeadHeight * sint),
                Y2 - (HeadHeight * cost - HeadWidth * sint));
            context.BeginFigure(pt1, true, false);
            context.LineTo(pt5, true, true);
            context.LineTo(pt6, true, true);
            context.LineTo(pt2, true, true);
            context.LineTo(pt3, true, true);
            context.LineTo(pt2, true, true);
            context.LineTo(pt4, true, true);
        }

        #endregion
        public void SetLineStyle(int lineType)
        {
            LineType = lineType;
        }

        public bool Contain(Point point)
        {
            switch (LineType)
            {
                case 0:
                    double centerX = X1 + (X2 - X1) / 2;
                    double centerY = Y1 + (Y2 - Y1) / 2;
                    var distance = Math.Sqrt((Math.Pow(point.X - centerX, 2) + Math.Pow(point.Y - centerY, 2)));
                    return distance < 30;
                case 1:
                    if (pointToLine(X1, Y1 + HeadHeight + 5, X2, Y1 + HeadHeight + 5, point.X, point.Y) < 10 || pointToLine(X2, Y1 + HeadHeight + 5, X2, Y2 - HeadHeight - 5, point.X, point.Y) < 10)
                    {
                        return true;
                    }
                    return false;
                case 2:
                    if (pointToLine(X1, Y2 - HeadHeight - 5, X2, Y2 - HeadHeight - 5, point.X, point.Y) < 10 || pointToLine(X1, Y1 + HeadHeight + 5, X1, Y2 - HeadHeight - 5, point.X, point.Y) < 10)
                    {
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }


        // 计算两点之间的距离    
        private double lineSpace(double x1, double y1, double x2, double y2)
        {
            double lineLength = 0;
            lineLength = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));

            return lineLength;

        }
        //点到线段距离  
        private double pointToLine(double x1, double y1, double x2, double y2, double x0, double y0)
        {
            double space = 0;
            double a, b, c;
            a = lineSpace(x1, y1, x2, y2);// 线段的长度    
            b = lineSpace(x1, y1, x0, y0);// (x1,y1)到点的距离    
            c = lineSpace(x2, y2, x0, y0);// (x2,y2)到点的距离    
            if (c <= 0.000001 || b <= 0.000001)
            {
                space = 0;
                return space;
            }
            if (a <= 0.000001)
            {
                space = b;
                return space;
            }
            if (c * c >= a * a + b * b)
            {
                space = b;
                return space;
            }
            if (b * b >= a * a + c * c)
            {
                space = c;
                return space;
            }
            double p = (a + b + c) / 2;// 半周长    
            double s = Math.Sqrt(p * (p - a) * (p - b) * (p - c));// 海伦公式求面积    
            space = 2 * s / a;// 返回点到线的距离（利用三角形面积公式求高）    
            return space;
        }
    }
}
