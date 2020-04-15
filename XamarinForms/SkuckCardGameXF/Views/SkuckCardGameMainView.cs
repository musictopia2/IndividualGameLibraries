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
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using SkuckCardGameCP.Data;
using Xamarin.Forms;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BasicGameFrameworkLibrary.TestUtilities;
using SkuckCardGameCP.ViewModels;
using BasicGamingUIXFLibrary.Helpers;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using SkuckCardGameCP.Cards;
using BasicGamingUIXFLibrary.BasicControls.TrickUIs;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIXFLibrary.BasicControls.Misc;

namespace SkuckCardGameXF.Views
{
    public class SkuckCardGameMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly SkuckCardGameVMData _model;
        private readonly SkuckCardGameGameContainer _gameContainer;
        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<SkuckCardGameCardInformation, ts, DeckOfCardsXF<SkuckCardGameCardInformation>> _playerHandWPF;



        private readonly TwoPlayerTrickXF<EnumSuitList, SkuckCardGameCardInformation, ts, DeckOfCardsXF<SkuckCardGameCardInformation>> _trick1;

        private readonly PlayerBoardXF<SkuckCardGameCardInformation> _temp1 = new PlayerBoardXF<SkuckCardGameCardInformation>();
        private readonly PlayerBoardXF<SkuckCardGameCardInformation> _temp2 = new PlayerBoardXF<SkuckCardGameCardInformation>();


        public SkuckCardGameMainView(IEventAggregator aggregator,
            TestOptions test,
            SkuckCardGameVMData model,
            SkuckCardGameGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            _gameContainer = gameContainer;
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<SkuckCardGameCardInformation, ts, DeckOfCardsXF<SkuckCardGameCardInformation>>();
            _trick1 = new TwoPlayerTrickXF<EnumSuitList, SkuckCardGameCardInformation, ts, DeckOfCardsXF<SkuckCardGameCardInformation>>();
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(SkuckCardGameMainViewModel.RestoreScreen));
            }
            _score.AddColumn("Strength", false, nameof(SkuckCardGamePlayerItem.StrengthHand));
            _score.AddColumn("Tie", false, nameof(SkuckCardGamePlayerItem.TieBreaker));
            _score.AddColumn("Bid", false, nameof(SkuckCardGamePlayerItem.BidAmount), visiblePath: nameof(SkuckCardGamePlayerItem.BidVisible));
            _score.AddColumn("Won", false, nameof(SkuckCardGamePlayerItem.TricksWon));
            _score.AddColumn("Cards", false, nameof(SkuckCardGamePlayerItem.ObjectCount));
            _score.AddColumn("P Rounds", false, nameof(SkuckCardGamePlayerItem.PerfectRounds));
            _score.AddColumn("T Score", false, nameof(SkuckCardGamePlayerItem.TotalScore));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Round", nameof(SkuckCardGameMainViewModel.RoundNumber));
            firstInfo.AddRow("Trump", nameof(SkuckCardGameMainViewModel.TrumpSuit)); //if trump is not needed, then comment.
            firstInfo.AddRow("Turn", nameof(SkuckCardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(SkuckCardGameMainViewModel.Status));
            _playerHandWPF.Divider = 1.4;
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_trick1);
            ParentSingleUIContainer parent = new ParentSingleUIContainer(nameof(SkuckCardGameMainViewModel.BidScreen));
            otherStack.Children.Add(parent);
            parent = new ParentSingleUIContainer(nameof(SkuckCardGameMainViewModel.SuitScreen));
            otherStack.Children.Add(parent);
            parent = new ParentSingleUIContainer(nameof(SkuckCardGameMainViewModel.ChoosePlayScreen));
            otherStack.Children.Add(parent);
            mainStack.Children.Add(otherStack);


            mainStack.Children.Add(_playerHandWPF);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            mainStack.Children.Add(otherStack);
            otherStack.Children.Add(_score);
            otherStack.Children.Add(firstInfo.GetContent);
            Grid finalGrid = new Grid();
            Grid tempGrid = new Grid();
            AddLeftOverRow(tempGrid, 1);
            AddLeftOverRow(tempGrid, 1);
            AddLeftOverColumn(finalGrid, 1);
            AddAutoColumns(finalGrid, 1);
            AddControlToGrid(finalGrid, mainStack, 0, 0);
            AddControlToGrid(finalGrid, tempGrid, 0, 1);
            AddControlToGrid(tempGrid, _temp1, 0, 0);
            AddControlToGrid(tempGrid, _temp2, 1, 0);


            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            SkuckCardGameSaveInfo save = cons!.Resolve<SkuckCardGameSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
            _trick1!.Init(_model.TrickArea1!, ts.TagUsed);
            SkuckCardGamePlayerItem selfPlayer = _gameContainer.PlayerList!.GetSelf();
            SkuckCardGamePlayerItem otherPlayer = _gameContainer.PlayerList.GetOnlyOpponent();

            _temp1.LoadList(selfPlayer.TempHand!);
            _temp2.LoadList(otherPlayer.TempHand!);

            Content = finalGrid;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
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
