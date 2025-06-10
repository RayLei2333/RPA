using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RPA.Views.FlowEditor.Converters
{
    internal class EllipseItemVisibilityConverter: IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length > 1)
            {
                //Dock Dock = (Dock)values[0];
                //StepType stepType = (StepType)values[1];
                return Visibility.Visible;
                //if (stepType == StepType.Begin)
                //{
                //    return GetVisibility(Dock is Dock.Bottom);
                //}
                //else if (stepType == StepType.End)
                //{
                //    return GetVisibility(Dock is Dock.Top or Dock.Left);
                //}
                //else
                //{
                //    return Visibility.Visible;
                //}
            }
            return Binding.DoNothing;
        }

        private static Visibility GetVisibility(bool value)
        {
            return value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
