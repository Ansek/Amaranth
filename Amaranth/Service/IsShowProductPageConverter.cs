using System;
using System.Windows.Data;
using System.Globalization;

namespace Amaranth.Service
{
    /// <summary>
    /// Конвертер для определения типа ShowProductPage.
    /// </summary>
    class IsShowProductPageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.GetType() == typeof(View.ShowProductPage);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
