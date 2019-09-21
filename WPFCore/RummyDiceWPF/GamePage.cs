using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using CommonBasicStandardLibraries.CollectionClasses; //just in case i want to use the new custom classes.
using CommonBasicStandardLibraries.Exceptions;
using RummyDiceCP;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace RummyDiceWPF
{
    public class GamePage : MultiPlayerWindow<RummyDiceViewModel, RummyDicePlayerItem, RummyDiceSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            if (_mainGame!.TempSets!.Count == 0)
                throw new BasicBlankException("You need to have 2 sets, not none for tempsets");
            _mainGame.TempSets.ForEach(tempCP =>
            {
                RummyDiceHandWPF thisTemp = _tempList.Single(items => items.Index == tempCP.Index);
                thisTemp.LoadList(tempCP);
            });
            _diceControl!.LoadDiceViewModel(_mainGame);
            _thisScore!.LoadLists(_mainGame.PlayerList!); // i think
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            _mainGame!.TempSets!.ForEach(tempCP =>
            {
                RummyDiceHandWPF thisTemp = _tempList.Single(items => items.Index == tempCP.Index);
                thisTemp.UpdateList(tempCP);
            });
            _diceControl!.UpdateDiceViewModel(_mainGame);
            _thisScore!.UpdateLists(_mainGame.PlayerList!);
            return Task.CompletedTask;
        }
        RummyDiceMainGameClass? _mainGame;
        readonly CustomBasicList<RummyDiceHandWPF>? _tempList = new CustomBasicList<RummyDiceHandWPF>();
        private ScoreBoardWPF? _thisScore;
        private RummyDiceListWPF? _diceControl;
        protected async override void AfterGameButton()
        {
            StackPanel tempSets = new StackPanel();
            StackPanel thisStack = new StackPanel();
            RummyDiceHandWPF thisTemp = new RummyDiceHandWPF();
            thisTemp.Index = 1;
            tempSets.Children.Add(thisTemp);
            _tempList!.Add(thisTemp); //forgot this too.
            thisTemp = new RummyDiceHandWPF();
            thisTemp.Index = 2;
            tempSets.Children.Add(thisTemp);
            _tempList.Add(thisTemp);
            _mainGame = OurContainer!.Resolve<RummyDiceMainGameClass>();
            BasicSetUp();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(tempSets);
            _thisScore = new ScoreBoardWPF();
            otherStack.Children.Add(_thisScore);
            thisStack.Children.Add(otherStack);
            SimpleLabelGrid otherScore = new SimpleLabelGrid();
            otherScore.AddRow("Roll", nameof(RummyDiceViewModel.RollNumber), new RollConverter());
            otherScore.AddRow("Score", nameof(RummyDiceViewModel.Score));
            var tempGrid = otherScore.GetContent;
            tempGrid.VerticalAlignment = VerticalAlignment.Bottom; // try this way
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(tempGrid);
            Button thisBut;
            thisBut = GetGamingButton("Put Back", nameof(RummyDiceViewModel.BoardCommand));
            thisBut.Margin = new Thickness(5, 0, 0, 0);
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(otherStack);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _thisScore.AddColumn("Score Round", false, nameof(RummyDicePlayerItem.ScoreRound));
            _thisScore.AddColumn("Score Game", false, nameof(RummyDicePlayerItem.ScoreGame));
            _thisScore.AddColumn("Phase", false, nameof(RummyDicePlayerItem.Phase));
            _thisScore.VerticalAlignment = VerticalAlignment.Top;
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            thisStack.Children.Add(otherStack);
            _diceControl = new RummyDiceListWPF();
            thisBut = GetGamingButton("Roll", nameof(RummyDiceViewModel.RollCommand));
            thisBut.HorizontalAlignment = HorizontalAlignment.Left;
            otherStack.Children.Add(thisBut);
            otherStack.Children.Add(_diceControl);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(RummyDiceViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(RummyDiceViewModel.Status));
            firstInfo.AddRow("Phase", nameof(RummyDiceViewModel.CurrentPhase));
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            StackPanel bottomStack = new StackPanel();
            otherStack.Children.Add(bottomStack);
            var endButton = GetGamingButton("End Turn", nameof(RummyDiceViewModel.EndTurnCommand));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            endButton.Margin = new Thickness(5, 0, 0, 20);
            bottomStack.Children.Add(endButton);
            thisBut = GetGamingButton("Score Hand", nameof(RummyDiceViewModel.CheckCommand));
            thisBut.Margin = new Thickness(5, 0, 0, 0);
            bottomStack.Children.Add(thisBut);
            otherStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(otherStack);
            otherStack = new StackPanel();
            thisStack.Children.Add(otherStack);
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(GameButton);
            MainGrid!.Children.Add(thisStack);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<RummyDicePlayerItem, RummyDiceSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<RummyDiceViewModel>();
            OurContainer.RegisterSingleton<IGenerateDice<int>, RummyDiceInfo>();
        }
    }
}