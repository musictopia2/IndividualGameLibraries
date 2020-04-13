using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.TrickUIs;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using RageCardGameCP.Cards;
using RageCardGameCP.Data;
using RageCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace RageCardGameWPF.Views
{
    public class RageCardGameMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly RageCardGameVMData _model;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsWPF> _playerHandWPF;

        private readonly SeveralPlayersTrickWPF<EnumColor, RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsWPF, RageCardGamePlayerItem> _trick1;

        internal static void PopulateScores(ScoreBoardWPF tempScore)
        {
            tempScore.AddColumn("Cards Left", true, nameof(RageCardGamePlayerItem.ObjectCount));
            tempScore.AddColumn("Bid Amount", true, nameof(RageCardGamePlayerItem.BidAmount), visiblePath: nameof(RageCardGamePlayerItem.RevealBid));
            tempScore.AddColumn("Tricks Won", true, nameof(RageCardGamePlayerItem.TricksWon));
            tempScore.AddColumn("Correctly Bidded", true, nameof(RageCardGamePlayerItem.CorrectlyBidded));
            tempScore.AddColumn("Score Round", true, nameof(RageCardGamePlayerItem.ScoreRound));
            tempScore.AddColumn("Score Game", true, nameof(RageCardGamePlayerItem.ScoreGame));
        }

        public RageCardGameMainView(IEventAggregator aggregator,
            TestOptions test,
            RageCardGameVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsWPF>();

            _trick1 = new SeveralPlayersTrickWPF<EnumColor, RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsWPF, RageCardGamePlayerItem>();


            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(RageCardGameMainViewModel.RestoreScreen)
                };
            }


            PopulateScores(_score);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(RageCardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(RageCardGameMainViewModel.Status));
            firstInfo.AddRow("Trump", nameof(RageCardGameMainViewModel.TrumpSuit));
            firstInfo.AddRow("Lead", nameof(RageCardGameMainViewModel.Lead));
            mainStack.Children.Add(_trick1);
            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(_score);

            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {


            return Task.CompletedTask;
        }



        Task IUIView.TryActivateAsync()
        {
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            RageCardGameSaveInfo save = cons!.Resolve<RageCardGameSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ""); // i think

            _trick1!.Init(_model.TrickArea1!, _model.TrickArea1, "");
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _trick1.Dispose();
            _aggregator.Unsubscribe(this);
            return Task.CompletedTask;
        }
    }
}
