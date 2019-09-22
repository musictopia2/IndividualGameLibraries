using LifeBoardGameCP;
using System.Windows.Data;
using static BasicControlsAndWindowsCore.BasicWindows.Misc.WindowHelper;
namespace LifeBoardGameWPF
{
    public class LifeVisibleWPF : LifeVisibleConverter, IValueConverter
    {
        public LifeVisibleWPF()
        {
            VisibleDelegate = GetPublicVisible;
        }
    }
}