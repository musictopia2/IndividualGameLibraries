using AndyCristinaGamePackageCP.DataClasses;
using AndyCristinaGamePackageXF;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
using BaseGPXPagesAndControlsXF.BasicControls.GameBoards;
using SkiaSharp;
using Xamarin.Forms;
using XPuzzleCP;
using static AndyCristinaGamePackageCP.DataClasses.GlobalStaticClass;
namespace XPuzzleXF
{
    public class XPuzzleGameBoard : BasicGameBoardXF<XPuzzleSpaceInfo>
    {
        protected override void StartInit()
        {
            Padding = new Thickness(5, 5, 5, 5);
        }
        protected override View GetControl(XPuzzleSpaceInfo thisItem, int index)
        {
            Button thisBut = new Button();
            thisBut.BindingContext = thisItem;
            thisBut.SetBinding(Button.TextProperty, new Binding(nameof(XPuzzleSpaceInfo.Text)));
            thisBut.SetBinding(BackgroundColorProperty, GetColorBinding(nameof(XPuzzleSpaceInfo.Color)));
            thisBut.SetBinding(Button.CommandProperty, GetCommandBinding(nameof(XPuzzleViewModel.SpaceCommand)));
            thisBut.BorderWidth = 2; //try this.
            thisBut.BorderColor = Color.White;
            if (ScreenUsed == EnumScreen.SmallPhone)
            {
                thisBut.FontSize = 60;
                thisBut.WidthRequest = 90;
                thisBut.HeightRequest = 90;
            }
            else if (ScreenUsed == EnumScreen.SmallTablet)
            {
                thisBut.FontSize = 130;
                thisBut.WidthRequest = 170;
                thisBut.HeightRequest = 170;
            }
            else
            {
                thisBut.FontSize = 200;
                thisBut.WidthRequest = 240;
                thisBut.HeightRequest = 240;
            }
            thisBut.TextColor = Color.White;
            thisBut.SetBinding(Button.CommandParameterProperty, new Binding(".")); // try this
            return  thisBut;
        }
    }
}