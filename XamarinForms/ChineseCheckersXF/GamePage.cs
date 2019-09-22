using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.GameBoards;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.GameGraphics.GamePieces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using ChineseCheckersCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace ChineseCheckersXF
{
    public class GamePage : MultiPlayerPage<ChineseCheckersViewModel, ChineseCheckersPlayerItem, ChineseCheckersSaveInfo>
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

        private EnumPickerXF<MarblePiecesCP<EnumColorList>, MarblePiecesXF<EnumColorList>,
            EnumColorList, ColorListChooser<EnumColorList>>? _thisColor;
        private readonly CompleteGameBoardXF<GameBoardGraphicsCP> _thisBoard = new CompleteGameBoardXF<GameBoardGraphicsCP>();
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _thisColor = new EnumPickerXF<MarblePiecesCP<EnumColorList>, MarblePiecesXF<EnumColorList>,
            EnumColorList, ColorListChooser<EnumColorList>>();
            _chooseColorStack = new StackLayout();
            thisGrid.Children.Add(_chooseColorStack);
            SimpleLabelGridXF colorTurn = new SimpleLabelGridXF();
            colorTurn.AddRow("Turn", nameof(ChineseCheckersViewModel.NormalTurn));
            colorTurn.AddRow("Instructions", nameof(ChineseCheckersViewModel.Instructions));
            Binding thisBind = new Binding(nameof(ChineseCheckersViewModel.ColorVisible));
            _chooseColorStack.SetBinding(IsVisibleProperty, thisBind);
            _chooseColorStack.Children.Add(_thisColor);
            _chooseColorStack.Children.Add(colorTurn.GetContent);
        }
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout();
            BasicSetUp();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            //_thisBoard.Margin = new Thickness(-10, 0, 0, 0);
            _thisBoard.HorizontalOptions = LayoutOptions.Start;
            _thisBoard.VerticalOptions = LayoutOptions.Start;
            thisStack.Children.Add(_thisBoard);

            //Grid tempGrid = new Grid();
            //AddPixelColumn(tempGrid, 300);
            //AddAutoColumns(tempGrid, 1);
            var endButton = GetGamingButton("End Turn", nameof(ChineseCheckersViewModel.EndTurnCommand));
            endButton.HorizontalOptions = LayoutOptions.Start;
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(ChineseCheckersViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(ChineseCheckersViewModel.Instructions));
            firstInfo.AddRow("Status", nameof(ChineseCheckersViewModel.Status));
            //StackLayout tempStack = new StackLayout();
            thisStack.Children.Add(endButton);
            thisStack.Children.Add(firstInfo.GetContent);
            //AddControlToGrid(tempGrid, tempStack, 0, 0);
            //AddControlToGrid(tempGrid, _thisBoard, 0, 1);
            //thisStack.Children.Add(tempGrid);
            MainGrid!.Children.Add(thisStack);
            //AddRestoreCommand(tempStack);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterSingleton<IProportionBoard, StandardProportion>("main"); //here too.
            OurContainer!.RegisterType<BasicGameLoader<ChineseCheckersPlayerItem, ChineseCheckersSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<ChineseCheckersViewModel>();
            OurContainer.RegisterType<StandardPickerSizeClass>(); //i think this too.
            OurContainer.RegisterSingleton(_thisBoard.ThisElement, "main");
        }
    }
}