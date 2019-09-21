using CommonBasicStandardLibraries.CommonConverters;
using System;
using System.Globalization;
namespace MinesweeperCP
{
    public abstract class ToggleNameConverterCP : IConverterCP
    {
        public object Convert(object value, Type TargetType, object Parameter, CultureInfo culture)
        {
            bool rets = bool.Parse(value.ToString());
            if (rets)
                return "Flag Mines";
            return "Unflip Mines";
        }

        public object ConvertBack(object value, Type TargetType, object Parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}