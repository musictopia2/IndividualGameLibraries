using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ThinkTwiceCP;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace ThinkTwiceWPF
{
    public class ColorConverter : IValueConverter
    {
        private ScoreViewModel? Scores1;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Scores1 == null)
                Scores1 = Resolve<ScoreViewModel>();
            if (Scores1.ItemSelected == -1)
                return Brushes.Aqua;// because absolutely nothing was selected.
            var tempText = Scores1.TextList[Scores1.ItemSelected]; // its okay because its used for this purpose only.
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
