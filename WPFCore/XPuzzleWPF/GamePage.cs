using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using XPuzzleCP;
namespace XPuzzleWPF
{
    public class GamePage : SinglePlayerWindow<XPuzzleViewModel>
    {
        private XPuzzleGameBoard? _thisBoard;
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            XPuzzleSaveInfo thisSave = OurContainer!.Resolve<XPuzzleSaveInfo>();
            _thisBoard!.CreateControls(thisSave.SpaceList);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            return Task.CompletedTask;
        }
        protected async override void AfterGameButton()
        {
            _thisBoard = new XPuzzleGameBoard();
            StackPanel thisStack = new StackPanel();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(_thisBoard);
            Content = thisStack;
            ThisMod!.NewGameVisible = true;
            await ThisMod.StartNewGameAsync(); //if starting new game, has to be at end.
            ThisMod.CommandContainer!.IsExecuting = false; //i think.
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<XPuzzleViewModel>(); //go ahead and use the custom processes for this.  decided to mention non saved classes.
        }
    }
}