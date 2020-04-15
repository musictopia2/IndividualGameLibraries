using MinesweeperCP.ViewModels;
using System;
using System.Globalization;
using Xamarin.Forms;
namespace MinesweeperXF
{
    public class ToggleNameConverter : ToggleNameConverterCP, IValueConverter { }
    public class ToggleColorConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool rets = bool.Parse(value.ToString()!);
            if (rets)
                return Color.Yellow;
            return Color.Aqua;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}