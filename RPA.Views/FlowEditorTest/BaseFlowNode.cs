using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RPA.Views.FlowEditor
{
    public class BaseFlowNode : ContentControl
    {
        public static readonly DependencyProperty IsSelectedProperty =
          DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(BaseFlowNode), new PropertyMetadata((sender, e) =>
          {
              var selectionControl = (BaseFlowNode)sender;
              bool newValue = (bool)e.NewValue;
              var routedEventHandler = newValue ? selectionControl.Selected : selectionControl.Unselected;
              routedEventHandler?.Invoke(selectionControl, new RoutedEventArgs());
          }));

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public event RoutedEventHandler Selected;

        public event RoutedEventHandler Unselected;
    }
}
