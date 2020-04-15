using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.BasicControls.TrickUIs;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using GermanWhistCP.Cards;
using GermanWhistCP.Data;
using GermanWhistCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace GermanWhistXF.Views
{
    public class GermanWhistMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly GermanWhistVMData _model;
        private readonly BaseDeckXF<GermanWhistCardInformation, ts, DeckOfCardsXF<GermanWhistCardInformation>> _deckGPile;
        private readonly BasePileXF<GermanWhistCardInformation, ts, DeckOfCardsXF<GermanWhistCardInformation>> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<GermanWhistCardInformation, ts, DeckOfCardsXF<GermanWhistCardInformation>> _playerHandWPF;

        private readonly TwoPlayerTrickXF<EnumSuitList, GermanWhistCardInformation, ts, DeckOfCardsXF<GermanWhistCardInformation>> _trick1;

        public GermanWhistMainView(IEventAggregator aggregator,
            TestOptions test,
            GermanWhistVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckXF<GermanWhistCardInformation, ts, DeckOfCardsXF<GermanWhistCardInformation>>();
            _discardGPile = new BasePileXF<GermanWhistCardInformation, ts, DeckOfCardsXF<GermanWhistCardInformation>>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<GermanWhistCardInformation, ts, DeckOfCardsXF<GermanWhistCardInformation>>();
            _trick1 = new TwoPlayerTrickXF<EnumSuitList, GermanWhistCardInformation, ts, DeckOfCardsXF<GermanWhistCardInformation>>();
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(GermanWhistMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Cards Left", false, nameof(GermanWhistPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Tricks Won", true, nameof(GermanWhistPlayerItem.TricksWon), rightMargin: 10);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(GermanWhistMainViewModel.NormalTurn));
            firstInfo.AddRow("Trump", nameof(GermanWhistMainViewModel.TrumpSuit));
            firstInfo.AddRow("Status", nameof(GermanWhistMainViewModel.Status));
            mainStack.Children.Add(_trick1);
            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(firstInfo.GetContent);

            mainStack.Children.Add(_score);

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

            GermanWhistSaveInfo save = cons!.Resolve<GermanWhistSaveInfo>(); //usually needs this part for multiplayer games.

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
