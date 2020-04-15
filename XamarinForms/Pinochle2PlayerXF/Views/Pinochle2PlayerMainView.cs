using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.Misc;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.BasicControls.TrickUIs;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using Pinochle2PlayerCP.Cards;
using Pinochle2PlayerCP.Data;
using Pinochle2PlayerCP.ViewModels;
using SkiaSharp;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace Pinochle2PlayerXF.Views
{
    public class Pinochle2PlayerMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly Pinochle2PlayerVMData _model;
        private readonly IGamePackageResolver _resolver;
        private readonly BaseDeckXF<Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>> _deckGPile;
        private readonly BasePileXF<Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>> _playerHandWPF;

        private readonly TwoPlayerTrickXF<EnumSuitList, Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>> _trick1;

        private readonly GuideUI _guide1 = new GuideUI();
        private readonly StackLayout _deckStack;
        private readonly BaseHandXF<Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>> _yourMeld;
        private readonly BaseHandXF<Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>> _opponentMeld;
        private readonly TempRummySetsXF<EnumSuitList, EnumColorList, Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>> _tempG;


        public Pinochle2PlayerMainView(IEventAggregator aggregator,
            TestOptions test,
            Pinochle2PlayerVMData model,
            IGamePackageResolver resolver
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            _resolver = resolver;
            _deckGPile = new BaseDeckXF<Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>>();
            _discardGPile = new BasePileXF<Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>>();
            _trick1 = new TwoPlayerTrickXF<EnumSuitList, Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>>();
            _guide1 = new GuideUI();
            _deckStack = new StackLayout();
            _yourMeld = new BaseHandXF<Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>>();
            _opponentMeld = new BaseHandXF<Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>>();
            _tempG = new TempRummySetsXF<EnumSuitList, EnumColorList, Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>>();
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(Pinochle2PlayerMainViewModel.RestoreScreen));
            }
            _opponentMeld.Divider = 1.5;
            _yourMeld.Divider = 1.5;
            _tempG.Divider = 1.5;
            _yourMeld.HandType = HandObservable<Pinochle2PlayerCardInformation>.EnumHandList.Vertical;
            _opponentMeld.HandType = HandObservable<Pinochle2PlayerCardInformation>.EnumHandList.Vertical;
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckStack);
            _deckStack.Children.Add(_discardGPile);
            otherStack.Children.Add(_trick1);
            otherStack.Children.Add(_tempG);
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Cards", false, nameof(Pinochle2PlayerPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("T Won", false, nameof(Pinochle2PlayerPlayerItem.TricksWon));
            _score.AddColumn("C Score", false, nameof(Pinochle2PlayerPlayerItem.CurrentScore));
            _score.AddColumn("T Score", false, nameof(Pinochle2PlayerPlayerItem.TotalScore));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(Pinochle2PlayerMainViewModel.NormalTurn));
            firstInfo.AddRow("Trump", nameof(Pinochle2PlayerMainViewModel.TrumpSuit));
            firstInfo.AddRow("Status", nameof(Pinochle2PlayerMainViewModel.Status));

            var endButton = GetGamingButton("End Turn", nameof(Pinochle2PlayerMainViewModel.EndTurnAsync));
            endButton.HorizontalOptions = LayoutOptions.Start;
            var meldBut = GetGamingButton("Meld", nameof(Pinochle2PlayerMainViewModel.MeldAsync));
            mainStack.Children.Add(_playerHandWPF);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            StackLayout tempStack = new StackLayout();
            tempStack.Children.Add(meldBut);
            tempStack.Children.Add(endButton);
            otherStack.Children.Add(tempStack); //i think this too.
            tempStack = new StackLayout();
            tempStack.Children.Add(firstInfo.GetContent);
            tempStack.Children.Add(_score);
            otherStack.Children.Add(tempStack);
            otherStack.Children.Add(_yourMeld);
            otherStack.Children.Add(_opponentMeld);
            mainStack.Children.Add(otherStack);
            Grid finalGrid = new Grid();
            AddLeftOverColumn(finalGrid, 60);
            AddLeftOverColumn(finalGrid, 40); // hopefully that works.
            AddControlToGrid(finalGrid, mainStack, 0, 0);
            AddControlToGrid(finalGrid, _guide1, 0, 1);

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
            _deckGPile.Margin = new Thickness(9, heights * -1, 0, 0);
            _deckStack!.Children.Add(_deckGPile);
            _guide1.LoadList(_model);

            return this.RefreshBindingsAsync(_aggregator);
        }


        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _playerHandWPF.Dispose(); //at least will help improve performance.
            _trick1.Dispose();
            return Task.CompletedTask;
        }
    }
}
