using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.GameGraphics.GamePieces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using ConnectTheDotsCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace ConnectTheDotsWPF
{
    public class GamePage : MultiPlayerWindow<ConnectTheDotsViewModel, ConnectTheDotsPlayerItem, ConnectTheDotsSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            _thisColor!.LoadLists(ThisMod!.ColorChooser!);
            ConnectTheDotsSaveInfo saveRoot = OurContainer!.Resolve<ConnectTheDotsSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _thisBoard!.LoadBoard();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            ConnectTheDotsSaveInfo saveRoot = OurContainer!.Resolve<ConnectTheDotsSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            return Task.CompletedTask;
        }
        private StackPanel? _chooseColorStack;

        private EnumPickerWPF<CheckerChoiceCP<EnumColorChoice>, CheckerChooserWPF<EnumColorChoice>,
            EnumColorChoice, ColorListChooser<EnumColorChoice>>? _thisColor;
        readonly GameBoardWPF _thisBoard = new GameBoardWPF();
        private ScoreBoardWPF? _thisScore;
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _thisColor = new EnumPickerWPF<CheckerChoiceCP<EnumColorChoice>, CheckerChooserWPF<EnumColorChoice>, EnumColorChoice
                , ColorListChooser<EnumColorChoice>>();
            _chooseColorStack = new StackPanel();
            thisGrid.Children.Add(_chooseColorStack);
            SimpleLabelGrid colorTurn = new SimpleLabelGrid();
            colorTurn.AddRow("Turn", nameof(ConnectTheDotsViewModel.NormalTurn));
            colorTurn.AddRow("Instructions", nameof(ConnectTheDotsViewModel.Instructions));
            Binding thisBind = GetVisibleBinding(nameof(ConnectTheDotsViewModel.ColorVisible));
            _chooseColorStack.SetBinding(VisibilityProperty, thisBind);
            _chooseColorStack.Children.Add(_thisColor);
            _chooseColorStack.Children.Add(colorTurn.GetContent);
        }
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            _thisScore = new ScoreBoardWPF();
            BasicSetUp();
            _thisBoard!.Margin = new Thickness(3, 3, 3, 3);
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(ConnectTheDotsViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(ConnectTheDotsViewModel.Status)); // this may have to show the status to begin with (?)
            _thisScore.AddColumn("Score", true, nameof(ConnectTheDotsPlayerItem.Score));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_thisBoard);
            thisStack.Children.Add(otherStack);
            StackPanel finalStack = new StackPanel();
            finalStack.Children.Add(firstInfo.GetContent);
            finalStack.Children.Add(_thisScore);
            otherStack.Children.Add(finalStack);
            MainGrid!.Children.Add(thisStack); //maybe forgot this.
            AddRestoreCommand(thisStack); //usually to this.  can be to another control if needed.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<ConnectTheDotsPlayerItem, ConnectTheDotsSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer.RegisterNonSavedClasses<ConnectTheDotsViewModel>();
            OurContainer.RegisterSingleton(_thisBoard.ThisElement, "");
            OurContainer.RegisterSingleton<IProportionBoard, StandardProportion>("");
        }
    }
}