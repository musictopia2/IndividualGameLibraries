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
using RageCardGameCP.Data;
using Xamarin.Forms;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BasicGameFrameworkLibrary.TestUtilities;
using RageCardGameCP.ViewModels;
using BasicGamingUIXFLibrary.Helpers;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using RageCardGameCP.Cards;
using BasicGamingUIXFLibrary.BasicControls.TrickUIs;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using BasicGameFrameworkLibrary.RegularDeckOfCards;

namespace RageCardGameXF.Views
{
    public class RageCardGameMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly RageCardGameVMData _model;
        private readonly BaseDeckXF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsXF> _deckGPile;
        private readonly BasePileXF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsXF> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsXF> _playerHandWPF;

        private readonly SeveralPlayersTrickXF<EnumColor, RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsXF, RageCardGamePlayerItem> _trick1;

        public RageCardGameMainView(IEventAggregator aggregator,
            TestOptions test,
            RageCardGameVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckXF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsXF>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsXF>();
            _trick1 = new SeveralPlayersTrickXF<EnumColor, RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsXF, RageCardGamePlayerItem>();
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(RageCardGameMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            mainStack.Children.Add(otherStack);
            PopulateScores(_score);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(RageCardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(RageCardGameMainViewModel.Status));
            firstInfo.AddRow("Trump", nameof(RageCardGameMainViewModel.TrumpSuit));
            firstInfo.AddRow("Lead", nameof(RageCardGameMainViewModel.Lead));
            mainStack.Children.Add(_trick1);
            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(_score);

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);

            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            RageCardGameSaveInfo save = cons!.Resolve<RageCardGameSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ""); // i think

            _trick1!.Init(_model.TrickArea1!, _model.TrickArea1, ""); //just in case the load won't work properly this time on xamarin forms.

            Content = mainStack;
        }

        internal static void PopulateScores(ScoreBoardXF tempScore)
        {
            tempScore.AddColumn("Cards Left", true, nameof(RageCardGamePlayerItem.ObjectCount));
            tempScore.AddColumn("Bid Amount", true, nameof(RageCardGamePlayerItem.BidAmount), visiblePath: nameof(RageCardGamePlayerItem.RevealBid));
            tempScore.AddColumn("Tricks Won", true, nameof(RageCardGamePlayerItem.TricksWon));
            tempScore.AddColumn("Correctly Bidded", true, nameof(RageCardGamePlayerItem.CorrectlyBidded));
            tempScore.AddColumn("Score Round", true, nameof(RageCardGamePlayerItem.ScoreRound));
            tempScore.AddColumn("Score Game", true, nameof(RageCardGamePlayerItem.ScoreGame));
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
