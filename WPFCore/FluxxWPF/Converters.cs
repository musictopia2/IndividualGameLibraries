using FluxxCP;
using System.Windows.Data;
using static BasicControlsAndWindowsCore.BasicWindows.Misc.WindowHelper;
namespace FluxxWPF
{
    public class KeeperVisibleConverter : KeeperVisibleConverterCP, IValueConverter
    {
        public KeeperVisibleConverter()
        {
            VisibleDelegate = GetPublicVisible;
        }
    }
    public class KeeperTextConverter : KeeperTextConverterCP, IValueConverter { }
    public class ActionVisibleConverter : ActionVisibleConverterCP, IValueConverter
    {
        public ActionVisibleConverter()
        {
            VisibleDelegate = GetPublicVisible;
        }
    }
}