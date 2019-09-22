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
using ShipCaptainCrewCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace ShipCaptainCrewWPF
{
    public class GamePage : MultiPlayerWindow<ShipCaptainCrewViewModel, ShipCaptainCrewPlayerItem, ShipCaptainCrewSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            ShipCaptainCrewSaveInfo SaveRoot = OurContainer!.Resolve<ShipCaptainCrewSaveInfo>();
            _thisScore!.LoadLists(SaveRoot.PlayerList);
            _diceControl!.LoadDiceViewModel(ThisMod!.ThisCup!);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            ShipCaptainCrewSaveInfo SaveRoot = OurContainer!.Resolve<ShipCaptainCrewSaveInfo>();
            _thisScore!.UpdateLists(SaveRoot.PlayerList);
            _diceControl!.UpdateDice(ThisMod!.ThisCup!);
            return Task.CompletedTask;
        }
        ScoreBoardWPF? _thisScore;
        DiceListControlWPF<SimpleDice>? _diceControl; //i think.
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            MainGrid!.Children.Add(thisStack);
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            var thisRoll = GetGamingButton("Roll Dice", nameof(ShipCaptainCrewViewModel.RollCommand));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _diceControl = new DiceListControlWPF<SimpleDice>();
            otherStack.Children.Add(_diceControl);
            otherStack.Children.Add(thisRoll);
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardWPF();
            _thisScore.UseAbbreviationForTrueFalse = false;
            _thisScore.AddColumn("Score", true, nameof(ShipCaptainCrewPlayerItem.Score));
            _thisScore.AddColumn("Out", true, nameof(ShipCaptainCrewPlayerItem.WentOut), useTrueFalse: true);
            _thisScore.AddColumn("Wins", true, nameof(ShipCaptainCrewPlayerItem.Wins));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(ShipCaptainCrewViewModel.NormalTurn)); // there is no roll number needed for this game.
            firstInfo.AddRow("Roll", nameof(ShipCaptainCrewViewModel.RollNumber)); //if you don't need, it comment it out.
            firstInfo.AddRow("Status", nameof(ShipCaptainCrewViewModel.Status));
            thisStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(_thisScore);
            AddRestoreCommand(thisStack); //usually to this.  can be to another control if needed.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<ShipCaptainCrewPlayerItem, ShipCaptainCrewSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<ShipCaptainCrewViewModel>();
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, ShipCaptainCrewPlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceWPF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
        }
    }
}