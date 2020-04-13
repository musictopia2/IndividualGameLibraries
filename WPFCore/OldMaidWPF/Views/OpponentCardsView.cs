using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using OldMaidCP.Data;
using System.Threading.Tasks;
using System.Windows.Controls;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace OldMaidWPF.Views
{
    public class OpponentCardsView : UserControl, IUIView
    {

        public OpponentCardsView(OldMaidVMData model)
        {
            BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>> hand = new BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            hand.Divider = 2;
            hand.LoadList(model.OpponentCards1, ts.TagUsed);
            Content = hand;
        }

        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
