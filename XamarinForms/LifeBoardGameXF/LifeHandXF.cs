using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using LifeBoardGameCP;
using System.Linq;
namespace LifeBoardGameXF
{
    public class LifeHandXF : BaseHandXF<LifeBaseCard, CardCP, CardXF>
    {
        protected override void AfterCollectionChange()
        {
            if (ObjectList!.Count == 0)
                return;
            if (ObjectList.First().IsUnknown)
            {
                Divider = 1.4;
                MaximumCards = 0;
            }
            else
            {
                Divider = 1;
                MaximumCards = ObjectList.Count;
            }
        }
    }
    public class LifePileXF : BasePileXF<LifeBaseCard, CardCP, CardXF> { }
}