using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using XPuzzleCP;
namespace XPuzzleXF
{
    public class GamePage : SinglePlayerGamePage<XPuzzleViewModel>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
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
        private XPuzzleGameBoard? _thisBoard;
        protected override async Task AfterGameButtonAsync()
        {
            _thisBoard = new XPuzzleGameBoard();
            StackLayout thisStack = new StackLayout();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton!.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(_thisBoard);
            Content = thisStack;
            ThisMod!.NewGameVisible = true;
            await ThisMod.StartNewGameAsync(); //if starting new game, has to be at end.
            ThisMod.CommandContainer!.IsExecuting = false; //i think.
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<XPuzzleViewModel>(); //go ahead and use the custom processes for this.

        }
    }
}