using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using TicTacToeCP;
using Xamarin.Forms;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
namespace TicTacToeXF
{
    public class GamePage : MultiPlayerPage<TicTacToeViewModel, TicTacToePlayerItem, TicTacToeSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            TicTacToeGraphicsCP tempBoard = OurContainer!.Resolve<TicTacToeGraphicsCP>();
            TicTacToeSaveInfo thisSave = OurContainer.Resolve<TicTacToeSaveInfo>();
            if (ScreenUsed == EnumScreen.LargeTablet)
                tempBoard.SpaceSize = 250;
            else
                tempBoard.SpaceSize = 100; //can experiment.
            _thisBoard!.CreateControls(thisSave.GameBoard);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            TicTacToeSaveInfo thisSave = OurContainer!.Resolve<TicTacToeSaveInfo>();
            _thisBoard!.UpdateControls(thisSave.GameBoard);
            return Task.CompletedTask;
        }
        GameBoardXF? _thisBoard;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            _thisBoard = new GameBoardXF();
            thisStack.Children.Add(_thisBoard);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(TicTacToeViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(TicTacToeViewModel.Status)); // this may have to show the status to begin with (?)
            thisStack.Children.Add(firstInfo.GetContent);
            MainGrid!.Children.Add(thisStack);
            AddRestoreCommand(thisStack); //usually to this.  can be to another control if needed.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<TicTacToePlayerItem, TicTacToeSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<TicTacToeViewModel>();

        }
    }
}