using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.Misc;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.GameGraphics.Dice;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using DeadDie96CP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace DeadDie96WPF
{
    public class GamePage : MultiPlayerWindow<DeadDie96ViewModel, DeadDie96PlayerItem, DeadDie96SaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            DeadDie96SaveInfo SaveRoot = OurContainer!.Resolve<DeadDie96SaveInfo>();
            _thisScore!.LoadLists(SaveRoot.PlayerList);
            _diceControl!.LoadDiceViewModel(ThisMod!.ThisCup!);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            DeadDie96SaveInfo SaveRoot = OurContainer!.Resolve<DeadDie96SaveInfo>();
            _thisScore!.UpdateLists(SaveRoot.PlayerList);
            _diceControl!.UpdateDice(ThisMod!.ThisCup!);
            return Task.CompletedTask;
        }
        ScoreBoardWPF? _thisScore;
        DiceListControlWPF<TenSidedDice>? _diceControl; //i think.
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            MainGrid!.Children.Add(thisStack);
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            var thisRoll = GetGamingButton("Roll Dice", nameof(DeadDie96ViewModel.RollCommand));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _diceControl = new DiceListControlWPF<TenSidedDice>();
            otherStack.Children.Add(_diceControl);
            otherStack.Children.Add(thisRoll);
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Current Score", true, nameof(DeadDie96PlayerItem.CurrentScore));
            _thisScore.AddColumn("Total Score", true, nameof(DeadDie96PlayerItem.TotalScore));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(DeadDie96ViewModel.NormalTurn)); // there is no roll number needed for this game.
            firstInfo.AddRow("Status", nameof(DeadDie96ViewModel.Status)); //if you don't need, it comment it out.
            thisStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(_thisScore);
            AddRestoreCommand(thisStack); //usually to this.  can be to another control if needed.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<DeadDie96PlayerItem, DeadDie96SaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<DeadDie96ViewModel>();
            OurContainer.RegisterType<StandardRollProcesses<TenSidedDice, DeadDie96PlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceWPF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, TenSidedDice>();
        }
    }
}