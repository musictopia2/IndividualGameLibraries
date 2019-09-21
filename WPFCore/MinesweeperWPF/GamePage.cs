using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BasicControlsAndWindowsCore.BasicWindows.BasicConverters;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using MinesweeperCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
namespace MinesweeperWPF
{
    public class GamePage : SinglePlayerWindow<MinesweeperViewModel>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            return Task.CompletedTask;
        }
        protected override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            Grid thisGrid = new Grid();
            AddLeftOverColumn(thisGrid, 50);
            AddLeftOverColumn(thisGrid, 25);
            AddLeftOverColumn(thisGrid, 25);
            AddControlToGrid(thisGrid, thisStack, 0, 1);
            GameboardWPF thisBoard = new GameboardWPF(ThisMod!);
            thisBoard.Margin = new Thickness(5, 5, 5, 5);
            thisStack.Margin = new Thickness(5, 5, 5, 5);
            AddControlToGrid(thisGrid, thisBoard, 0, 0);
            SimpleLabelGrid thisLab = new SimpleLabelGrid();
            thisLab.AddRow("Mines Needed", nameof(MinesweeperViewModel.HowManyMinesNeeded));
            thisLab.AddRow("Mines Left", nameof(MinesweeperViewModel.NumberOfMinesLeft));
            thisStack.Children.Add(thisLab.GetContent);
            ListChooserWPF picker = new ListChooserWPF();
            picker.LoadLists(ThisMod!.LevelPicker!);
            picker.Margin = new Thickness(5, 5, 5, 5);
            AddControlToGrid(thisGrid, picker, 0, 2);
            VisibilityConverter thisV = new VisibilityConverter();
            thisV.UseCollapsed = true;
            var ToggleBut = GetGamingButton("", nameof(MinesweeperViewModel.ChangeFlagCommand));
            var thisBind = new Binding(nameof(MinesweeperViewModel.ToggleVisible));
            thisBind.Converter = thisV;
            ToggleBut.SetBinding(VisibilityProperty, thisBind);
            IValueConverter thisCon;
            thisBind = new Binding(nameof(MinesweeperViewModel.IsFlagging));
            thisCon = new ToggleNameConverter();
            thisBind.Converter = thisCon;
            ToggleBut.SetBinding(ContentProperty, thisBind);
            thisCon = new ToggleColorConverter();
            thisBind = new Binding(nameof(MinesweeperViewModel.IsFlagging));
            thisBind.Converter = thisCon;
            ToggleBut.SetBinding(BackgroundProperty, thisBind);
            ToggleBut.Margin = new Thickness(5, 5, 5, 5);
            ToggleBut.HorizontalAlignment = HorizontalAlignment.Left;
            ToggleBut.VerticalAlignment = VerticalAlignment.Top;
            thisStack.Children.Add(ToggleBut);
            Content = thisGrid; //if not doing this, rethink.
            ThisMod.NewGameVisible = true;
            ThisMod.CommandContainer!.IsExecuting = false;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<MinesweeperViewModel>(); //go ahead and use the custom processes for this.  decided to mention non saved classes.
        }
    }
}