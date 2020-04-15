using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.Converters;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FluxxCP.Cards;
using FluxxCP.Containers;
using FluxxCP.Data;
using FluxxCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace FluxxXF.Views
{
    public class FluxxMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly FluxxVMData _model;
        private readonly FluxxGameContainer _gameContainer;
        private readonly BaseDeckXF<FluxxCardInformation, FluxxGraphicsCP, CardGraphicsXF> _deckGPile;
        private readonly BasePileXF<FluxxCardInformation, FluxxGraphicsCP, CardGraphicsXF> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<FluxxCardInformation, FluxxGraphicsCP, CardGraphicsXF> _playerHandWPF;

        readonly ShowCardUI _cardDetail1;
        readonly RuleUI _rule1 = new RuleUI();
        readonly GoalHandXF _goal1 = new GoalHandXF();
        readonly KeeperHandXF _keeperHand1 = new KeeperHandXF();

        public FluxxMainView(IEventAggregator aggregator,
            TestOptions test,
            FluxxVMData model,
            ActionContainer actionContainer,
            KeeperContainer keeperContainer,
            FluxxGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _gameContainer = gameContainer;
            _aggregator.Subscribe(this);
            _cardDetail1 = new ShowCardUI(model, actionContainer, keeperContainer, EnumShowCategory.MainScreen);
            _cardDetail1.WidthRequest = 700;
            _deckGPile = new BaseDeckXF<FluxxCardInformation, FluxxGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<FluxxCardInformation, FluxxGraphicsCP, CardGraphicsXF>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<FluxxCardInformation, FluxxGraphicsCP, CardGraphicsXF>();
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(FluxxMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_rule1);
            otherStack.Children.Add(_cardDetail1);
            otherStack.Children.Add(_goal1);
            mainStack.Children.Add(otherStack); //possibly forgot this.

            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            mainStack.Children.Add(otherStack);
            var endButton = GetSmallerButton("End Turn", nameof(FluxxMainViewModel.EndTurnAsync));
            otherStack.Children.Add(endButton);
            var button = GetSmallerButton("Discard", nameof(FluxxMainViewModel.DiscardAsync));
            otherStack.Children.Add(button);

            ParentSingleUIContainer parent = new ParentSingleUIContainer(nameof(FluxxMainViewModel.PlayGiveScreen))
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start
            };
            otherStack.Children.Add(parent);

            button = GetSmallerButton("Unselect All", nameof(FluxxMainViewModel.UnselectHandCards));
            otherStack.Children.Add(button);
            button = GetSmallerButton("Select All", nameof(FluxxMainViewModel.SelectHandCards));
            otherStack.Children.Add(button);
            button = GetSmallerButton("Show Keepers", nameof(FluxxMainViewModel.ShowKeepersAsync));
            otherStack.Children.Add(button);
            Grid tempGrid = new Grid();
            AddLeftOverColumn(tempGrid, 75);
            AddAutoColumns(tempGrid, 1);
            TrueFalseConverter tConverter = new TrueFalseConverter();
            tConverter.UseAbb = false;
            DetailGameInformationXF detail1 = new DetailGameInformationXF();
            detail1.Margin = new Thickness(3, 3, 3, 3);
            {
                var withBlock = detail1;
                withBlock.AddRow("Plays Left", nameof(FluxxMainViewModel.PlaysLeft));
                withBlock.AddRow("Hand Limit", nameof(FluxxMainViewModel.HandLimit));
                withBlock.AddRow("Keeper Limit", nameof(FluxxMainViewModel.KeeperLimit));
                withBlock.AddRow("Play Limit", nameof(FluxxMainViewModel.PlayLimit));
                withBlock.AddRow("Another Turn", nameof(FluxxMainViewModel.AnotherTurn), tConverter);
                withBlock.AddRow("Current Turn", nameof(FluxxMainViewModel.NormalTurn));
                withBlock.AddRow("Other Turn", nameof(FluxxMainViewModel.OtherTurn));
                withBlock.AddRow("Status", nameof(FluxxMainViewModel.Status));
                withBlock.AddRow("Draw Bonus", nameof(FluxxMainViewModel.DrawBonus));
                withBlock.AddRow("Play Bonus", nameof(FluxxMainViewModel.PlayBonus));
                withBlock.AddRow("Cards Drawn", nameof(FluxxMainViewModel.CardsDrawn));
                withBlock.AddRow("Cards Played", nameof(FluxxMainViewModel.CardsPlayed));
                withBlock.AddRow("Draw Rules", nameof(FluxxMainViewModel.DrawRules));
                withBlock.AddRow("Previous" + Constants.vbCrLf + "Bonus", nameof(FluxxMainViewModel.PreviousBonus));
            }
            mainStack.Children.Add(tempGrid);
            StackLayout finalStack = new StackLayout();
            AddControlToGrid(tempGrid, detail1, 0, 1);
            AddControlToGrid(tempGrid, finalStack, 0, 0);

            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            mainStack.Children.Add(otherStack); //possibly forgot this.
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.

            otherStack.Children.Add(_keeperHand1);
            finalStack.Children.Add(otherStack);
            _score.UseAbbreviationForTrueFalse = true; // this time has to be abbreviated
            _score.AddColumn("# In Hand", false, nameof(FluxxPlayerItem.ObjectCount));
            _score.AddColumn("# Of Keepers", false, nameof(FluxxPlayerItem.NumberOfKeepers));
            _score.AddColumn("Bread", false, nameof(FluxxPlayerItem.Bread), useTrueFalse: true);
            _score.AddColumn("Chocolate", false, nameof(FluxxPlayerItem.Chocolate), useTrueFalse: true);
            _score.AddColumn("Cookies", false, nameof(FluxxPlayerItem.Cookies), useTrueFalse: true);
            _score.AddColumn("Death", false, nameof(FluxxPlayerItem.Death), useTrueFalse: true);
            _score.AddColumn("Dreams", false, nameof(FluxxPlayerItem.Dreams), useTrueFalse: true);
            _score.AddColumn("Love", false, nameof(FluxxPlayerItem.Love), useTrueFalse: true);
            _score.AddColumn("Milk", false, nameof(FluxxPlayerItem.Milk), useTrueFalse: true);
            _score.AddColumn("Money", false, nameof(FluxxPlayerItem.Money), useTrueFalse: true);
            _score.AddColumn("Peace", false, nameof(FluxxPlayerItem.Peace), useTrueFalse: true);
            _score.AddColumn("Sleep", false, nameof(FluxxPlayerItem.Sleep), useTrueFalse: true);
            _score.AddColumn("Television", false, nameof(FluxxPlayerItem.Television), useTrueFalse: true);
            _score.AddColumn("The Brain", false, nameof(FluxxPlayerItem.TheBrain), useTrueFalse: true);
            _score.AddColumn("The Moon", false, nameof(FluxxPlayerItem.TheMoon), useTrueFalse: true);
            _score.AddColumn("The Rocket", false, nameof(FluxxPlayerItem.TheRocket), useTrueFalse: true);
            _score.AddColumn("The Sun", false, nameof(FluxxPlayerItem.TheSun), useTrueFalse: true);
            _score.AddColumn("The Toaster", false, nameof(FluxxPlayerItem.TheToaster), useTrueFalse: true);
            _score.AddColumn("Time", false, nameof(FluxxPlayerItem.Time), useTrueFalse: true);
            _score.AddColumn("War", false, nameof(FluxxPlayerItem.War), useTrueFalse: true);


            _playerHandWPF.Divider = 1.2;
            finalStack.Children.Add(_playerHandWPF);
            _keeperHand1.MinimumWidthRequest = 300;
            finalStack.Children.Add(_score);


            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);

            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            FluxxSaveInfo save = cons!.Resolve<FluxxSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ""); // i think
            _discardGPile!.Init(_model.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _rule1.LoadControls(_gameContainer);
            _goal1.LoadList(_model.Goal1!, "");
            _keeperHand1.LoadList(_model.Keeper1!, "");

            Content = mainStack;
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
            _discardGPile.StopListening();
            _deckGPile.StopListening();
            return Task.CompletedTask;
        }
    }
}
