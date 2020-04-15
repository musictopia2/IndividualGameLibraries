using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.GameGraphics.GamePieces;
using GoFishCP.Data;
using GoFishCP.ViewModels;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace GoFishXF.Views
{
    public class AskView : FrameUIViewXF
    {
        public AskView(GoFishVMData model)
        {
            Grid parentGrid = new Grid();
            StackLayout thisStack = new StackLayout();
            EnumPickerXF<NumberPieceCP, NumberPieceXF, EnumCardValueList>
                thisAsk = new EnumPickerXF<NumberPieceCP, NumberPieceXF, EnumCardValueList>();
            thisAsk.Rows = 3;
            thisAsk.Columns = 5;
            thisAsk.LoadLists(model.AskList!);
            Text = "Choose Number To Ask";
            thisStack.Children.Add(thisAsk);
            var thisBut = GetGamingButton("Number To Ask", nameof(AskViewModel.AskAsync));
            thisStack.Children.Add(thisBut);
            SetUpMarginsOnParentControl(thisStack);
            parentGrid.Children.Add(ThisDraw); // maybe this is causing problems for the other (?)
            parentGrid.Children.Add(thisStack);
            Content = parentGrid;
        }
    }
}
