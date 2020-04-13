using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.Misc;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using ThinkTwiceCP.Data;
using ThinkTwiceCP.Logic;
using ThinkTwiceCP.ViewModels;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper;
namespace ThinkTwiceWPF.Views
{
    public class ThinkTwiceMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly ThinkTwiceVMData _model;
        private readonly IGamePackageResolver _resolver;
        private readonly ThinkTwiceGameContainer _gameContainer;
        readonly ScoreBoardWPF _score;
        readonly DiceListControlWPF<SimpleDice> _diceControl; //i think.
        private readonly StackPanel _multStack;

        public ThinkTwiceMainView(IEventAggregator aggregator,
            TestOptions test, ThinkTwiceVMData model,
            IGamePackageResolver resolver,
            ThinkTwiceGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _resolver = resolver;
            _gameContainer = gameContainer;
            _aggregator.Subscribe(this);
            StackPanel mainStack = new StackPanel();
            _multStack = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            StackPanel firsts = new StackPanel();
            StackPanel otherStack = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            _diceControl = new DiceListControlWPF<SimpleDice>();
            firsts.Children.Add(_multStack);
            firsts.Children.Add(_diceControl);
            Grid grid = new Grid();
            StackPanel columnStack = new StackPanel();
            AddControlToGrid(grid, columnStack, 0, 0);
            AddLeftOverColumn(grid, 70);
            AddLeftOverColumn(grid, 30);
            mainStack.Children.Add(grid);
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(ThinkTwiceMainViewModel.RestoreScreen)
                };
            }
            var thisRoll = GetGamingButton("Roll Dice", nameof(ThinkTwiceMainViewModel.RollDiceAsync));
            var endButton = GetGamingButton("End Turn", nameof(ThinkTwiceMainViewModel.EndTurnAsync));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            _score = new ScoreBoardWPF();

            ParentSingleUIContainer parent = new ParentSingleUIContainer()
            {
                Name = nameof(ThinkTwiceMainViewModel.ScoreScreen)
            };
            AddControlToGrid(grid, parent, 0, 1);


            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(ThinkTwiceMainViewModel.NormalTurn)); // there is no roll number needed for this game.
            firstInfo.AddRow("Roll", nameof(ThinkTwiceMainViewModel.RollNumber)); //if you don't need, it comment it out.
            firstInfo.AddRow("Category", nameof(ThinkTwiceMainViewModel.CategoryChosen));
            firstInfo.AddRow("Score", nameof(ThinkTwiceMainViewModel.Score));
            firstInfo.AddRow("Status", nameof(ThinkTwiceMainViewModel.Status));

            firsts.Children.Add(firstInfo.GetContent);
            otherStack.Children.Add(firsts);

            StackPanel seconds = new StackPanel();
            var thisBut = GetGamingButton("Roll Multiplier Dice", nameof(ThinkTwiceMainViewModel.RollMultAsync));
            seconds.Children.Add(thisBut);
            seconds.Children.Add(thisRoll);
            seconds.Children.Add(endButton);
            otherStack.Children.Add(seconds);
            columnStack.Children.Add(otherStack);
            columnStack.Children.Add(_score);
            _score.AddColumn("Score Round", true, nameof(ThinkTwicePlayerItem.ScoreRound));
            _score.AddColumn("Score Game", true, nameof(ThinkTwicePlayerItem.ScoreGame));

            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.
            ThinkTwiceSaveInfo save = cons!.Resolve<ThinkTwiceSaveInfo>(); //usually needs this part for multiplayer games.
            _score!.LoadLists(save.PlayerList);
            _diceControl!.LoadDiceViewModel(_model.Cup!);
            CategoriesDice thisCat = _resolver.Resolve<CategoriesDice>(); //i think
            Multiplier thisMul = _resolver.Resolve<Multiplier>();
            CategoryWPF first = new CategoryWPF();
            first.SendDiceInfo(thisCat, _gameContainer);
            first.Margin = new Thickness(3, 3, 100, 3); //well see on tablets.
            _multStack!.Children.Add(first);
            MultWPF second = new MultWPF();
            second.SendDiceInfo(thisMul);
            second.Margin = new Thickness(0, 3, 0, 3);
            _multStack.Children.Add(second);


            return this.RefreshBindingsAsync(_aggregator);
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
