using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGamingUIXFLibrary.BasicControls.GameBoards;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.ViewLinkersPlusBinders;
using SequenceDiceCP.Data;
using SequenceDiceCP.ViewModels;
using Xamarin.Forms;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
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
            thisCon.SetName(nameof(SequenceDiceMainViewModel.MakeMoveAsync));
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
            GamePackageViewModelBinder.ManuelElements.Add(thisCon);
            thisCon.Init(); //maybe this too.
            thisCon.CommandParameter = thisItem;
            return thisCon; //hopefully this simple.
        }
    }
}
