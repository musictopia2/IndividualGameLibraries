using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseMahjongTilesCP;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using MahJongSolitaireCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
namespace MahJongSolitaireWPF
{
    public class GamePage : SinglePlayerWindow<MahJongSolitaireViewModel>
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
        protected async override void AfterGameButton()
        {
            Grid thisGrid = new Grid();
            AddLeftOverColumn(thisGrid, 1);
            AddAutoColumns(thisGrid, 1);
            StackPanel thisStack = new StackPanel();
            thisStack.Margin = new Thickness(5, 5, 5, 5);
            GameBoardWPF thisControl = new GameBoardWPF();
            thisControl.Margin = new Thickness(5, 5, 5, 5);
            AddControlToGrid(thisGrid, thisControl, 0, 0);
            AddControlToGrid(thisGrid, thisStack, 0, 1);
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            var thisMove = GetGamingButton("Undo Move", nameof(MahJongSolitaireViewModel.UndoMoveCommand));
            thisStack.Children.Add(thisMove);
            var thisHint = GetGamingButton("Hint", nameof(MahJongSolitaireViewModel.HintCommand));
            thisStack.Children.Add(thisHint);
            Grid.SetColumnSpan(thisControl, 2);
            SimpleLabelGrid thisHelps = new SimpleLabelGrid();
            thisHelps.AddRow("Tiles Gone", nameof(MahJongSolitaireViewModel.TilesGone));
            var ttherGrid = thisHelps.GetContent;
            thisStack.Children.Add(ttherGrid);
            Content = thisGrid;
            await ThisMod!.StartNewGameAsync(); //i think
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<MahJongSolitaireViewModel>(); //go ahead and use the custom processes for this.  decided to mention non saved classes.
            OurContainer!.RegisterType<BaseMahjongGlobals>(true);
            OurContainer.RegisterSingleton<IProportionImage, CustomProportion>("");
            OurContainer.RegisterType<MahjongShuffler>(true); //you still have to register because of the ideckcount part.
        }
    }
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion => 1.1f;
    }
}