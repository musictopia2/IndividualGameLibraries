using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.Misc;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.GameGraphics.Dice;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using DeadDie96CP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace DeadDie96XF
{
    public class GamePage : MultiPlayerPage<DeadDie96ViewModel, DeadDie96PlayerItem, DeadDie96SaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
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
        ScoreBoardXF? _thisScore;
        DiceListControlXF<TenSidedDice>? _diceControl; //i think.
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            MainGrid!.Children.Add(thisStack);
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            var thisRoll = GetGamingButton("Roll Dice", nameof(DeadDie96ViewModel.RollCommand));
            _diceControl = new DiceListControlXF<TenSidedDice>();
            thisStack.Children.Add(_diceControl);
            thisStack.Children.Add(thisRoll);
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Current Score", true, nameof(DeadDie96PlayerItem.CurrentScore));
            _thisScore.AddColumn("Total Score", true, nameof(DeadDie96PlayerItem.TotalScore));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(DeadDie96ViewModel.NormalTurn)); // there is no roll number needed for this game.
            firstInfo.AddRow("Status", nameof(DeadDie96ViewModel.Status));
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
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceXF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, TenSidedDice>();
        }
    }
}