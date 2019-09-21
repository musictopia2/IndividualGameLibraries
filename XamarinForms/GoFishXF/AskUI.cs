using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.GameGraphics.GamePieces;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.RegularDeckOfCards;
using GoFishCP;
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace GoFishXF
{
    public class AskUI : BaseFrameXF
    {
        public void Init()
        {
            GoFishViewModel thisMod = Resolve<GoFishViewModel>();
            Grid parentGrid = new Grid();
            StackLayout thisStack = new StackLayout();
            EnumPickerXF<NumberPieceCP, NumberPieceXF, EnumCardValueList, CardValueListChooser>
    thisAsk = new EnumPickerXF<NumberPieceCP, NumberPieceXF, EnumCardValueList, CardValueListChooser>();
            thisAsk.Rows = 3;
            thisAsk.Columns = 5;
            thisAsk.LoadLists(thisMod.AskList!);
            Text = "Choose Number To Ask";
            thisStack.Children.Add(thisAsk);
            var thisBut = GetSmallerButton("Number To Ask", nameof(GoFishViewModel.AskCommand));
            thisStack.Children.Add(thisBut);
            SetUpMarginsOnParentControl(thisStack);
            parentGrid.Children.Add(ThisDraw); // maybe this is causing problems for the other (?)
            parentGrid.Children.Add(thisStack);
            Content = parentGrid;
        }
    }
}