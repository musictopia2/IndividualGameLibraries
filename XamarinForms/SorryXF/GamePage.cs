using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.GameBoards;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.GameGraphics.GamePieces;
using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicDrawables.MiscClasses;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using SorryCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace SorryXF
{
    public class GamePage : MultiPlayerPage<SorryViewModel, SorryPlayerItem, SorrySaveInfo>
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

        private EnumPickerXF<PawnPiecesCP<EnumColorChoice>, PawnPiecesXF<EnumColorChoice>,
            EnumColorChoice, ColorListChooser<EnumColorChoice>>? _thisColor;
        readonly CompleteGameBoardXF<GameBoardGraphicsCP> _thisBoard = new CompleteGameBoardXF<GameBoardGraphicsCP>();
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _thisColor = new EnumPickerXF<PawnPiecesCP<EnumColorChoice>, PawnPiecesXF<EnumColorChoice>,
            EnumColorChoice, ColorListChooser<EnumColorChoice>>();
            _chooseColorStack = new StackLayout();
            thisGrid.Children.Add(_chooseColorStack);
            SimpleLabelGridXF colorTurn = new SimpleLabelGridXF();
            colorTurn.AddRow("Turn", nameof(SorryViewModel.NormalTurn));
            colorTurn.AddRow("Instructions", nameof(SorryViewModel.Instructions));
            Binding thisBind = new Binding(nameof(SorryViewModel.ColorVisible));
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
            thisStack.Children.Add(_thisBoard);
            //_thisBoard.HorizontalOptions = LayoutOptions.Start;
            var endButton = GetGamingButton("End Turn", nameof(SorryViewModel.EndTurnCommand));
            endButton.HorizontalOptions = LayoutOptions.Start;
            thisStack.Children.Add(endButton);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(SorryViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(SorryViewModel.Instructions));
            firstInfo.AddRow("Status", nameof(SorryViewModel.Status));
            thisStack.Children.Add(firstInfo.GetContent);
            AddVerticalLabelGroup("Card Details", nameof(SorryViewModel.CardDetails), thisStack);
            MainGrid!.Children.Add(thisStack);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterSingleton<IProportionBoard, BoardProportion>(""); //here too.
            OurContainer!.RegisterType<BasicGameLoader<SorryPlayerItem, SorrySaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<SorryViewModel>();
            OurContainer.RegisterType<StandardPickerSizeClass>(); //i think this too.
            OurContainer.RegisterSingleton(_thisBoard.ThisElement, "");
            OurContainer.RegisterType<DrawShuffleClass<CardInfo, SorryPlayerItem>>();
            OurContainer.RegisterType<GenericCardShuffler<CardInfo>>();
            OurContainer.RegisterSingleton<IDeckCount, DeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(""); //i think.
            OurContainer.RegisterSingleton<IDrawCardNM, MultiPlayerDraw>();
        }
    }
    public class BoardProportion : IProportionBoard
    {
        float IProportionBoard.Proportion => 1.3f; //was originally 1.5
    }
}