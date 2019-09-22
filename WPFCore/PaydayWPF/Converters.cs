using PaydayCP;
using System.Windows.Data;
using static BasicControlsAndWindowsCore.BasicWindows.Misc.WindowHelper;
namespace PaydayWPF
{
    public class ShowRollConverter : ShowRollConverterCP, IValueConverter
    {
        public ShowRollConverter()
        {
            VisibleDelegate = GetPublicVisible;
        }
    }
}