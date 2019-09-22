using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasicControls.GameBoards;
using SkiaSharp;
using Xamarin.Forms;
using XPuzzleCP;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
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