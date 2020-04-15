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
using RoundsCardGameCP.Data;
using Xamarin.Forms;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BasicGameFrameworkLibrary.TestUtilities;
using RoundsCardGameCP.ViewModels;
using BasicGamingUIXFLibrary.Helpers;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using RoundsCardGameCP.Cards;
using BasicGamingUIXFLibrary.BasicControls.TrickUIs;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using BasicGameFrameworkLibrary.RegularDeckOfCards;

namespace RoundsCardGameXF.Views
{
    public class RoundsCardGameMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly RoundsCardGameVMData _model;
        private readonly BaseDeckXF<RoundsCardGameCardInformation, ts, DeckOfCardsXF<RoundsCardGameCardInformation>> _deckGPile;
        private readonly BasePileXF<RoundsCardGameCardInformation, ts, DeckOfCardsXF<RoundsCardGameCardInformation>> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<RoundsCardGameCardInformation, ts, DeckOfCardsXF<RoundsCardGameCardInformation>> _playerHandWPF;

        private readonly TwoPlayerTrickXF<EnumSuitList, RoundsCardGameCardInformation, ts, DeckOfCardsXF<RoundsCardGameCardInformation>> _trick1;

        public RoundsCardGameMainView(IEventAggregator aggregator,
            TestOptions test,
            RoundsCardGameVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckXF<RoundsCardGameCardInformation, ts, DeckOfCardsXF<RoundsCardGameCardInformation>>();
            _discardGPile = new BasePileXF<RoundsCardGameCardInformation, ts, DeckOfCardsXF<RoundsCardGameCardInformation>>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<RoundsCardGameCardInformation, ts, DeckOfCardsXF<RoundsCardGameCardInformation>>();
            _trick1 = new TwoPlayerTrickXF<EnumSuitList, RoundsCardGameCardInformation, ts, DeckOfCardsXF<RoundsCardGameCardInformation>>();
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(RoundsCardGameMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            mainStack.Children.Add(otherStack);
            _score.AddColumn("# In Hand", true, nameof(RoundsCardGamePlayerItem.ObjectCount));
            _score.AddColumn("Tricks Won", true, nameof(RoundsCardGamePlayerItem.TricksWon));
            _score.AddColumn("Rounds Won", true, nameof(RoundsCardGamePlayerItem.RoundsWon));
            _score.AddColumn("Points", true, nameof(RoundsCardGamePlayerItem.CurrentPoints));
            _score.AddColumn("Total Score", true, nameof(RoundsCardGamePlayerItem.TotalScore));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(RoundsCardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Trump", nameof(RoundsCardGameMainViewModel.TrumpSuit));
            firstInfo.AddRow("Status", nameof(RoundsCardGameMainViewModel.Status));
            mainStack.Children.Add(_trick1);
            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(firstInfo.GetContent);

            mainStack.Children.Add(_score);
            //this is only a starting point.

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);

            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            RoundsCardGameSaveInfo save = cons!.Resolve<RoundsCardGameSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(_model.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _trick1!.Init(_model.TrickArea1!, ts.TagUsed);
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
