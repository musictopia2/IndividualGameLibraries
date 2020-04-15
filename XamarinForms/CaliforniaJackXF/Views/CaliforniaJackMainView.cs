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
using CaliforniaJackCP.Cards;
using CaliforniaJackCP.Data;
using CaliforniaJackCP.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace CaliforniaJackXF.Views
{
    public class CaliforniaJackMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly CaliforniaJackVMData _model;
        private readonly BaseDeckXF<CaliforniaJackCardInformation, ts, DeckOfCardsXF<CaliforniaJackCardInformation>> _deckGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<CaliforniaJackCardInformation, ts, DeckOfCardsXF<CaliforniaJackCardInformation>> _playerHandWPF;

        private readonly TwoPlayerTrickXF<EnumSuitList, CaliforniaJackCardInformation, ts, DeckOfCardsXF<CaliforniaJackCardInformation>> _trick1;

        public CaliforniaJackMainView(IEventAggregator aggregator,
            TestOptions test,
            CaliforniaJackVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckXF<CaliforniaJackCardInformation, ts, DeckOfCardsXF<CaliforniaJackCardInformation>>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<CaliforniaJackCardInformation, ts, DeckOfCardsXF<CaliforniaJackCardInformation>>();
            _trick1 = new TwoPlayerTrickXF<EnumSuitList, CaliforniaJackCardInformation, ts, DeckOfCardsXF<CaliforniaJackCardInformation>>();
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(CaliforniaJackMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Cards Left", false, nameof(CaliforniaJackPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Tricks Won", true, nameof(CaliforniaJackPlayerItem.TricksWon), rightMargin: 10);
            _score.AddColumn("Points", true, nameof(CaliforniaJackPlayerItem.Points), rightMargin: 10);
            _score.AddColumn("Total Score", true, nameof(CaliforniaJackPlayerItem.TotalScore), rightMargin: 10);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(CaliforniaJackMainViewModel.NormalTurn));
            firstInfo.AddRow("Trump", nameof(CaliforniaJackMainViewModel.TrumpSuit));
            firstInfo.AddRow("Status", nameof(CaliforniaJackMainViewModel.Status));
            mainStack.Children.Add(_trick1);
            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(_score);

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);


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

            CaliforniaJackSaveInfo save = cons!.Resolve<CaliforniaJackSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think

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
