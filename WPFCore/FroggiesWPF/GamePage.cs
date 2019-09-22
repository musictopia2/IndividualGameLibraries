using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using FroggiesCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
namespace FroggiesWPF
{
    public class GamePage : SinglePlayerWindow<FroggiesViewModel>
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
            AddAutoColumns(thisGrid, 2);
            AddLeftOverColumn(thisGrid, 1);
            AddControlToGrid(thisGrid, thisStack, 0, 1);
            GameBoardWPF thisBoard = new GameBoardWPF(ThisMod!);
            thisBoard.Margin = new Thickness(5, 5, 5, 5);
            thisStack.Margin = new Thickness(5, 5, 5, 5);
            AddControlToGrid(thisGrid, thisBoard, 0, 0);
            Button redoButton = GetGamingButton("Redo Game", nameof(FroggiesViewModel.RedoGameCommand));
            thisStack.Children.Add(redoButton);
            SimpleLabelGrid thisLab = new SimpleLabelGrid();
            thisLab.AddRow("Moves Left", nameof(FroggiesViewModel.MovesLeft));
            thisLab.AddRow("How Many Frogs", nameof(FroggiesViewModel.NumberOfFrogs));
            thisStack.Children.Add(thisLab.GetContent);
            NumberChooserWPF picker = new NumberChooserWPF();
            picker.Columns = 8;
            picker.LoadLists(ThisMod!.LevelPicker!);
            AddControlToGrid(thisGrid, picker, 0, 2);
            Content = thisGrid; //if not doing this, rethink.
            ThisMod.NewGameVisible = true;
            ThisMod.CommandContainer!.IsExecuting = false;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<FroggiesViewModel>(); //go ahead and use the custom processes for this.  decided to mention non saved classes.
            OurContainer!.RegisterType<StandardWidthHeight>(); //i think this should be fine.
        }
    }
}