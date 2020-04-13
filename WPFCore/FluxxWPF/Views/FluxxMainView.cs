using BasicControlsAndWindowsCore.BasicWindows.BasicConverters;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FluxxCP.Cards;
using FluxxCP.Containers;
using FluxxCP.Data;
using FluxxCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace FluxxWPF.Views
{
    public class FluxxMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly FluxxVMData _model;
        private readonly FluxxGameContainer _gameContainer;
        private readonly BaseDeckWPF<FluxxCardInformation, FluxxGraphicsCP, CardGraphicsWPF> _deckGPile;
        private readonly BasePileWPF<FluxxCardInformation, FluxxGraphicsCP, CardGraphicsWPF> _discardGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<FluxxCardInformation, FluxxGraphicsCP, CardGraphicsWPF> _playerHandWPF;

        readonly ShowCardUI _cardDetail1;
        readonly RuleUI _rule1 = new RuleUI();
        readonly GoalHandWPF _goal1 = new GoalHandWPF();
        readonly KeeperHandWPF _keeperHand1 = new KeeperHandWPF();

        

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
            _deckGPile = new BaseDeckWPF<FluxxCardInformation, FluxxGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<FluxxCardInformation, FluxxGraphicsCP, CardGraphicsWPF>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<FluxxCardInformation, FluxxGraphicsCP, CardGraphicsWPF>();
            _cardDetail1.Width = 700;
            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(FluxxMainViewModel.RestoreScreen)
                };
            }


            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_rule1);
            otherStack.Children.Add(_cardDetail1);
            otherStack.Children.Add(_goal1);
            mainStack.Children.Add(otherStack); //possibly forgot this.
            
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            mainStack.Children.Add(otherStack);
            var endButton = GetGamingButton("End Turn", nameof(FluxxMainViewModel.EndTurnAsync));
            otherStack.Children.Add(endButton);
            var button = GetGamingButton("Discard", nameof(FluxxMainViewModel.DiscardAsync));
            otherStack.Children.Add(button);

            ParentSingleUIContainer parent = new ParentSingleUIContainer()
            {
                Name = nameof(FluxxMainViewModel.PlayGiveScreen),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            otherStack.Children.Add(parent);

            button = GetGamingButton("Unselect All", nameof(FluxxMainViewModel.UnselectHandCards));
            otherStack.Children.Add(button);
            button = GetGamingButton("Select All", nameof(FluxxMainViewModel.SelectHandCards));
            otherStack.Children.Add(button);
            button = GetGamingButton("Show Keepers", nameof(FluxxMainViewModel.ShowKeepersAsync));
            otherStack.Children.Add(button);
            Grid tempGrid = new Grid();
            AddLeftOverColumn(tempGrid, 75);
            AddAutoColumns(tempGrid, 1);
            TrueFalseConverter tConverter = new TrueFalseConverter();
            tConverter.UseAbb = false;
            DetailGameInformationWPF detail1 = new DetailGameInformationWPF();
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
            StackPanel finalStack = new StackPanel();
            AddControlToGrid(tempGrid, detail1, 0, 1);
            AddControlToGrid(tempGrid, finalStack, 0, 0);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _keeperHand1.HandType = HandObservable<KeeperCard>.EnumHandList.Horizontal;
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
            _playerHandWPF = new FluxxHandWPF();
            _playerHandWPF.Divider = 1.2;
            finalStack.Children.Add(_playerHandWPF);
            _keeperHand1.MinWidth = 300;
            finalStack.Children.Add(_score);


            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {


            return Task.CompletedTask;
            //return this.RefreshBindingsAsync(_aggregator);
        }



        Task IUIView.TryActivateAsync()
        {
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
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _discardGPile.StopListening();
            _deckGPile.StopListening();
            return Task.CompletedTask;
        }
    }
}
