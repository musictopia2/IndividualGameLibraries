using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using LifeBoardGameCP;
using System.Linq;
namespace LifeBoardGameWPF
{
    public class LifeHandWPF : BaseHandWPF<LifeBaseCard, CardCP, CardWPF>
    {
        protected override void AfterCollectionChange()
        {
            if (ObjectList!.Count == 0)
                return;
            if (ObjectList.First().IsUnknown)
            {
                Divider = 1.4;
                MaximumCards = 0;
                Height = 900;
            }
            else
            {
                Divider = 1;
                MaximumCards = ObjectList.Count;
            }
            RecalulateFrames();
        }
    }
    public class LifePileWPF : BasePileWPF<LifeBaseCard, CardCP, CardWPF> { }
}
