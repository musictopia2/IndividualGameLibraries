using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using LifeBoardGameCP.Cards;
using LifeBoardGameCP.Graphics;
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
}