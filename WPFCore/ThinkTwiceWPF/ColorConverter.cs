using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ThinkTwiceCP.Data;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace ThinkTwiceWPF
{
    public class ColorConverter : IValueConverter
    {
        private ThinkTwiceVMData? _scores1;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (_scores1 == null)
                _scores1 = Resolve<ThinkTwiceVMData>();
            if (_scores1.ItemSelected == -1)
                return Brushes.Aqua;// because absolutely nothing was selected.
            var tempText = _scores1.TextList[_scores1.ItemSelected]; // its okay because its used for this purpose only.
            if (tempText.Equals(parameter))
                return Brushes.LimeGreen;
            return Brushes.Aqua;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}