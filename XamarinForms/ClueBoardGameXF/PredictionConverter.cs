using System;
using System.Globalization;
using Xamarin.Forms;
namespace ClueBoardGameXF
{
    public class PredictionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value.Equals(parameter))
                    return Color.LimeGreen;
                return Color.Aqua;
            }
            catch
            {
                return Color.Aqua;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}