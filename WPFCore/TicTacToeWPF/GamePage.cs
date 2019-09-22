using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TicTacToeCP;
namespace TicTacToeWPF
{
    public class GamePage : MultiPlayerWindow<TicTacToeViewModel, TicTacToePlayerItem, TicTacToeSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            TicTacToeGraphicsCP tempBoard = OurContainer!.Resolve<TicTacToeGraphicsCP>();
            TicTacToeSaveInfo thisSave = OurContainer.Resolve<TicTacToeSaveInfo>();
            tempBoard.SpaceSize = 250;
            _thisBoard!.CreateControls(thisSave.GameBoard);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            TicTacToeSaveInfo thisSave = OurContainer!.Resolve<TicTacToeSaveInfo>();
            _thisBoard!.UpdateControls(thisSave.GameBoard);
            return Task.CompletedTask;
        }
        GameBoardWPF? _thisBoard;
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            _thisBoard = new GameBoardWPF();
            thisStack.Children.Add(_thisBoard);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
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