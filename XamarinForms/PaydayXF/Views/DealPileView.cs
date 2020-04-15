using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using PaydayCP.Cards;
using PaydayCP.Data;
using PaydayCP.Graphics;

namespace PaydayXF.Views
{
    public class DealPileView : CustomControlBase
    {
        public DealPileView(PaydayVMData data)
        {
            BasePileXF<DealCard, CardGraphicsCP, DealCardXF> pile = new BasePileXF<DealCard, CardGraphicsCP, DealCardXF>();
            pile.Init(data.DealPile, "");
            Content = pile;
        }

    }
}