using System;
using System.Windows.Data;
using System.Globalization;

namespace Amaranth.Service
{
    /// <summary>
    /// Конвертер для проверки, что значение на равно null.
    /// </summary>
    class NotNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
