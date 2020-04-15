using System;
using System.Globalization;
using ThinkTwiceCP.Data;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace ThinkTwiceXF
{
    public class ColorConverter : IValueConverter
    {
        private ThinkTwiceVMData? _scores1;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (_scores1 == null)
                _scores1 = Resolve<ThinkTwiceVMData>();
            if (_scores1.ItemSelected == -1)
                return Color.Aqua;// because absolutely nothing was selected.
            var tempText = _scores1.TextList[_scores1.ItemSelected]; // its okay because its used for this purpose only.
            if (tempText.Equals(parameter))
                return Color.LimeGreen;
            return Color.Aqua;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}