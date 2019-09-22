using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.GameGraphics.GamePieces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using ConnectTheDotsCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
namespace ConnectTheDotsXF
{
    public class GamePage : MultiPlayerPage<ConnectTheDotsViewModel, ConnectTheDotsPlayerItem, ConnectTheDotsSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
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
        private StackLayout? _chooseColorStack;

        private EnumPickerXF<CheckerChoiceCP<EnumColorChoice>, CheckerChooserXF<EnumColorChoice>,
            EnumColorChoice, ColorListChooser<EnumColorChoice>>? _thisColor;
        readonly GameBoardXF _thisBoard = new GameBoardXF();
        private ScoreBoardXF? _thisScore;
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _thisColor = new EnumPickerXF<CheckerChoiceCP<EnumColorChoice>, CheckerChooserXF<EnumColorChoice>, EnumColorChoice
                , ColorListChooser<EnumColorChoice>>();
            _chooseColorStack = new StackLayout();
            thisGrid.Children.Add(_chooseColorStack);
            SimpleLabelGridXF colorTurn = new SimpleLabelGridXF();
            colorTurn.AddRow("Turn", nameof(ConnectTheDotsViewModel.NormalTurn));
            colorTurn.AddRow("Instructions", nameof(ConnectTheDotsViewModel.Instructions));
            Binding thisBind = new Binding(nameof(ConnectTheDotsViewModel.ColorVisible));
            _chooseColorStack.SetBinding(IsVisibleProperty, thisBind);
            _chooseColorStack.Children.Add(_thisColor);
            _chooseColorStack.Children.Add(colorTurn.GetContent);
        }
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            _thisScore = new ScoreBoardXF();
            BasicSetUp();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(ConnectTheDotsViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(ConnectTheDotsViewModel.Status)); // this may have to show the status to begin with (?)
            _thisScore.AddColumn("Score", true, nameof(ConnectTheDotsPlayerItem.Score));
            thisStack.Children.Add(_thisBoard);
            thisStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(_thisScore);
            AddRestoreCommand(thisStack); //usually to this.  can be to another control if needed.
            MainGrid!.Children.Add(thisStack);// this is for sure needed everytime.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<ConnectTheDotsPlayerItem, ConnectTheDotsSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<ConnectTheDotsViewModel>();
            OurContainer.RegisterType<StandardPickerSizeClass>(); //i think this too.
            OurContainer.RegisterSingleton(_thisBoard.ThisElement, "");
            OurContainer.RegisterSingleton<IProportionBoard, StandardProportion>("");
        }
    }
}