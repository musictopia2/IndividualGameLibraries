using AndyCristinaGamePackageCP.DataClasses;
using BaseGPXPagesAndControlsXF.BasicControls.GameBoards;
using SequenceDiceCP;
using Xamarin.Forms;
using static AndyCristinaGamePackageCP.DataClasses.GlobalStaticClass;
namespace SequenceDiceXF
{
    public class GameBoardXF : BasicGameBoardXF<SpaceInfoCP>
    {
        protected override View GetControl(SpaceInfoCP thisItem, int index)
        {
            var thisCon = new MiscGraphicsXF();
            thisCon.BindingContext = thisItem;
            thisCon.SetBinding(MiscGraphicsXF.WasPreviousProperty, new Binding(nameof(SpaceInfoCP.WasRecent)));
            thisCon.SetBinding(MiscGraphicsXF.NumberProperty, new Binding(nameof(SpaceInfoCP.Number)));
            thisCon.SetBinding(MiscGraphicsXF.MainColorProperty, new Binding(nameof(SpaceInfoCP.Color)));
            thisCon.SetBinding(MiscGraphicsXF.CommandProperty, GetCommandBinding(nameof(SequenceDiceViewModel.SpaceCommand)));
            if (ScreenUsed == EnumScreen.SmallPhone)
            {
                thisCon.WidthRequest = 52;
                thisCon.HeightRequest = 52;
            }
            else if (ScreenUsed == EnumScreen.SmallTablet)
            {
                thisCon.WidthRequest = 95;
                thisCon.HeightRequest = 95;
            }
            else
            {
                thisCon.WidthRequest = 130;
                thisCon.HeightRequest = 130;
            }
            thisCon.Init(); //maybe this too.
            thisCon.CommandParameter = thisItem;
            return thisCon; //hopefully this simple.
        }
    }
}
