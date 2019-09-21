using CommonBasicStandardLibraries.CommonConverters;
using System;
using System.Globalization;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace FluxxCP
{
    public class KeeperVisibleConverterCP : IConverterCP
    {
        public VisibleTranslation? VisibleDelegate;
        public object Convert(object value, Type targetType, object Parameter, CultureInfo culture)
        {
            var whatIs = (EnumKeeperSection)value;
            var category = (EnumKeeperVisibleCategory)Parameter;
            if (category == EnumKeeperVisibleCategory.Close && whatIs == EnumKeeperSection.None)
                return FinalResults(true);
            if (category == EnumKeeperVisibleCategory.Close || whatIs == EnumKeeperSection.None)
                return FinalResults(false);
            return FinalResults(true);
        }
        private object FinalResults(bool value)
        {
            if (VisibleDelegate == null)
                return value;
            return VisibleDelegate(value);
        }
        public object ConvertBack(object value, Type targetType, object Parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ActionVisibleConverterCP : EnumVisibleConverter<EnumActionCategory>
    {
        protected override bool Convert(EnumActionCategory enumSent, EnumActionCategory parameter)
        {
            return enumSent == parameter;
        }
    }
    public class KeeperTextConverterCP : IConverterCP
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var whatIs = (EnumKeeperSection)value;
            return whatIs switch
            {
                EnumKeeperSection.Trash => "Trash A Keeper",
                EnumKeeperSection.Steal => "Steal A Keeper",
                EnumKeeperSection.Exchange => "Exchange A Keeper",
                _ => "",
            };
        }
        public object ConvertBack(object value, Type targetType, object Parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}