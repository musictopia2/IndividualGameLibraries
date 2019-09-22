using System;
using System.Globalization;
using Xamarin.Forms;
namespace MillebournesXF
{
    public class SafetyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (bool.Parse(value.ToString()!) == true)
                return FontAttributes.Bold;
            return FontAttributes.None;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}