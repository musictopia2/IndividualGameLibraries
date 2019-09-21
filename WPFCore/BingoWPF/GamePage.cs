using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.NetworkingClasses.Data;
using BingoCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
namespace BingoWPF
{
    public class GamePage : MultiPlayerWindow<BingoViewModel, BingoPlayerItem, BingoSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
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
        private BingoBoardWPF? _thisBoard;
        private BingoMainGameClass? _mainGame;
        protected async override void AfterGameButton()
        {

            //NetworkMessage output = new NetworkMessage();
            //output.SpecificPlayer = "hello";
            //output.YourNickName = "test";
            //output.Message = await js.SerializeObjectAsync(message);

            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            _mainGame = OurContainer!.Resolve<BingoMainGameClass>();
            BasicSetUp();
            _thisBoard = new BingoBoardWPF();
            AddAutoColumns(MainGrid!, 2);
            StackPanel tempStack = new StackPanel();
            SimpleLabelGrid secondInfo = new SimpleLabelGrid();
            secondInfo.FontSize = 40;
            secondInfo.AddRow("Number Called", nameof(BingoViewModel.NumberCalled));
            tempStack.Children.Add(secondInfo.GetContent);
            _thisBoard = new BingoBoardWPF();
            _thisBoard.Margin = new Thickness(5, 5, 5, 5);
            tempStack.Children.Add(_thisBoard);
            AddControlToGrid(MainGrid!, tempStack, 0, 0);
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Status", nameof(BingoViewModel.Status));
            thisStack.Children.Add(firstInfo.GetContent);
            AddControlToGrid(MainGrid!, thisStack, 0, 1);
            var endButton = GetGamingButton("Bingo", nameof(BingoViewModel.BingoCommand)); // its bingo instead.
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            thisStack.Children.Add(endButton);
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
            //anything else that needs to be registered will be here.
        }
    }
}