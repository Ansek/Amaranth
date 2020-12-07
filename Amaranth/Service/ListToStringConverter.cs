using System;
using System.Windows.Data;
using System.Globalization;
using System.Collections.ObjectModel;

namespace Amaranth.Service
{
    class ListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var list = (ObservableCollection<string>)value;
            if (list != null)
            {
                string s = string.Empty;
                foreach (var str in list)
                {
                    if (s != string.Empty)
                        s += ", ";
                    s += str;
                }
                return s;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
