using MinesweeperCP;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
namespace MinesweeperWPF
{
    public class ToggleNameConverter : ToggleNameConverterCP, IValueConverter { }
    public class ToggleColorConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool rets = bool.Parse(value.ToString()!);
            if (rets)
                return Brushes.Yellow;
            return Brushes.Aqua;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}