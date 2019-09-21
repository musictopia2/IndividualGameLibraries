using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.GameGraphics.GamePieces;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.RegularDeckOfCards;
using GoFishCP;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace GoFishWPF
{
    public class AskUI : BaseFrameWPF
    {
        public void Init()
        {
            GoFishViewModel thisMod = Resolve<GoFishViewModel>();
            Grid parentGrid = new Grid();
            StackPanel thisStack = new StackPanel();
            EnumPickerWPF<NumberPieceCP, NumberPieceWPF, EnumCardValueList, CardValueListChooser>
                thisAsk = new EnumPickerWPF<NumberPieceCP, NumberPieceWPF, EnumCardValueList, CardValueListChooser>();
            thisAsk.Rows = 3;
            thisAsk.Columns = 5;
            thisAsk.LoadLists(thisMod.AskList!);
            Text = "Choose Number To Ask";
            thisStack.Children.Add(thisAsk);
            var thisBut = GetGamingButton("Number To Ask", nameof(GoFishViewModel.AskCommand));
            thisStack.Children.Add(thisBut);
            var thisRect = ThisFrame.GetControlArea();
            SetUpMarginsOnParentControl(thisStack, thisRect);
            parentGrid.Children.Add(ThisDraw); // maybe this is causing problems for the other (?)
            parentGrid.Children.Add(thisStack);
            Content = parentGrid;
        }
    }
}