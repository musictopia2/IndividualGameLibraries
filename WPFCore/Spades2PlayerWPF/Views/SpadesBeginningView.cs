using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using Spades2PlayerCP.Cards;
using Spades2PlayerCP.Data;
using Spades2PlayerCP.ViewModels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace Spades2PlayerWPF.Views
{
    public class SpadesBeginningView : UserControl, IUIView
    {
        private readonly BaseDeckWPF<Spades2PlayerCardInformation, ts, DeckOfCardsWPF<Spades2PlayerCardInformation>> _deckGPile;
        private readonly BasePileWPF<Spades2PlayerCardInformation, ts, DeckOfCardsWPF<Spades2PlayerCardInformation>> _discardGPile;
        private readonly BasePileWPF<Spades2PlayerCardInformation, ts, DeckOfCardsWPF<Spades2PlayerCardInformation>> _other;
        private readonly Spades2PlayerVMData _model;

        public SpadesBeginningView(Spades2PlayerVMData model)
        {
            StackPanel stack = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            _deckGPile = new BaseDeckWPF<Spades2PlayerCardInformation, ts, DeckOfCardsWPF<Spades2PlayerCardInformation>>();
            _discardGPile = new BasePileWPF<Spades2PlayerCardInformation, ts, DeckOfCardsWPF<Spades2PlayerCardInformation>>();
            _other = new BasePileWPF<Spades2PlayerCardInformation, ts, DeckOfCardsWPF<Spades2PlayerCardInformation>>();
            stack.Children.Add(_deckGPile);
            stack.Children.Add(_discardGPile);
            var button = GetGamingButton("Take Card", nameof(SpadesBeginningViewModel.TakeCardAsync));
            stack.Children.Add(button);
            stack.Children.Add(_other);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            _other.Margin = new Thickness(5);

            Content = stack;
            _model = model;
        }
        Task IUIView.TryActivateAsync()
        {
            _discardGPile!.Init(_model.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();

            _other.Init(_model.OtherPile!, ts.TagUsed);
            _other.StartAnimationListener("otherpile");
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
