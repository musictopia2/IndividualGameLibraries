using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseMahjongTilesCP;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using MahJongSolitaireCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
namespace MahJongSolitaireXF
{
    public class GamePage : SinglePlayerGamePage<MahJongSolitaireViewModel>
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
        protected override async Task AfterGameButtonAsync() //looks like the layout showed different.  can try this way but if does not work, try the way it used to be.
        {
            Grid thisGrid = new Grid();
            AddLeftOverColumn(thisGrid, 75);
            AddLeftOverColumn(thisGrid, 25);
            StackLayout thisStack = new StackLayout();
            thisStack.Margin = new Thickness(5, 5, 5, 5);
            GameBoardXF thisControl = new GameBoardXF();
            thisControl.Margin = new Thickness(5, 5, 5, 5);
            AddControlToGrid(thisGrid, thisControl, 0, 0);
            AddControlToGrid(thisGrid, thisStack, 0, 1);
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            var thisMove = GetGamingButton("Undo Move", nameof(MahJongSolitaireViewModel.UndoMoveCommand));
            thisStack.Children.Add(thisMove);
            var thisHint = GetGamingButton("Hint", nameof(MahJongSolitaireViewModel.HintCommand));
            thisStack.Children.Add(thisHint);
            Grid.SetColumnSpan(thisControl, 2);
            SimpleLabelGridXF thisHelps = new SimpleLabelGridXF();
            thisHelps.AddRow("Tiles Gone", nameof(MahJongSolitaireViewModel.TilesGone));
            var ttherGrid = thisHelps.GetContent;
            thisStack.Children.Add(ttherGrid);
            Content = thisGrid;
            await ThisMod!.StartNewGameAsync(); //i think
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<MahJongSolitaireViewModel>(); //go ahead and use the custom processes for this.
            OurContainer!.RegisterType<BaseMahjongGlobals>(true);
            OurContainer.RegisterSingleton<IProportionImage, CustomProportion>("");
            OurContainer.RegisterType<MahjongShuffler>(true);
        }
    }
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.LargeTablet)
                    return 0.95f;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 0.75f;
                return 0.95f;
            }
        }
    }
}