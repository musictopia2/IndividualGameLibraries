using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using OldMaidCP.Data;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace OldMaidXF.Views
{
    public class OpponentCardsView : CustomControlBase
    {
        public OpponentCardsView(OldMaidVMData model)
        {
            BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> hand = new BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            hand.Divider = 2;
            hand.LoadList(model.OpponentCards1, ts.TagUsed);
            Content = hand;
        }
    }
}
