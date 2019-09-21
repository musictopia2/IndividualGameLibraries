using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using SolitaireBoardGameCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
namespace SolitaireBoardGameXF
{
    public class GamePage : SinglePlayerGamePage<SolitaireBoardGameViewModel>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }

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
        private SolitaireGameBoard? _thisBoard;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton!.VerticalOptions = LayoutOptions.Center;
            _thisBoard = new SolitaireGameBoard();
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(_thisBoard);
            Content = thisStack;
            ThisMod!.NewGameVisible = true;
            await ThisMod.StartNewGameAsync(); //can't put true because no autosave currently.
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<SolitaireBoardGameViewModel>(); //go ahead and use the custom processes for this.
        }
    }
}