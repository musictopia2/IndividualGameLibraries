using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.GameGraphics.GamePieces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using MastermindCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace MastermindWPF
{
    public class GamePage : SinglePlayerWindow<MastermindViewModel>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            return Task.CompletedTask;
        }
        private readonly BoardUI _gameBoard1 = new BoardUI();
        private readonly SolutionUI _solution1 = new SolutionUI(); //because of dependency injection.
        protected override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel();
            LevelUI levels = new LevelUI();
            EnumPickerWPF<CirclePieceCP<EnumColorPossibilities>, CirclePieceWPF<EnumColorPossibilities>, EnumColorPossibilities, CustomColorClass> colors = new EnumPickerWPF<CirclePieceCP<EnumColorPossibilities>, CirclePieceWPF<EnumColorPossibilities>, EnumColorPossibilities, CustomColorClass>();
            colors.Rows = 10; //try this way now.
            colors.HorizontalAlignment = HorizontalAlignment.Center;
            var acceptBut = GetGamingButton("Accept", nameof(MastermindViewModel.AcceptCommand));
            var giveUpBut = GetGamingButton("Give Up", nameof(MastermindViewModel.GiveUpCommand));
            var levelBut = GetGamingButton("Level" + Constants.vbCrLf + "Information", nameof(MastermindViewModel.LevelCommand));
            _gameBoard1.Width = 820;
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_gameBoard1);
            Grid testGrid = new Grid();
            testGrid.Background = Brushes.Brown;
            testGrid.Width = 200;
            testGrid.Children.Add(colors);
            testGrid.Margin = new Thickness(10, 10, 10, 10);
            otherStack.Children.Add(testGrid);
            otherStack.Children.Add(thisStack);
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(acceptBut);
            thisStack.Children.Add(giveUpBut);
            thisStack.Children.Add(levelBut);
            thisStack = new StackPanel();
            thisStack.Children.Add(levels);
            thisStack.Children.Add(_solution1);
            otherStack.Children.Add(thisStack);
            Content = otherStack; //if not doing this, rethink.
            ThisMod!.Finish();
            _gameBoard1.Init(ThisMod);
            levels.Init(ThisMod);
            colors.LoadLists(ThisMod.Color1!);
            ThisMod.CommandContainer!.IsExecuting = false;
            ThisMod.NewGameVisible = true;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<MastermindViewModel>(); //go ahead and use the custom processes for this.  decided to mention non saved classes.
            OurContainer!.RegisterSingleton(_gameBoard1);
            OurContainer.RegisterSingleton(_solution1);
        }
    }
}