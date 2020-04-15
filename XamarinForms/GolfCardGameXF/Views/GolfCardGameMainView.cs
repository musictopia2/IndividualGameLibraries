using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.MultipleFrameContainers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using GolfCardGameCP.Data;
using GolfCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace GolfCardGameXF.Views
{
    public class GolfCardGameMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly GolfCardGameVMData _model;
        private readonly BaseDeckXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> _deckGPile;
        private readonly BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> _discardGPile;

        private readonly ScoreBoardXF _score;

        private readonly BasicMultiplePilesXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> _hiddenWPF;
        private readonly BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> _otherPileWPF;
        private readonly CardBoardXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> _golfWPF;

        public GolfCardGameMainView(IEventAggregator aggregator,
            TestOptions test,
            GolfCardGameVMData model,
            GolfCardGameGameContainer gameContainer
            )
        {

            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            _deckGPile = new BaseDeckXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _discardGPile = new BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _score = new ScoreBoardXF();

            _hiddenWPF = new BasicMultiplePilesXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _otherPileWPF = new BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _golfWPF = new CardBoardXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            ParentSingleUIContainer? restoreP = null;
            StackLayout mainStack = new StackLayout();
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(GolfCardGameMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            var button = GetGamingButton("Knock", nameof(GolfCardGameMainViewModel.KnockAsync));
            otherStack.Children.Add(button);
            mainStack.Children.Add(otherStack);
            _score.UseAbbreviationForTrueFalse = true;
            mainStack.Children.Add(_hiddenWPF);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_golfWPF);
            otherStack.Children.Add(_otherPileWPF);
            mainStack.Children.Add(otherStack);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(GolfCardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(GolfCardGameMainViewModel.Status));
            Grid finalGrid = new Grid();
            AddLeftOverColumn(finalGrid, 40);
            AddLeftOverColumn(finalGrid, 60); // this is for scoreboard
            _score.AddColumn("Knocked", false, nameof(GolfCardGamePlayerItem.Knocked), useTrueFalse: true); // well see how this work.  hopefully this simple.
            _score.AddColumn("1 Changed", false, nameof(GolfCardGamePlayerItem.FirstChanged), useTrueFalse: true);
            _score.AddColumn("2 Changed", false, nameof(GolfCardGamePlayerItem.SecondChanged), useTrueFalse: true);
            _score.AddColumn("Previous Score", false, nameof(GolfCardGamePlayerItem.PreviousScore), rightMargin: 20);
            _score.AddColumn("Total Score", false, nameof(GolfCardGamePlayerItem.TotalScore), rightMargin: 20);
            firstInfo.AddRow("Round", nameof(GolfCardGameMainViewModel.Round));
            firstInfo.AddRow("Instructions", nameof(GolfCardGameMainViewModel.Instructions));
            mainStack.Children.Add(finalGrid);
            AddControlToGrid(finalGrid, firstInfo.GetContent, 0, 0);
            AddControlToGrid(finalGrid, _score, 0, 1);

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            _score!.LoadLists(gameContainer.SaveRoot.PlayerList);


            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }

            

            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            return this.RefreshBindingsAsync(_aggregator); //may be needed so it can do the buttons.
        }

        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        Task IUIView.TryActivateAsync()
        {
            _discardGPile!.Init(_model.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();


            _otherPileWPF!.Init(_model.OtherPile!, ts.TagUsed);
            _hiddenWPF!.Init(_model.HiddenCards1!, ts.TagUsed);
            _golfWPF!.LoadList(_model.GolfHand1!, ts.TagUsed);
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this); //try this too.
            return Task.CompletedTask;
        }
    }
}
