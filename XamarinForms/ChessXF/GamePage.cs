using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.GameGraphics.GamePieces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using ChessCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace ChessXF
{
    public class GamePage : MultiPlayerPage<ChessViewModel, ChessPlayerItem, ChessSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }

        public override Task HandleAsync(LoadEventModel message)
        {
            _thisColor!.LoadLists(ThisMod!.ColorChooser!);
            _thisBoard.LoadBoard();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            return Task.CompletedTask;
        }
        private StackLayout? _chooseColorStack;

        private EnumPickerXF<CheckerChoiceCP<EnumColorChoice>, CheckerChooserXF<EnumColorChoice>,
            EnumColorChoice, ColorListChooser<EnumColorChoice>>? _thisColor;
        private readonly GameBoardXF _thisBoard = new GameBoardXF();
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _thisColor = new EnumPickerXF<CheckerChoiceCP<EnumColorChoice>, CheckerChooserXF<EnumColorChoice>, EnumColorChoice
                , ColorListChooser<EnumColorChoice>>();
            _chooseColorStack = new StackLayout();
            thisGrid.Children.Add(_chooseColorStack);
            SimpleLabelGridXF colorTurn = new SimpleLabelGridXF();
            colorTurn.AddRow("Turn", nameof(ChessViewModel.NormalTurn));
            colorTurn.AddRow("Instructions", nameof(ChessViewModel.Instructions));
            Binding thisBind = new Binding(nameof(ChessViewModel.ColorVisible));
            _chooseColorStack.SetBinding(IsVisibleProperty, thisBind);
            _chooseColorStack.Children.Add(_thisColor);
            _chooseColorStack.Children.Add(colorTurn.GetContent);
        }
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            _thisBoard.Margin = new Thickness(3, 3, 3, 3);
            var thisBind = new Binding(nameof(ChessViewModel.MainOptionsVisible));
            thisStack.SetBinding(IsVisibleProperty, thisBind);
            var endButton = GetGamingButton("End Turn", nameof(ChessViewModel.EndTurnCommand));
            endButton.HorizontalOptions = LayoutOptions.Start;
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(ChessViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(ChessViewModel.Instructions));
            firstInfo.AddRow("Status", nameof(ChessViewModel.Status));
            thisStack.Children.Add(_thisBoard);
            StackLayout tempStack = new StackLayout();
            tempStack.Orientation = StackOrientation.Horizontal;
            tempStack.Children.Add(endButton);
            var thisBut = GetGamingButton("Show Tie", nameof(ChessViewModel.TieCommand));
            tempStack.Children.Add(thisBut);
            StackLayout FinalStack = new StackLayout();
            FinalStack.Children.Add(tempStack);
            FinalStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(FinalStack);
            AddRestoreCommand(thisStack); //usually to this.  can be to another control if needed.
            MainGrid!.Children.Add(thisStack);// this is for sure needed everytime.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterSingleton<IProportionBoard, CustomProportion>("main"); //here too.
            OurContainer!.RegisterType<BasicGameLoader<ChessPlayerItem, ChessSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<ChessViewModel>();
            OurContainer.RegisterType<StandardPickerSizeClass>(); //i think this too.
            OurContainer.RegisterSingleton(_thisBoard.ThisElement, "main");
        }
    }
}