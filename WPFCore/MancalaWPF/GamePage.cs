using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using MancalaCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
namespace MancalaWPF
{
    public class GamePage : MultiPlayerWindow<MancalaViewModel, MancalaPlayerItem, MancalaSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            _thisBoard.LoadBoard();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            return Task.CompletedTask;
        }
        readonly GameBoardWPF _thisBoard = new GameBoardWPF();
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(MancalaViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(MancalaViewModel.Status));
            firstInfo.AddRow("Instructions", nameof(MancalaViewModel.Instructions));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            thisStack.Children.Add(otherStack);
            otherStack.Children.Add(_thisBoard);
            otherStack.Children.Add(firstInfo.GetContent);
            MainGrid!.Children.Add(thisStack); //maybe i forgot that too.
            AddRestoreCommand(thisStack); //usually to this.  can be to another control if needed.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<MancalaPlayerItem, MancalaSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<MancalaViewModel>();
            OurContainer.RegisterSingleton(_thisBoard.ThisElement, "");
            OurContainer.RegisterSingleton<IProportionBoard, StandardProportion>(""); //here too.
        }
    }
}