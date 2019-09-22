using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BingoCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
namespace BingoXF
{
    public class GamePage : MultiPlayerPage<BingoViewModel, BingoPlayerItem, BingoSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }

        public override Task HandleAsync(LoadEventModel message)
        {
            _thisBoard!.CreateControls(_mainGame!.SaveRoot!.BingoBoard);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            _thisBoard!.UpdateControls(_mainGame!.SaveRoot!.BingoBoard); //hopefully still okay.
            return Task.CompletedTask;
        }
        private BingoBoardXF? _thisBoard;
        private BingoMainGameClass? _mainGame;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            _mainGame = OurContainer!.Resolve<BingoMainGameClass>();
            BasicSetUp();
            _thisBoard = new BingoBoardXF();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            SimpleLabelGridXF secondInfo = new SimpleLabelGridXF();
            secondInfo.FontSize = 30;
            secondInfo.AddRow("Number Called", nameof(BingoViewModel.NumberCalled));
            thisStack.Children.Add(secondInfo.GetContent);
            thisStack.Children.Add(_thisBoard);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Status", nameof(BingoViewModel.Status));
            thisStack.Children.Add(firstInfo.GetContent);
            var endButton = GetGamingButton("Bingo", nameof(BingoViewModel.BingoCommand)); // its bingo instead.
            endButton.HorizontalOptions = LayoutOptions.Start;
            thisStack.Children.Add(endButton);
            MainGrid!.Children.Add(thisStack); //try this too.
            AddRestoreCommand(thisStack); //usually to this.  can be to another control if needed.
            await FinishUpAsync();
        }
        //protected override void RegisterTests()
        //{
        //    ThisTest!.AllowAnyMove = true; //for now to see if clicking works.
        //}
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<BingoPlayerItem, BingoSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<BingoViewModel>();

        }
    }
}