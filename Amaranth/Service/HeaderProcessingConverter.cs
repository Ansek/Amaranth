using System;
using System.Windows.Data;
using System.Globalization;

namespace Amaranth.Service
{
    /// <summary>
    /// Конвертер для обрезания заголовков.
    /// </summary>
    class HeaderProcessingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string title = value.ToString();
            // Уменьшения имени до 15 символов и установка многоточия
            if (title.Length > 15)
                title = title.Substring(0, 15) + "...";
            return title;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
