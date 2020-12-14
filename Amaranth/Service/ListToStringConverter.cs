using System;
using System.Windows.Data;
using System.Globalization;
using System.Collections.ObjectModel;

namespace Amaranth.Service
{
    /// <summary>
    /// Конвертер для преобразование списков в строку через запятую.
    /// </summary>
    class ListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var list = (ObservableCollection<string>)value;
            if (list != null)
            {
                string s = string.Empty;
                // Перебор строк
                foreach (var str in list)
                {
                    // Объединение через запятую
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
