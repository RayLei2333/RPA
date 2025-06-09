using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows;
using System.Xml.XPath;

namespace RPA.Views.FlowEditor
{
    public class WorkflowItem : BaseFlowNode
    {
        public WorkflowItem()
        {
            DependencyPropertyDescriptor property = DependencyPropertyDescriptor.FromProperty(IsKeyboardFocusWithinProperty, typeof(WorkflowItem));
            //property?.RemoveValueChanged(this, OnIsKeyboardFocusWithinChanged);
            property?.AddValueChanged(this, OnIsKeyboardFocusWithinChanged);
        }

        public static readonly DependencyProperty IsDraggableProperty =
            DependencyProperty.Register(nameof(IsDraggable), typeof(bool), typeof(WorkflowItem), new PropertyMetadata(true));

        public static readonly DependencyProperty LastStepProperty =
            DependencyProperty.Register(nameof(LastStep), typeof(object), typeof(WorkflowItem), new PropertyMetadata(OnWorkflowItemChanged));

        public static readonly DependencyProperty FromStepProperty =
            DependencyProperty.Register(nameof(FromStep), typeof(object), typeof(WorkflowItem), new PropertyMetadata(OnWorkflowItemChanged));

        public static readonly DependencyProperty JumpStepProperty =
            DependencyProperty.Register(nameof(JumpStep), typeof(object), typeof(WorkflowItem), new PropertyMetadata(OnWorkflowItemChanged));

        public static readonly DependencyProperty NextStepProperty =
            DependencyProperty.Register(nameof(NextStep), typeof(object), typeof(WorkflowItem), new PropertyMetadata(OnWorkflowItemChanged));

        //public static readonly DependencyProperty StepTypeProperty =
        //    DependencyProperty.Register(nameof(StepType), typeof(StepType), typeof(WorkflowItem), new PropertyMetadata(StepType.Nomal));

        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register(nameof(Geometry), typeof(Geometry), typeof(WorkflowItem));

        private Thumb PART_Thumb;
        private EllipseItem EllipseLeft;
        private EllipseItem EllipseTop;
        private EllipseItem EllipseRight;
        private EllipseItem EllipseBottom;

        internal Point MouseDownControlPoint { get; set; }

        internal Dictionary<Dock, EllipseItem> EllipseItems { get; private set; }

        public bool IsInit { get; private set; }

        public WorkflowEditor EditorParent { get; internal set; }

        public bool IsDraggable
        {
            get => (bool)GetValue(IsDraggableProperty);
            set => SetValue(IsDraggableProperty, value);
        }

        public object LastStep
        {
            get => GetValue(LastStepProperty);
            set => SetValue(LastStepProperty, value);
        }

        public object FromStep
        {
            get => GetValue(FromStepProperty);
            set => SetValue(FromStepProperty, value);
        }

        public object JumpStep
        {
            get => GetValue(JumpStepProperty);
            set => SetValue(JumpStepProperty, value);
        }

        public object NextStep
        {
            get => GetValue(NextStepProperty);
            set => SetValue(NextStepProperty, value);
        }

        //public StepType StepType
        //{
        //    get => (StepType)GetValue(StepTypeProperty);
        //    set => SetValue(StepTypeProperty, value);
        //}

        public Geometry Geometry
        {
            get => (Geometry)GetValue(GeometryProperty);
            set => SetValue(GeometryProperty, value);
        }

        public override void OnApplyTemplate()
        {
            if (PART_Thumb != null)
            {
                PART_Thumb.DragDelta -= Thumb_DragDelta;
                PART_Thumb.DragCompleted -= PART_Thumb_DragCompleted;
            }
            base.OnApplyTemplate();
            EllipseLeft = GetTemplateChild("EllipseLeft") as EllipseItem;
            EllipseTop = GetTemplateChild("EllipseTop") as EllipseItem;
            EllipseRight = GetTemplateChild("EllipseRight") as EllipseItem;
            EllipseBottom = GetTemplateChild("EllipseBottom") as EllipseItem;
            EllipseItems = new Dictionary<Dock, EllipseItem>();
            PART_Thumb = GetTemplateChild("PART_Thumb") as Thumb;
            PART_Thumb.DragDelta += Thumb_DragDelta;
            PART_Thumb.DragCompleted += PART_Thumb_DragCompleted;

            EllipseItems.Add(EllipseLeft.Dock, EllipseLeft);
            EllipseItems.Add(EllipseTop.Dock, EllipseTop);
            EllipseItems.Add(EllipseRight.Dock, EllipseRight);
            EllipseItems.Add(EllipseBottom.Dock, EllipseBottom);
            foreach (var item in EllipseItems.Values)
            {
                item.WorkflowParent = this;
            }
            IsInit = true;
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Width + e.HorizontalChange > 0)
            {
                Width += e.HorizontalChange;
            }
            if (Height + e.VerticalChange > 0)
            {
                Height += e.VerticalChange;
            }
            UpdateCurve();
        }

        private void PART_Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            var cellSize = EditorParent.GridSize;
            Width = Math.Max(cellSize, Width.Adsorb(cellSize));
            Height = Math.Max(cellSize, Height.Adsorb(cellSize));
            Dispatcher.InvokeAsync(() =>
            {
                UpdateCurve();
            }, DispatcherPriority.Render);
        }

        private void OnIsKeyboardFocusWithinChanged(object sender, EventArgs e)
        {
            if (IsKeyboardFocusWithin)
            {
                EditorParent.BeginUpdateSelectedItems();
                IsSelected = true;
                //if (!EditorParent.IsCtrlKeyDown)
                //{
                //    foreach (var item in EditorParent.SelectableElements.Where(x => x != this))
                //    {
                //        item.IsSelected = false;
                //    }
                //}
                EditorParent.EndUpdateSelectedItems();
            }
        }

        internal void Delete()
        {
            if (IsInit)
            {
                foreach (var item in EllipseItems.Values)
                {
                    item.PathItem?.Delete();
                }
            }
            else
            {
                if (LastStep != null)
                {
                    FirstOrDefault(LastStep).NextStep = null;
                    LastStep = null;
                }
                if (NextStep != null)
                {
                    FirstOrDefault(NextStep).LastStep = null;
                    NextStep = null;
                }
                if (FromStep != null)
                {
                    FirstOrDefault(FromStep).JumpStep = null;
                    FromStep = null;
                }
                if (JumpStep != null)
                {
                    FirstOrDefault(JumpStep).FromStep = null;
                    JumpStep = null;
                }
            }
            BindingOperations.ClearAllBindings(this);
            EditorParent.Children.Remove(this);
        }

        private static void OnWorkflowItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WorkflowItem)d).OnWorkflowItemChanged(e);
        }

        protected virtual void OnWorkflowItemChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateCurve();
        }

        internal WorkflowItem FirstOrDefault(object item)
        {
            return null;//EditorParent.FirstOrDefault(item);
        }

        public void UpdateCurve()
        {
            if (!IsInit)
            {
                Loaded += WorkflowItem_Loaded;
                return;
            }
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

        private void WorkflowItem_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= WorkflowItem_Loaded;
            UpdateCurve();
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
                if (DataContext is not WorkflowItem)
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
    }
}
