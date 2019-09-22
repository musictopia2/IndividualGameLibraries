using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.GameGraphics.GamePieces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using MastermindCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using ss  = AndyCristinaGamePackageCP.DataClasses.GlobalStaticClass;
namespace MastermindXF
{
    public class GamePage : SinglePlayerGamePage<MastermindViewModel>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            return Task.CompletedTask;
        }
        private readonly BoardUI _gameBoard1 = new BoardUI();
        private readonly SolutionUI _solution1 = new SolutionUI(); //because of dependency injection.
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout();
            LevelUI levels = new LevelUI();
            EnumPickerXF<CirclePieceCP<EnumColorPossibilities>, CirclePieceXF<EnumColorPossibilities>, EnumColorPossibilities, CustomColorClass> colors = new EnumPickerXF<CirclePieceCP<EnumColorPossibilities>, CirclePieceXF<EnumColorPossibilities>, EnumColorPossibilities, CustomColorClass>();
            colors.Rows = 10; //try this way now.
            colors.HorizontalOptions = LayoutOptions.Center;
            var acceptBut = GetSmallerButton("Accept", nameof(MastermindViewModel.AcceptCommand));
            var giveUpBut = GetSmallerButton("Give Up", nameof(MastermindViewModel.GiveUpCommand));
            var levelBut = GetSmallerButton("Level" + Constants.vbCrLf + "Information", nameof(MastermindViewModel.LevelCommand));
            if (ss.ScreenUsed == EnumScreen.LargeTablet)
                _gameBoard1.WidthRequest = 820;
            else if (ss.ScreenUsed == EnumScreen.SmallTablet)
                _gameBoard1.WidthRequest = 600;
            else
                _gameBoard1.WidthRequest = 300;
            StackLayout otherStack = new StackLayout();
            MakeGameButtonSmaller(acceptBut);
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_gameBoard1);
            Grid testGrid = new Grid();
            testGrid.BackgroundColor = Color.Brown;
            if (ss.ScreenUsed == EnumScreen.LargeTablet)
                testGrid.WidthRequest = 200;
            else if (ss.ScreenUsed == EnumScreen.SmallTablet)
                testGrid.WidthRequest = 150;
            else
                testGrid.WidthRequest = 70;
            testGrid.Children.Add(colors);
            testGrid.Margin = new Thickness(0, 5, 0, 5);
            otherStack.Children.Add(testGrid);
            otherStack.Children.Add(thisStack);
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(acceptBut);
            thisStack.Children.Add(giveUpBut);
            thisStack.Children.Add(levelBut);
            thisStack = new StackLayout();
            thisStack.Children.Add(levels);
            thisStack.Children.Add(_solution1);
            otherStack.Children.Add(thisStack);
            Content = otherStack; //if not doing this, rethink.
            ThisMod!.Finish();
            _gameBoard1.Init(ThisMod);
            levels.Init(ThisMod);
            colors.LoadLists(ThisMod.Color1!);
            ThisMod.CommandContainer!.IsExecuting = false;
            ThisMod.NewGameVisible = true;
            await Task.CompletedTask;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<MastermindViewModel>(); //go ahead and use the custom processes for this.
            OurContainer!.RegisterSingleton(_gameBoard1);
            OurContainer.RegisterSingleton(_solution1);
            OurContainer.RegisterType<StandardPickerSizeClass>();
        }
    }
}