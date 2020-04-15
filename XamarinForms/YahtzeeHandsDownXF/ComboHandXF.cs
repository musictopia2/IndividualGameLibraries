using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using YahtzeeHandsDownCP.Cards;

namespace YahtzeeHandsDownXF
{
    public class ComboHandXF : BaseHandXF<ComboCardInfo, ComboCP, ComboCardXF>
    {
        protected override void FinishBindings(ComboCardXF thisDeck, ComboCardInfo thisCard)
        {
            thisDeck.CurrentCombo = thisCard;
        }
    }
    
}