using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.XPath;

namespace RPA.Views.FlowEditor
{
    /// <summary>
    /// 顶点对象
    /// </summary>
    internal class EllipseItem : Control
    {
        internal static readonly DependencyProperty DockProperty =
          DependencyProperty.Register(nameof(Dock), typeof(Dock), typeof(EllipseItem));

        internal static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(EllipseItem), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        internal Dock Dock
        {
            get => (Dock)GetValue(DockProperty);
            set => SetValue(DockProperty, value);
        }

        [TypeConverter(typeof(LengthConverter))]
        internal double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        internal WorkflowItem WorkflowParent { get; set; }

        internal PathItem PathItem { get; set; }

        internal Point GetPoint(UIElement parent)
        {
            return TranslatePoint(new Point(ActualWidth / 2, ActualHeight / 2), parent);
        }

        internal void RemoveStep()
        {
            if (Dock == Dock.Right)
            {
                WorkflowParent.FirstOrDefault(WorkflowParent.JumpStep).FromStep = null;
                WorkflowParent.JumpStep = null;
            }
            else if (Dock == Dock.Bottom)
            {
                //var w = WorkflowParent.FirstOrDefault(WorkflowParent.NextStep);
                WorkflowParent.FirstOrDefault(WorkflowParent.NextStep).LastStep = null;
                WorkflowParent.NextStep = null;
            }
        }
    }
}
