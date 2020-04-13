using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using System.Windows.Controls;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using GolfCardGameCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using GolfCardGameCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using BasicGamingUIWPFLibrary.Helpers;
using System.Windows;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.MultipleFrameContainers;

namespace GolfCardGameWPF.Views
{
    public class GolfCardGameMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;
        private readonly GolfCardGameVMData _model;
        private readonly BaseDeckWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>> _deckGPile;
        private readonly BasePileWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>> _discardGPile;

        private readonly ScoreBoardWPF _score;

        private readonly BasicMultiplePilesWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>> _hiddenWPF;
        private readonly BasePileWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>> _otherPileWPF;
        private readonly CardBoardWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>> _golfWPF;

        public GolfCardGameMainView(IEventAggregator aggregator,
            TestOptions test,
            GolfCardGameVMData model,
            GolfCardGameGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _discardGPile = new BasePileWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _score = new ScoreBoardWPF();

            _hiddenWPF = new BasicMultiplePilesWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _otherPileWPF = new BasePileWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _golfWPF = new CardBoardWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();


            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(GolfCardGameMainViewModel.RestoreScreen)
                };
            }


            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            var button = GetGamingButton("Knock", nameof(GolfCardGameMainViewModel.KnockAsync));
            otherStack.Children.Add(button);
            mainStack.Children.Add(otherStack);
            _score.UseAbbreviationForTrueFalse = true;
            mainStack.Children.Add(_hiddenWPF);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_golfWPF);
            otherStack.Children.Add(_otherPileWPF);
            mainStack.Children.Add(otherStack);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
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


            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }

            _score!.LoadLists(gameContainer.SaveRoot.PlayerList);
            _discardGPile!.Init(_model.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();


            _otherPileWPF!.Init(_model.OtherPile!, ts.TagUsed);
            _hiddenWPF!.Init(_model.HiddenCards1!, ts.TagUsed);
            _golfWPF!.LoadList(_model.GolfHand1!, ts.TagUsed);

            Content = mainStack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            return Task.CompletedTask;
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
