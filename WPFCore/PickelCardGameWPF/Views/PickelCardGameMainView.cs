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
using PickelCardGameCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using PickelCardGameCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using BasicGamingUIWPFLibrary.Helpers;
using System.Windows;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using PickelCardGameCP.Cards;
using BasicGamingUIWPFLibrary.BasicControls.TrickUIs;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace PickelCardGameWPF.Views
{
    public class PickelCardGameMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly PickelCardGameVMData _model;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<PickelCardGameCardInformation, ts, DeckOfCardsWPF<PickelCardGameCardInformation>> _playerHandWPF;

        private readonly TwoPlayerTrickWPF<EnumSuitList, PickelCardGameCardInformation, ts, DeckOfCardsWPF<PickelCardGameCardInformation>> _trick1;


        public PickelCardGameMainView(IEventAggregator aggregator,
            TestOptions test,
            PickelCardGameVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            GamePackageViewModelBinder.ManuelElements.Clear();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<PickelCardGameCardInformation, ts, DeckOfCardsWPF<PickelCardGameCardInformation>>();

            _trick1 = new TwoPlayerTrickWPF<EnumSuitList, PickelCardGameCardInformation, ts, DeckOfCardsWPF<PickelCardGameCardInformation>>();


            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(PickelCardGameMainViewModel.RestoreScreen)
                };
            }

            PopulateScores(_score);




            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(PickelCardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Trump", nameof(PickelCardGameMainViewModel.TrumpSuit));
            firstInfo.AddRow("Status", nameof(PickelCardGameMainViewModel.Status));
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
        private void PopulateScores(ScoreBoardWPF score)
        {
            score.AddColumn("Suit Desired", true, nameof(PickelCardGamePlayerItem.SuitDesired));
            score.AddColumn("Bid Amount", false, nameof(PickelCardGamePlayerItem.BidAmount));
            score.AddColumn("Tricks Won", false, nameof(PickelCardGamePlayerItem.TricksWon));
            score.AddColumn("Current Score", false, nameof(PickelCardGamePlayerItem.CurrentScore));
            score.AddColumn("Total Score", false, nameof(PickelCardGamePlayerItem.TotalScore));
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {


            return Task.CompletedTask;
        }

        

        Task IUIView.TryActivateAsync()
        {
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            PickelCardGameSaveInfo save = cons!.Resolve<PickelCardGameSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
            _trick1!.Init(_model.TrickArea1!, ts.TagUsed);
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _trick1.Dispose();
            return Task.CompletedTask;
        }
    }
}
