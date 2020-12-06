using System;
using System.Windows.Data;
using System.Globalization;

namespace Amaranth.Service
{
    class PriceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string res = string.Empty;
            if (values != null)
            {
                int c = System.Convert.ToInt32(values[0]);
                double p = System.Convert.ToDouble(values[1]);
                res = $"{c}x{p:0.00}={c*p:0.00₽}";
            }
            return res;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
