using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.Misc;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIWPFLibrary.BasicControls.TrickUIs;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using Pinochle2PlayerCP.Cards;
using Pinochle2PlayerCP.Data;
using Pinochle2PlayerCP.ViewModels;
using SkiaSharp;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace Pinochle2PlayerWPF.Views
{
    public class Pinochle2PlayerMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly Pinochle2PlayerVMData _model;
        private readonly IGamePackageResolver _resolver;
        private readonly BaseDeckWPF<Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>> _deckGPile;
        private readonly BasePileWPF<Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>> _discardGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>> _playerHandWPF;

        private readonly TwoPlayerTrickWPF<EnumSuitList, Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>> _trick1;

        private readonly GuideUI _guide1 = new GuideUI();
        private readonly StackPanel _deckStack;
        private readonly BaseHandWPF<Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>> _yourMeld;
        private readonly BaseHandWPF<Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>> _opponentMeld;
        private readonly TempRummySetsWPF<EnumSuitList, EnumColorList, Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>> _tempG;

        public Pinochle2PlayerMainView(IEventAggregator aggregator,
            TestOptions test,
            Pinochle2PlayerVMData model,
            IGamePackageResolver resolver
            )
        {
            _aggregator = aggregator;
            _model = model;
            _resolver = resolver;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>>();
            _discardGPile = new BasePileWPF<Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>>();

            _trick1 = new TwoPlayerTrickWPF<EnumSuitList, Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>>();

            _guide1 = new GuideUI();
            _deckStack = new StackPanel();
            _yourMeld = new BaseHandWPF<Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>>();
            _opponentMeld = new BaseHandWPF<Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>>();
            _tempG = new TempRummySetsWPF<EnumSuitList, EnumColorList, Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>>();

            _trick1.Width = 500; // i think.
            _yourMeld.Divider = 1.5;
            _opponentMeld.Divider = 1.5;
            _yourMeld.HandType = HandObservable<Pinochle2PlayerCardInformation>.EnumHandList.Vertical;
            _opponentMeld.HandType = HandObservable<Pinochle2PlayerCardInformation>.EnumHandList.Vertical;
            _tempG.Height = 250; //i think.

            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(Pinochle2PlayerMainViewModel.RestoreScreen)
                };
            }


            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckStack);
            _deckStack.Children.Add(_discardGPile);
            otherStack.Children.Add(_trick1);
            otherStack.Children.Add(_tempG);
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Cards Left", false, nameof(Pinochle2PlayerPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Tricks Won", false, nameof(Pinochle2PlayerPlayerItem.TricksWon));
            _score.AddColumn("Current Score", false, nameof(Pinochle2PlayerPlayerItem.CurrentScore));
            _score.AddColumn("Total Score", false, nameof(Pinochle2PlayerPlayerItem.TotalScore));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(Pinochle2PlayerMainViewModel.NormalTurn));
            firstInfo.AddRow("Trump", nameof(Pinochle2PlayerMainViewModel.TrumpSuit));
            firstInfo.AddRow("Status", nameof(Pinochle2PlayerMainViewModel.Status));
            mainStack.Children.Add(_playerHandWPF);

            var endButton = GetGamingButton("End Turn", nameof(Pinochle2PlayerMainViewModel.EndTurnAsync));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            var meldBut = GetGamingButton("Meld", nameof(Pinochle2PlayerMainViewModel.MeldAsync));
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            StackPanel tempStack = new StackPanel();
            tempStack.Children.Add(meldBut);
            tempStack.Children.Add(endButton);
            otherStack.Children.Add(tempStack);
            otherStack.Children.Add(_yourMeld);
            otherStack.Children.Add(_opponentMeld);
            mainStack.Children.Add(otherStack);
            StackPanel scoreStack = new StackPanel();
            scoreStack.Children.Add(_score);
            scoreStack.Children.Add(firstInfo.GetContent);
            scoreStack.Children.Add(_guide1);
            Grid finalGrid = new Grid();
            AddLeftOverColumn(finalGrid, 70);
            AddLeftOverColumn(finalGrid, 30); // hopefully that works.
            AddControlToGrid(finalGrid, mainStack, 0, 0);
            AddControlToGrid(finalGrid, scoreStack, 0, 1);


            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);


            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = finalGrid;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            Pinochle2PlayerSaveInfo save = cons!.Resolve<Pinochle2PlayerSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(_model.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _trick1!.Init(_model.TrickArea1!, ts.TagUsed);


            _tempG!.Init(_model.TempSets!, ts.TagUsed);
            _yourMeld!.LoadList(_model.YourMelds!, ts.TagUsed);
            _opponentMeld!.LoadList(_model.OpponentMelds!, ts.TagUsed);
            var thisCard = new Pinochle2PlayerCardInformation();
            IProportionImage thisP = _resolver.Resolve<IProportionImage>(ts.TagUsed);
            SKSize thisSize = thisCard.DefaultSize.GetSizeUsed(thisP.Proportion);
            var heights = thisSize.Height / 1.5f;
            _deckGPile.Margin = new Thickness(6, heights * -1, 0, 0);
            _deckStack!.Children.Add(_deckGPile);
            _guide1.LoadList(_model);

            return this.RefreshBindingsAsync(_aggregator);
        }



        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _trick1.Dispose();
            return Task.CompletedTask;
        }
    }
}
