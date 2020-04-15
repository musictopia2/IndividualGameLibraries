using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.TrickUIs;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SnagCardGameCP.Cards;
using SnagCardGameCP.Data;
using SnagCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace SnagCardGameXF.Views
{
    public class SnagCardGameMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly SnagCardGameVMData _model;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<SnagCardGameCardInformation, ts, DeckOfCardsXF<SnagCardGameCardInformation>> _playerHandWPF;

        private readonly SeveralPlayersTrickXF<EnumSuitList, SnagCardGameCardInformation, ts, DeckOfCardsXF<SnagCardGameCardInformation>, SnagCardGamePlayerItem> _trick1;

        private readonly BaseHandXF<SnagCardGameCardInformation, ts, DeckOfCardsXF<SnagCardGameCardInformation>> _bar1;
        private readonly BaseHandXF<SnagCardGameCardInformation, ts, DeckOfCardsXF<SnagCardGameCardInformation>> _human1;
        private readonly BaseHandXF<SnagCardGameCardInformation, ts, DeckOfCardsXF<SnagCardGameCardInformation>> _opponent1;
        public SnagCardGameMainView(IEventAggregator aggregator,
            TestOptions test,
            SnagCardGameVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<SnagCardGameCardInformation, ts, DeckOfCardsXF<SnagCardGameCardInformation>>();
            _trick1 = new SeveralPlayersTrickXF<EnumSuitList, SnagCardGameCardInformation, ts, DeckOfCardsXF<SnagCardGameCardInformation>, SnagCardGamePlayerItem>();
            _bar1 = new BaseHandXF<SnagCardGameCardInformation, ts, DeckOfCardsXF<SnagCardGameCardInformation>>();
            _human1 = new BaseHandXF<SnagCardGameCardInformation, ts, DeckOfCardsXF<SnagCardGameCardInformation>>();
            _opponent1 = new BaseHandXF<SnagCardGameCardInformation, ts, DeckOfCardsXF<SnagCardGameCardInformation>>();
            _bar1.HandType = HandObservable<SnagCardGameCardInformation>.EnumHandList.Vertical;
            _bar1.HasAngles = true; //this is needed.

            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(SnagCardGameMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _score.AddColumn("Cards Won", true, nameof(SnagCardGamePlayerItem.CardsWon));
            _score.AddColumn("Current Points", true, nameof(SnagCardGamePlayerItem.CurrentPoints));
            _score.AddColumn("Total Points", true, nameof(SnagCardGamePlayerItem.TotalPoints));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(SnagCardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(SnagCardGameMainViewModel.Status));
            firstInfo.AddRow("Instructions", nameof(SnagCardGameMainViewModel.Instructions));

            StackLayout tempStack = new StackLayout();
            otherStack.Children.Add(_trick1);
            tempStack.Children.Add(_human1);
            tempStack.Children.Add(_opponent1);
            otherStack.Children.Add(tempStack);
            mainStack.Children.Add(otherStack);
            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(_score);
            Grid otherGrid = new Grid();
            AddLeftOverColumn(otherGrid, 30);
            AddLeftOverColumn(otherGrid, 70); // can always be adjusted
            AddControlToGrid(otherGrid, _bar1, 0, 0);
            AddControlToGrid(otherGrid, mainStack, 0, 1);

            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = otherGrid;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            SnagCardGameSaveInfo save = cons!.Resolve<SnagCardGameSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
            _trick1!.Init(_model.TrickArea1!, _model.TrickArea1, ts.TagUsed);

            _bar1!.LoadList(_model.Bar1!, ts.TagUsed);
            _human1!.LoadList(_model.Human1!, ts.TagUsed);
            _opponent1!.LoadList(_model.Opponent1!, ts.TagUsed);

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
