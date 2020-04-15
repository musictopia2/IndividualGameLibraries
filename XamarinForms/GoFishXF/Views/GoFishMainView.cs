using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using GoFishCP.Data;
using GoFishCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace GoFishXF.Views
{
    public class GoFishMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly GoFishVMData _model;
        private readonly BaseDeckXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> _deckGPile;
        private readonly BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> _playerHandXF;
        public GoFishMainView(IEventAggregator aggregator,
            TestOptions test,
            GoFishVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _discardGPile = new BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _score = new ScoreBoardXF();
            _playerHandXF = new BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(GoFishMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            mainStack.Children.Add(otherStack);
            var endButton = GetGamingButton("End Turn", nameof(GoFishMainViewModel.EndTurnAsync));
            endButton.HorizontalOptions = LayoutOptions.Start;
            endButton.VerticalOptions = LayoutOptions.Start;
            _score.AddColumn("Cards Left", true, nameof(GoFishPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Pairs", true, nameof(GoFishPlayerItem.Pairs));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(GoFishMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(GoFishMainViewModel.Status));
            StackLayout finalStack = new StackLayout();
            otherStack.Children.Add(finalStack);
            finalStack.Children.Add(endButton);
            finalStack.Children.Add(firstInfo.GetContent);
            finalStack.Children.Add(_score);
            mainStack.Children.Add(_playerHandXF);
            ParentSingleUIContainer ask = new ParentSingleUIContainer(nameof(GoFishMainViewModel.AskScreen));
            mainStack.Children.Add(ask);

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

            GoFishSaveInfo save = cons!.Resolve<GoFishSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandXF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(_model.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();

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
            return Task.CompletedTask;
        }
    }
}
