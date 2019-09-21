using AndyCristinaGamePackageCP.DataClasses;
using BaseGPXPagesAndControlsXF.BasicControls.GameBoards;
using BaseGPXPagesAndControlsXF.GameGraphics.GamePieces;
using ConnectFourCP;
using Xamarin.Forms;
using static AndyCristinaGamePackageCP.DataClasses.GlobalStaticClass;
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
            //if (column == 0 || column == 1)
            //    return base.CalculateLeft(oldWidth, column);
            //var starts = oldWidth * column;
            //if (column == 2)
            //    return starts + 10;
            //if (column == 3)
            //    return starts + 20;
            //if (column == 4)
            //    return starts + 20;
            //if (column == 5)
            //    return starts + 25;
            //if (column == 6)
            //    return starts + 30;
            //throw new Exception("Not found");
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
            output.SetBinding(CheckerPiecesXF.CommandProperty, GetCommandBinding(nameof(ConnectFourViewModel.ColumnCommand)));
            output.SetBinding(CheckerPiecesXF.MainColorProperty, new Binding(nameof(SpaceInfoCP.Color)));
            output.SetBinding(CheckerPiecesXF.HasImageProperty, new Binding(nameof(SpaceInfoCP.HasImage)));
            output.BlankColor = cs.Aqua;
            output.CommandParameter = thisItem;
            output.Init(); // try this
            return output;
        }
    }
}
