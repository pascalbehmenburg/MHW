using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace MHW_Save_Editor.Support
{
    public class RowToIndexConverter : MarkupExtension, IValueConverter
        {
            static RowToIndexConverter converter;
    
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                DataGridRow row = value as DataGridRow;
                if (row != null)
                    return row.GetIndex()+1;
                else
                    return -1;
            }
    
            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
    
            public override object ProvideValue(IServiceProvider serviceProvider)
            {
                if (converter == null) converter = new RowToIndexConverter();
                return converter;
            }
    
            public RowToIndexConverter()
            {
            }
        }
}