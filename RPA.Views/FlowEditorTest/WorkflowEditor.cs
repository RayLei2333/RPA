using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RPA.Views.FlowEditor
{
    public class WorkflowEditor : Canvas
    {
        #region DependencyProperty

        public static readonly DependencyProperty ScaleProperty =
          DependencyProperty.Register(nameof(Scale), typeof(double), typeof(WorkflowEditor), new PropertyMetadata(1d, (sender, e) =>
          {
              WorkflowEditor workflowEditor = (WorkflowEditor)sender;
              if (workflowEditor.Scale < 0.2)
              {
                  workflowEditor.Scale = 0.2;
              }
              if (workflowEditor.Scale > 3)
              {
                  workflowEditor.Scale = 3;
              }
          }));

        public static readonly DependencyProperty SelectedItemsProperty =
         DependencyProperty.Register(nameof(SelectedItems), typeof(IList), typeof(WorkflowEditor), new FrameworkPropertyMetadata(default(IList), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty GridSizeProperty =
         DependencyProperty.Register(nameof(GridSize), typeof(double), typeof(WorkflowEditor),
             new FrameworkPropertyMetadata(20d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty PathTemplateProperty =
         DependencyProperty.Register(nameof(PathTemplate), typeof(DataTemplate), typeof(WorkflowEditor));

        public static readonly DependencyProperty PathTemplateSelectorProperty =
            DependencyProperty.Register(nameof(PathTemplateSelector), typeof(DataTemplateSelector), typeof(WorkflowEditor));

        public static readonly DependencyProperty MousePositionProperty =
         DependencyProperty.Register(nameof(MousePosition), typeof(Point), typeof(WorkflowEditor));

        public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(WorkflowEditor), new PropertyMetadata(OnItemsSourceChanged));

        #endregion

        //鼠标选中多个元素的Rectangle遮罩
        private Rectangle _multiSelectionMask;
        //跟随鼠标运移动绘制的多选Rectangle区域
        private Rectangle _selectionArea;
        //鼠标选择多个元素起始的位置
        private Point _selectionStartPoint;

        //鼠标按下的位置
        private Point _mouseDownPoint;
        //鼠标按下控件的位置
        private Point _mouseDownControlPoint;
        //多个选中时鼠标按下的位置
        private Point _multiMouseDownPoint;
        //多个选中时Rectangle遮罩按下的位置
        private Point _multiMouseDownRectanglePoint;

        private bool _isUpdatingSelectedItems;

        private PathItem _currentPath;
        private FlowNode _lastWorkflowItem;
        private EllipseItem _lastEllipseItem;

        private Point _pathStartPoint;

        /// <summary>
        /// 所有节点集合
        /// </summary>
        public IEnumerable<FlowNode> WorkflowItems => Children.OfType<FlowNode>();

        /// <summary>
        /// 缩放
        /// </summary>
        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        /// <summary>
        /// 选中项集合
        /// </summary>
        public IList SelectedItems
        {
            get => (IList)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        private EditorStatus _editorStatus;
        internal EditorStatus EditorStatus
        {
            get => _editorStatus;
            set
            {
                if (_editorStatus == value)
                {
                    return;
                }
                // SetEditorStatus(this, value);
                _editorStatus = value;
            }
        }

        public DataTemplate PathTemplate
        {
            get => (DataTemplate)GetValue(PathTemplateProperty);
            set => SetValue(PathTemplateProperty, value);
        }

        public DataTemplateSelector PathTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(PathTemplateSelectorProperty);
            set => SetValue(PathTemplateSelectorProperty, value);
        }

        public double GridSize
        {
            get => (double)GetValue(GridSizeProperty);
            set => SetValue(GridSizeProperty, value);
        }

        public Point MousePosition
        {
            get => (Point)GetValue(MousePositionProperty);
            set => SetValue(MousePositionProperty, value);
        }

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public WorkflowEditor()
        {
            MouseLeftButtonDown += WorkflowEditor_MouseLeftButtonDown;
            PreviewMouseLeftButtonUp += WorkflowEditor_MouseLeftButtonUp;
            MouseMove += WorkflowEditor_MouseMove;
            //MouseWheel += WorkflowEditor_MouseWheel;
            //KeyDown += WorkflowEditor_KeyDown;


            _multiSelectionMask = new Rectangle();
            _multiSelectionMask.Fill = "#44AACCEE".ToSolidColorBrush();
            _multiSelectionMask.Stroke = "#FF0F80D9".ToSolidColorBrush();
            _multiSelectionMask.StrokeDashArray = new DoubleCollection(new double[] { 2, 2 });
            _multiSelectionMask.MouseLeftButtonDown += MultiSelectionRectangle_MouseLeftButtonDown;
            SetZIndex(_multiSelectionMask, int.MaxValue - 1);

            _selectionArea = new Rectangle();
            _selectionArea.Fill = "#88AACCEE".ToSolidColorBrush();
            _selectionArea.Stroke = "#FF0F80D9".ToSolidColorBrush();
            SetZIndex(_selectionArea, int.MaxValue);

            ScaleTransform scaleTransform = new ScaleTransform();
            BindingOperations.SetBinding(scaleTransform, ScaleTransform.ScaleXProperty, new Binding() { Source = this, Path = new PropertyPath(ScaleProperty) });
            BindingOperations.SetBinding(scaleTransform, ScaleTransform.ScaleYProperty, new Binding() { Source = this, Path = new PropertyPath(ScaleProperty) });
            LayoutTransform = scaleTransform;
        }

        #region Event
        private void WorkflowEditor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (EditorStatus is EditorStatus.MultiMoving)
            {
                return;
            }
            Point point = e.GetPosition(this);
            Children.Remove(_multiSelectionMask);

            //IEnumerable<SelectionControl> selectableElements;
            //BeginUpdateSelectedItems();
            //if (this.GetVisualHit<SelectionControl>(point) is SelectionControl selectableElement)
            //{
            //    selectableElement.IsSelected = true;
            //    selectableElements = SelectableElements.Where(x => x != selectableElement);
            //}
            //else
            //{
            //    selectableElements = SelectableElements;
            //}
            //if (!IsCtrlKeyDown)
            //{
            //    foreach (var item in selectableElements)
            //    {
            //        item.IsSelected = false;
            //    }
            //}
            EndUpdateSelectedItems();

            Focus();
            if (EditorStatus is EditorStatus.Moving or EditorStatus.Drawing /*|| isPathItem*/)
            {
                return;
            }

            if (!Children.Contains(_selectionArea))
            {
                Children.Add(_selectionArea);
            }

            _selectionStartPoint = point;
            EditorStatus = EditorStatus.Selecting;

            // Reset the selection rectangle
            _selectionArea.Width = 0;
            _selectionArea.Height = 0;
            SetLeft(_selectionArea, _selectionStartPoint.X);
            SetTop(_selectionArea, _selectionStartPoint.Y);
        }

        private void WorkflowEditor_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (EditorStatus == EditorStatus.Drawing)
                {
                    Point point = e.GetPosition(this);
                    var endEllipseItem = GetEllipseWithPoint(point);
                    var node = GetNodeWithPoint(point);
                    if (endEllipseItem != null)
                    {
                        SetStep(_lastWorkflowItem.DataContext, endEllipseItem.WorkflowParent.DataContext, _lastEllipseItem, endEllipseItem);
                    }
                }
                else if (EditorStatus == EditorStatus.Moving)
                {
                    var workflowItem = _lastWorkflowItem;
                    if (workflowItem == null || !workflowItem.IsDraggable)
                    {
                        return;
                    }

                    PositionWorkflowItem(workflowItem);
                }
                else if (EditorStatus == EditorStatus.MultiMoving)
                {
                    SetLeft(_multiSelectionMask, Adsorb(GetLeft(_multiSelectionMask)));
                    SetTop(_multiSelectionMask, Adsorb(GetTop(_multiSelectionMask)));
                    foreach (var item in WorkflowItems.Where(x => x.IsSelected && x.IsDraggable))
                    {
                        PositionWorkflowItem(item);
                    }
                }
            }
            finally
            {
                Cursor = null;
                _multiSelectionMask.Cursor = null;
                Children.Remove(_selectionArea);
                Children.Remove(_currentPath);
                _currentPath = null;
                EditorStatus = EditorStatus.None;
            }
        }
        internal void SetStep(object fromStep, object toStep, EllipseItem fromEllipse, EllipseItem toEllipse)
        {
            if (fromEllipse == toEllipse)
            {
                return;
            }
            if (fromStep == toStep)
            {
                return;
            }
            else if (fromEllipse.PathItem != null || toEllipse.PathItem != null)
            {
                return;
            }
            else if (fromEllipse.Dock == Dock.Left || toEllipse.Dock == Dock.Right)
            {
                return;
            }
            else if (fromEllipse.Dock == Dock.Top || toEllipse.Dock == Dock.Bottom)
            {
                return;
            }
            else if (fromEllipse.Dock == Dock.Right && toEllipse.Dock == Dock.Top)
            {
                return;
            }
            else if (fromEllipse.Dock == Dock.Bottom && toEllipse.Dock == Dock.Left)
            {
                return;
            }
            else
            {
                var fromWorkflow = FirstOrDefault(fromStep);
                var toWorkflow = FirstOrDefault(toStep);
                if (fromEllipse.Dock == Dock.Right)
                {
                    fromWorkflow.JumpStep = toStep;
                    toWorkflow.FromStep = fromStep;
                }
                else if (fromEllipse.Dock == Dock.Bottom)
                {
                    fromWorkflow.NextStep = toStep;
                    toWorkflow.LastStep = fromStep;
                }
                fromWorkflow.UpdateCurve();
                toWorkflow.UpdateCurve();
            }
        }


        private void WorkflowEditor_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(this);
            MousePosition = point;
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            if (EditorStatus == EditorStatus.MultiMoving)
            {
                Vector vector = point - _multiMouseDownPoint;

                SetLeft(_multiSelectionMask, _multiMouseDownRectanglePoint.X + vector.X);
                SetTop(_multiSelectionMask, _multiMouseDownRectanglePoint.Y + vector.Y);
                if (GetLeft(_multiSelectionMask) < 0)
                {
                    SetLeft(_multiSelectionMask, 0);
                }
                if (GetTop(_multiSelectionMask) < 0)
                {
                    SetTop(_multiSelectionMask, 0);
                }
                foreach (var item in WorkflowItems.Where(x => x.IsSelected && x.IsDraggable))
                {
                    var minX = item.MouseDownControlPoint.X - _multiMouseDownRectanglePoint.X;
                    var minY = item.MouseDownControlPoint.Y - _multiMouseDownRectanglePoint.Y;
                    SetLeft(item, item.MouseDownControlPoint.X + vector.X);
                    SetTop(item, item.MouseDownControlPoint.Y + vector.Y);
                    if (GetLeft(item) < minX)
                    {
                        SetLeft(item, minX);
                    }
                    if (GetTop(item) < minY)
                    {
                        SetTop(item, minY); 
                    }
                    item.UpdateCurve();
                    if (GetLeft(item) > ActualWidth - item.ActualWidth)
                    {
                        SetLeft(item, ActualWidth - item.ActualWidth);
                    }
                    if (GetTop(item) > ActualHeight - item.ActualHeight)
                    {
                        SetTop(item, ActualHeight - item.ActualHeight);
                    }
                }
            }
            else if (EditorStatus == EditorStatus.Selecting)
            {
                double x = Math.Min(point.X, _selectionStartPoint.X);
                double y = Math.Min(point.Y, _selectionStartPoint.Y);

                double width = Math.Abs(point.X - _selectionStartPoint.X);
                double height = Math.Abs(point.Y - _selectionStartPoint.Y);

                SetLeft(_selectionArea, x);
                SetTop(_selectionArea, y);
                _selectionArea.Width = width;
                _selectionArea.Height = height;

                if (width >= 1 || height >= 1)
                {
                    Rect selectedArea = new Rect(x, y, width, height);
                    RectangleGeometry rectangleGeometry = new RectangleGeometry(selectedArea);
                    BeginUpdateSelectedItems();
                    foreach (var item in WorkflowItems)
                    {
                        item.IsSelected = CheckOverlap(rectangleGeometry, item);
                    }
                    EndUpdateSelectedItems();
                }
            }
            else if (EditorStatus == EditorStatus.Drawing)
            {
                _currentPath.UpdateBezierCurve(_pathStartPoint, point);
            }
            else if (EditorStatus == EditorStatus.Moving)
            {
                var workflowItem = _lastWorkflowItem;
                if (!workflowItem.IsDraggable)
                {
                    return;
                }
                Vector vector = point - _mouseDownPoint;
                SetLeft(workflowItem, Math.Round(_mouseDownControlPoint.X + vector.X, 0));
                SetTop(workflowItem, Math.Round(_mouseDownControlPoint.Y + vector.Y, 0));

                if (GetLeft(workflowItem) < 0)
                {
                    SetLeft(workflowItem, 0);
                }
                if (GetTop(workflowItem) < 0)
                {
                    SetTop(workflowItem, 0);
                }

                if (GetLeft(workflowItem) > ActualWidth - workflowItem.ActualWidth)
                {
                    SetLeft(workflowItem, ActualWidth - workflowItem.ActualWidth);
                }
                if (GetTop(workflowItem) > ActualHeight - workflowItem.ActualHeight)
                {
                    SetTop(workflowItem, ActualHeight - workflowItem.ActualHeight);
                }
                workflowItem.UpdateCurve();
            }
        }


        private void MultiSelectionRectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //if (CanvasStatus is CanvasStatus.Moving or CanvasStatus.Drawing or CanvasStatus.Selecting)
            //{
            //    return;
            //}

            EditorStatus = EditorStatus.MultiMoving;
            _multiSelectionMask.Cursor = Cursors.ScrollAll;
            _multiMouseDownPoint = e.GetPosition(this);
            _multiMouseDownRectanglePoint = new Point(GetLeft(_multiSelectionMask), GetTop(_multiSelectionMask));
            foreach (var item in WorkflowItems.Where(x => x.IsSelected))
            {
                item.MouseDownControlPoint = new Point(GetLeft(item), GetTop(item));
            }
        }

        #endregion

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            UpdateSelectedItems();
            if (visualAdded is FlowNode added)
            {
                SetLeft(added, Adsorb(GetLeft(added)));
                SetTop(added, Adsorb(GetTop(added)));

                //added.EditorParent = this;
                added.MouseLeftButtonDown += WorkflowItem_MouseLeftButtonDown;
                added.Selected += WorkflowItem_SelectedChanged;
                added.Unselected += WorkflowItem_SelectedChanged;
            }
            if (visualRemoved is FlowNode removed)
            {
                //removed.EditorParent = null;
                removed.MouseLeftButtonDown -= WorkflowItem_MouseLeftButtonDown;
                removed.Selected -= WorkflowItem_SelectedChanged;
                removed.Unselected -= WorkflowItem_SelectedChanged;
            }
        }

        private void WorkflowItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var workflowItem = sender as FlowNode;
            _lastWorkflowItem = workflowItem;
            Point point = e.GetPosition(this);
            var startEllipseItem = GetEllipseWithPoint(point);
            if (startEllipseItem == null)
            {
                EditorStatus = EditorStatus.Moving;
            }
            else
            {
                //_lastWorkflowItem = startEllipseItem.WorkflowParent;
                _pathStartPoint = startEllipseItem.GetPoint(this);
                _lastEllipseItem = startEllipseItem;
                EditorStatus = EditorStatus.Drawing;

                if (_currentPath == null)
                {
                    _currentPath = new PathItem(this);
                    Children.Add(_currentPath);
                }
            }

            if ((!workflowItem.IsDraggable && startEllipseItem == null) /*|| CanvasStatus is CanvasStatus.Selecting or CanvasStatus.MultiMoving*/)
            {
                return;
            }

            Cursor = EditorStatus == EditorStatus.Drawing ? Cursors.Cross : Cursors.ScrollAll;
            _mouseDownPoint = point;
            _mouseDownControlPoint = new Point(GetLeft(workflowItem), GetTop(workflowItem));
        }

        private void WorkflowItem_SelectedChanged(object sender, RoutedEventArgs e)
        {
            if (!_isUpdatingSelectedItems)
            {
                UpdateSelectedItems();
            }
        }

        private EllipseItem GetEllipseWithPoint(Point point)
        {
            var ellipseItem = this.GetVisualHit<EllipseItem>(point);
            if (ellipseItem != null && !ellipseItem.IsVisible)
            {
                return null;
            }
            return ellipseItem;
        }

        private SimplePanel GetNodeWithPoint(Point point)
        {
            return this.GetVisualHit<SimplePanel>(point);
        }

        //protected override void OnRender(DrawingContext dc)
        //{
        //    base.OnRender(dc);

        //    double width = ActualWidth;
        //    double height = ActualHeight;
        //    double gridSize = GridSize;

        //    Pen pen = new Pen(LineBrush, 0.4);
        //    Pen sidePen = new Pen(LineBrush, 1);

        //    int index = 0;
        //    for (double x = 0; x < width; x += gridSize)
        //    {
        //        dc.DrawLine(IsSide(index) ? sidePen : pen, new Point(x, 0), new Point(x, height));
        //        index++;
        //    }
        //    index = 0;
        //    for (double y = 0; y < height; y += gridSize)
        //    {
        //        dc.DrawLine(IsSide(index) ? sidePen : pen, new Point(0, y), new Point(width, y));
        //        index++;
        //    }
        //}


        public void BeginUpdateSelectedItems()
        {
            _isUpdatingSelectedItems = true;
        }

        public void EndUpdateSelectedItems()
        {
            _isUpdatingSelectedItems = false;
            UpdateSelectedItems();
        }

        internal void UpdateSelectedItems()
        {
            List<object> list = new List<object>();
            foreach (var item in WorkflowItems.Where(x => x.IsSelected))
            {
                list.Add(item.DataContext);
            }

            if (SelectedItems == null)
            {
                SelectedItems = list;
            }
            else
            {
                if (list.Count != 0 || SelectedItems.Count != 0)
                {
                    SelectedItems = list;
                }
            }

            UpdateMultiSelectionMask();
        }

        public void UpdateMultiSelectionMask()
        {
            if (SelectedItems != null && SelectedItems.Count > 1)
            {
                double minX = double.MaxValue;
                double minY = double.MaxValue;
                double maxX = double.MinValue;
                double maxY = double.MinValue;

                foreach (var item in WorkflowItems.Where(x => x.IsSelected))
                {
                    double left = GetLeft(item);
                    double top = GetTop(item);
                    double right = left + item.ActualWidth;
                    double bottom = top + item.ActualHeight;

                    minX = Math.Min(left, minX);
                    minY = Math.Min(top, minY);
                    maxX = Math.Max(right, maxX);
                    maxY = Math.Max(bottom, maxY);
                }
                _multiSelectionMask.Width = maxX - minX;
                _multiSelectionMask.Height = maxY - minY;
                SetLeft(_multiSelectionMask, minX);
                SetTop(_multiSelectionMask, minY);
                if (!Children.Contains(_multiSelectionMask))
                {
                    Children.Add(_multiSelectionMask);
                }
            }
            else
            {
                Children.Remove(_multiSelectionMask);
            }
        }

        private void PositionWorkflowItem(FlowNode workflowItem)
        {
            SetLeft(workflowItem, Adsorb(GetLeft(workflowItem)));
            SetTop(workflowItem, Adsorb(GetTop(workflowItem)));
            Dispatcher.InvokeAsync(() =>
            {
                workflowItem.UpdateCurve();
            }, DispatcherPriority.Render);
        }

        private double Adsorb(double value)
        {
            return value.Adsorb(GridSize);
        }

        public FlowNode FirstOrDefault(object item)
        {
            var flowNode = WorkflowItems.FirstOrDefault(x => x.DataContext == item);
            flowNode ??= new FlowNode();
            return flowNode;
        }

        private bool CheckOverlap(RectangleGeometry rectangleGeometry, FlowNode flowNode)
        {
            GeneralTransform transform = flowNode.TransformToVisual(this);
            Geometry geometry = flowNode.Geometry?.Clone();
            if (geometry != null)
            {
                geometry.Transform = (Transform)transform;
            }
            return rectangleGeometry.FillContainsWithDetail(geometry) != IntersectionDetail.Empty;
        }

        //private EllipseItem GetEllipseWithPoint(Point point)
        //{
        //    var ellipseItem = this.GetVisualHit<EllipseItem>(point);
        //    if (ellipseItem != null && !ellipseItem.IsVisible)
        //    {
        //        return null;
        //    }
        //    return ellipseItem;
        //}

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    Children.Add(CreateFlowNode(item));
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldItem in e.OldItems)
                {
                    var workflowItem = WorkflowItems.FirstOrDefault(x => x.DataContext == oldItem);
                    //workflowItem.Delete();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                var newItem = WorkflowItems.FirstOrDefault(x => x.DataContext == e.NewItems[0]);
                var oldItem = WorkflowItems.FirstOrDefault(x => x.DataContext == e.OldItems[0]);
                Children.Add(CreateFlowNode(newItem));
                // oldItem.Delete();
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Children.Clear();
            }
        }

        public virtual void OnItemsSourceChanged(IEnumerable oldItemsSource, IEnumerable newItemsSource)
        {
            Children.Clear();
            if (oldItemsSource != null)
            {
                if (oldItemsSource is INotifyCollectionChanged notifyCollectionChanged)
                {
                    notifyCollectionChanged.CollectionChanged -= CollectionChanged;
                }
            }
            if (newItemsSource != null)
            {
                foreach (var item in newItemsSource.OfType<object>())
                {
                    Children.Add(CreateFlowNode(item));
                }
                if (newItemsSource is INotifyCollectionChanged notifyCollectionChanged)
                {
                    notifyCollectionChanged.CollectionChanged += CollectionChanged;
                }
            }
        }


        public FlowNode CreateFlowNode(object item)
        {
            FlowNode flowNode = null;
            if (item is FlowNode node)
            {
                flowNode = node;
                flowNode.DataContext = node;
            }
            else
            {
                flowNode = new FlowNode()
                {
                    DataContext  =item,
                    Content = item
                };
            }
            //flowNode

            return flowNode;
        }


        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var workflowEditor = (WorkflowEditor)d;
            IEnumerable oldItemsSource = (IEnumerable)e.OldValue;
            IEnumerable newItemsSource = (IEnumerable)e.NewValue;
            workflowEditor.OnItemsSourceChanged(oldItemsSource, newItemsSource);
        }

    }
}
