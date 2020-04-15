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
using PickelCardGameCP.Data;
using Xamarin.Forms;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BasicGameFrameworkLibrary.TestUtilities;
using PickelCardGameCP.ViewModels;
using BasicGamingUIXFLibrary.Helpers;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using PickelCardGameCP.Cards;
using BasicGamingUIXFLibrary.BasicControls.TrickUIs;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using BasicGameFrameworkLibrary.RegularDeckOfCards;

namespace PickelCardGameXF.Views
{
    public class PickelCardGameMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly PickelCardGameVMData _model;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<PickelCardGameCardInformation, ts, DeckOfCardsXF<PickelCardGameCardInformation>> _playerHandWPF;

        private readonly TwoPlayerTrickXF<EnumSuitList, PickelCardGameCardInformation, ts, DeckOfCardsXF<PickelCardGameCardInformation>> _trick1;

        public PickelCardGameMainView(IEventAggregator aggregator,
            TestOptions test,
            PickelCardGameVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<PickelCardGameCardInformation, ts, DeckOfCardsXF<PickelCardGameCardInformation>>();
            _trick1 = new TwoPlayerTrickXF<EnumSuitList, PickelCardGameCardInformation, ts, DeckOfCardsXF<PickelCardGameCardInformation>>();
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(PickelCardGameMainViewModel.RestoreScreen));
            }
            PopulateScores(_score);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(PickelCardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Trump", nameof(PickelCardGameMainViewModel.TrumpSuit));
            firstInfo.AddRow("Status", nameof(PickelCardGameMainViewModel.Status));
            mainStack.Children.Add(_trick1);
            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(firstInfo.GetContent);

            mainStack.Children.Add(_score);
            //this is only a starting point.

            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }

        private void PopulateScores(ScoreBoardXF score)
        {
            score.AddColumn("Suit Desired", true, nameof(PickelCardGamePlayerItem.SuitDesired));
            score.AddColumn("Bid", false, nameof(PickelCardGamePlayerItem.BidAmount));
            score.AddColumn("Won", false, nameof(PickelCardGamePlayerItem.TricksWon));
            score.AddColumn("C Score", false, nameof(PickelCardGamePlayerItem.CurrentScore));
            score.AddColumn("T Score", false, nameof(PickelCardGamePlayerItem.TotalScore));
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
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            PickelCardGameSaveInfo save = cons!.Resolve<PickelCardGameSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
            _trick1!.Init(_model.TrickArea1!, ts.TagUsed);
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
