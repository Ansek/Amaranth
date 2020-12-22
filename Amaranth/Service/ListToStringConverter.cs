using System;
using System.Windows.Data;
using System.Globalization;
using System.Collections.Generic;
using Amaranth.Model.Data;

namespace Amaranth.Service
{
    /// <summary>
    /// Конвертер для преобразования списков в строку через запятую.
    /// </summary>
    class ListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var list = (IEnumerable<Tag>)value;
            if (list != null)
            {
                string s = string.Empty;
                // Перебор строк
                foreach (var str in list)
                {
                    // Объединение через запятую
                    if (s != string.Empty)
                        s += ", ";
                    s += str.Title;
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
