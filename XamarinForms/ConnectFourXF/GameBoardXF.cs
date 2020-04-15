using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGamingUIXFLibrary.BasicControls.GameBoards;
using BasicGamingUIXFLibrary.GameGraphics.GamePieces;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.ViewLinkersPlusBinders;
using ConnectFourCP.Data;
using ConnectFourCP.ViewModels;
using Xamarin.Forms;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace ConnectFourXF
{
    public class GameBoardXF : ImageGameBoardXF<SpaceInfoCP>
    {
        protected override float CalculateLeft(float oldWidth, float column)
        {
            var mults = oldWidth * column;
            var adds = column * 5;
            return mults + adds;
        }
        protected override void StartInit()
        {
            base.StartInit();
            CanClearAtEnd = false;
        }
        protected override View GetControl(SpaceInfoCP thisItem, int index)
        {
            CheckerPiecesXF output = new CheckerPiecesXF();
            output.BindingContext = thisItem;
            output.Margin = new Thickness(0, 0, 5, 0);
            if (ScreenUsed == EnumScreen.SmallPhone)
            {
                output.HeightRequest = 35;
                output.WidthRequest = 35;
            }
            else
            {
                output.HeightRequest = 95;
                output.WidthRequest = 95;
            }
            GamePackageViewModelBinder.ManuelElements.Add(output); //just in case.
            output.SetName(nameof(ConnectFourMainViewModel.ColumnAsync));
            output.SetBinding(CheckerPiecesXF.MainColorProperty, new Binding(nameof(SpaceInfoCP.Color)));
            output.SetBinding(CheckerPiecesXF.HasImageProperty, new Binding(nameof(SpaceInfoCP.HasImage)));
            output.BlankColor = cs.Aqua;
            output.CommandParameter = thisItem;
            output.Init(); // try this
            return output;
        }
    }
}
