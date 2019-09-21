using CommonBasicStandardLibraries.CommonConverters;
using System;
using System.Globalization;
namespace RummyDiceCP
{
    public abstract class RollConverterCP : IConverterCP
    {
        public object Convert(object value, Type TargetType, object Parameter, CultureInfo culture)
        {
            return int.Parse(value.ToString()) - 1;
        }

        public object ConvertBack(object value, Type TargetType, object Parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}