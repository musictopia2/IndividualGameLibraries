using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using SolitaireBoardGameCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
namespace SolitaireBoardGameWPF
{
    public class GamePage : SinglePlayerWindow<SolitaireBoardGameViewModel>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        private SolitaireGameBoard? _thisBoard;
        public override Task HandleAsync(LoadEventModel message)
        {
            SolitaireBoardGameSaveInfo ThisSave = OurContainer!.Resolve<SolitaireBoardGameSaveInfo>();
            _thisBoard!.CreateControls(ThisSave.SpaceList);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            return Task.CompletedTask;
        }
        protected async override void AfterGameButton()
        {
            _thisBoard = new SolitaireGameBoard();
            StackPanel thisStack = new StackPanel();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(_thisBoard);
            Content = thisStack;
            ThisMod!.NewGameVisible = true;
            await ThisMod.StartNewGameAsync(); //can't put true because no autosave currently.
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<SolitaireBoardGameViewModel>(); //go ahead and use the custom processes for this.  decided to mention non saved classes.
        }
    }
}