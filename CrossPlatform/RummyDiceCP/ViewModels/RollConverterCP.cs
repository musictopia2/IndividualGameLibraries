using CommonBasicStandardLibraries.CommonConverters;
using System;
using System.Globalization;
namespace RummyDiceCP.ViewModels
{
    public abstract class RollConverterCP : IConverterCP
    {
        public object Convert(object value, Type target, object Parameter, CultureInfo culture)
        {
            return int.Parse(value.ToString()) - 1;
        }

        public object ConvertBack(object value, Type target, object Parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
