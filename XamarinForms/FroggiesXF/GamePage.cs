using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.CommonInterfaces;
using FroggiesCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using BasicGameFramework.BasicGameDataClasses;
namespace FroggiesXF
{
    public class GamePage : SinglePlayerGamePage<FroggiesViewModel>
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
        protected override Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout();
            SetDefaultStartOrientations(GameButton!);
            thisStack.Children.Add(GameButton);
            Grid thisGrid = new Grid();
            AddAutoColumns(thisGrid, 1);
            AddLeftOverColumn(thisGrid, 1);
            AddAutoColumns(thisGrid, 1);
            AddControlToGrid(thisGrid, thisStack, 0, 1);
            GameBoardXF thisBoard = new GameBoardXF(ThisMod!);
            thisBoard.Margin = new Thickness(5, 5, 5, 5);
            thisStack.Margin = new Thickness(5, 5, 5, 5);
            AddControlToGrid(thisGrid, thisBoard, 0, 0);
            Button redoButton = GetGamingButton("Redo Game", nameof(FroggiesViewModel.RedoGameCommand));
            SetDefaultStartOrientations(redoButton);
            thisStack.Children.Add(redoButton);
            SimpleLabelGridXF thisLab = new SimpleLabelGridXF();
            thisLab.AddRow("Moves", nameof(FroggiesViewModel.MovesLeft));
            thisLab.AddRow("Frogs", nameof(FroggiesViewModel.NumberOfFrogs));
            thisStack.Children.Add(thisLab.GetContent);
            NumberChooserXF picker = new NumberChooserXF();
            picker.Columns = 4; //may need experimenting here too.
            picker.LoadLists(ThisMod!.LevelPicker!);
            AddControlToGrid(thisGrid, picker, 0, 2);
            Content = thisGrid; //if not doing this, rethink.
            ThisMod.NewGameVisible = true;
            ThisMod.CommandContainer!.IsExecuting = false;
            return Task.CompletedTask;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<FroggiesViewModel>(); //go ahead and use the custom processes for this.
            OurContainer!.RegisterType<CustomSize>(); //i think this should be fine.
        }
    }
    public class CustomSize : IWidthHeight
    {
        int IWidthHeight.GetWidthHeight
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 30;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 60;
                return 80;
            }
        }
    }
}