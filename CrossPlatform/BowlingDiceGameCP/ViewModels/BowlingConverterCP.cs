using CommonBasicStandardLibraries.CommonConverters;
using System;
using System.Globalization;

namespace BowlingDiceGameCP.ViewModels
{
    public abstract class BowlingConverterCP : IConverterCP
    {
        public object Convert(object value, Type TargetType, object Parameter, CultureInfo culture)
        {
            if (value.ToString() == "-1")
                return "";
            return value;
        }

        public object ConvertBack(object value, Type TargetType, object Parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
