using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using MinesweeperCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
namespace MinesweeperXF
{
    public class GamePage : SinglePlayerGamePage<MinesweeperViewModel>
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
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton!.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            Grid thisGrid = new Grid();
            AddLeftOverColumn(thisGrid, 50);
            AddLeftOverColumn(thisGrid, 25);
            AddLeftOverColumn(thisGrid, 25);
            AddControlToGrid(thisGrid, thisStack, 0, 1);
            GameboardXF thisBoard = new GameboardXF(ThisMod!);
            thisBoard.Margin = new Thickness(5, 5, 5, 5);
            thisStack.Margin = new Thickness(5, 5, 5, 5);
            AddControlToGrid(thisGrid, thisBoard, 0, 0);
            SimpleLabelGridXF thisLab = new SimpleLabelGridXF();
            thisLab.AddRow("Mines Needed", nameof(MinesweeperViewModel.HowManyMinesNeeded));
            thisLab.AddRow("Mines Left", nameof(MinesweeperViewModel.NumberOfMinesLeft));
            thisStack.Children.Add(thisLab.GetContent);
            ListChooserXF picker = new ListChooserXF();
            picker.LoadLists(ThisMod!.LevelPicker!);
            picker.Margin = new Thickness(5, 5, 5, 5);
            AddControlToGrid(thisGrid, picker, 0, 2);
            var ToggleBut = GetGamingButton("", nameof(MinesweeperViewModel.ChangeFlagCommand));
            var thisBind = new Binding(nameof(MinesweeperViewModel.ToggleVisible));
            ToggleBut.SetBinding(IsVisibleProperty, thisBind);
            IValueConverter thisCon;
            thisBind = new Binding(nameof(MinesweeperViewModel.IsFlagging));
            thisCon = new ToggleNameConverter();
            thisBind.Converter = thisCon;
            ToggleBut.SetBinding(Button.TextProperty, thisBind);
            thisCon = new ToggleColorConverter();
            thisBind = new Binding(nameof(MinesweeperViewModel.IsFlagging));
            thisBind.Converter = thisCon;
            ToggleBut.SetBinding(BackgroundColorProperty, thisBind);
            ToggleBut.Margin = new Thickness(5, 5, 5, 5);
            ToggleBut.HorizontalOptions = LayoutOptions.Start;
            ToggleBut.VerticalOptions = LayoutOptions.Start;
            thisStack.Children.Add(ToggleBut);
            Content = thisGrid; //if not doing this, rethink.
            ThisMod.NewGameVisible = true;
            ThisMod.CommandContainer!.IsExecuting = false;
            await Task.CompletedTask;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<MinesweeperViewModel>(); //go ahead and use the custom processes for this.
        }
    }
}