using System;
using System.Globalization;
using ThinkTwiceCP;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace ThinkTwiceXF
{
    public class ColorConverter : IValueConverter
    {
        private ScoreViewModel? Scores1;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Scores1 == null)
                Scores1 = Resolve<ScoreViewModel>();
            if (Scores1.ItemSelected == -1)
                return Color.Aqua;// because absolutely nothing was selected.
            var tempText = Scores1.TextList[Scores1.ItemSelected]; // its okay because its used for this purpose only.
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