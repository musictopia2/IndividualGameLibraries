using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DrawableListsObservable;
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
using Rummy500CP.Data;
using Rummy500CP.Logic;
using Rummy500CP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace Rummy500XF.Views
{
    public class Rummy500MainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly Rummy500VMData _model;
        private readonly BaseDeckXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>> _deckGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>> _playerHandWPF;

        private readonly BaseHandXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>> _discardRummy;
        private readonly MainRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>, RummySet, SavedSet> _mainG;


        public Rummy500MainView(IEventAggregator aggregator,
            TestOptions test,
            Rummy500VMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _playerHandWPF.HorizontalOptions = LayoutOptions.Fill;
            _discardRummy = new BaseHandXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _mainG = new MainRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>, RummySet, SavedSet>();


            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(Rummy500MainViewModel.RestoreScreen));
            }

            Grid finalGrid = new Grid();
            AddAutoColumns(finalGrid, 1);
            AddLeftOverColumn(finalGrid, 1);
            AddControlToGrid(finalGrid, mainStack, 0, 1);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(Rummy500MainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(Rummy500MainViewModel.Status));

            _score.AddColumn("Cards Left", false, nameof(Rummy500PlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Points Played", false, nameof(Rummy500PlayerItem.PointsPlayed));
            _score.AddColumn("Cards Played", false, nameof(Rummy500PlayerItem.CardsPlayed));
            _score.AddColumn("Score Current", false, nameof(Rummy500PlayerItem.CurrentScore));
            _score.AddColumn("Score Total", false, nameof(Rummy500PlayerItem.TotalScore));
            otherStack.Children.Add(_score);
            otherStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(otherStack);
            mainStack.Children.Add(_playerHandWPF);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            mainStack.Children.Add(otherStack);
            Button button;
            button = GetGamingButton("Discard Current", nameof(Rummy500MainViewModel.DiscardCurrentAsync));
            button.HorizontalOptions = LayoutOptions.Start;
            button.VerticalOptions = LayoutOptions.Start;
            otherStack.Children.Add(button);
            button = GetGamingButton("Create New Rummy Set", nameof(Rummy500MainViewModel.CreateSetAsync));
            button.HorizontalOptions = LayoutOptions.Start;
            button.VerticalOptions = LayoutOptions.Start;
            otherStack.Children.Add(button);
            mainStack.Children.Add(otherStack);
            _mainG.Divider = 1.3;
            mainStack.Children.Add(_mainG);
            _discardRummy.Divider = 1.4;
            _discardRummy.ExtraControlSpace = 20;

            _discardRummy.HandType = HandObservable<RegularRummyCard>.EnumHandList.Vertical;
            _discardRummy.HorizontalOptions = LayoutOptions.Start;
            _discardRummy.VerticalOptions = LayoutOptions.FillAndExpand;
            AddControlToGrid(finalGrid, _discardRummy, 0, 0);


            _deckGPile.Margin = new Thickness(5, 5, 5, 5);


            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            Rummy500SaveInfo save = cons!.Resolve<Rummy500SaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think

            _deckGPile!.Init(_model.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();

            
            _mainG!.Init(_model.MainSets1!, ts.TagUsed);

            Content = finalGrid;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            _discardRummy!.LoadList(_model.DiscardList1!, ts.TagUsed);


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
