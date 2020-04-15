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
using CrazyEightsCP.Data;
using CrazyEightsCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace CrazyEightsXF.Views
{
    public class CrazyEightsMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly CrazyEightsVMData _model;
        private readonly BaseDeckXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> _deckGPile;
        private readonly BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> _playerHandWPF;
        public CrazyEightsMainView(IEventAggregator aggregator,
            TestOptions test,
            CrazyEightsVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _discardGPile = new BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(CrazyEightsMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Cards Left", false, nameof(CrazyEightsPlayerItem.ObjectCount)); //very common.
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(CrazyEightsMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(CrazyEightsMainViewModel.Status));

            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(firstInfo.GetContent);

            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            mainStack.Children.Add(otherStack);
            otherStack.Children.Add(_score);

            ParentSingleUIContainer parent = new ParentSingleUIContainer(nameof(CrazyEightsMainViewModel.SuitScreen));
            mainStack.Children.Add(parent);
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

            CrazyEightsSaveInfo save = cons!.Resolve<CrazyEightsSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
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
