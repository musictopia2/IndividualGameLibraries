using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
namespace MillebournesWPF
{
    public class SafetyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (bool.Parse(value.ToString()!) == true)
                return FontWeights.Bold;
            return FontWeights.Regular;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}