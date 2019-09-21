using AndyCristinaGamePackageCP.DataClasses;
using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using CandylandCP;
using static AndyCristinaGamePackageCP.DataClasses.GlobalStaticClass;
namespace CandylandXF
{
    public class PieceXF : BaseGraphicsXF<CandylandPieceGraphicsCP>
    {
        public void SetSizes()
        {
            if (ScreenUsed == EnumScreen.SmallPhone)
            {
                WidthRequest = 33;
                HeightRequest = 43;
            }
            else
            {
                WidthRequest = 66;
                HeightRequest = 86;
            }
            Init(); //i think this is it.
        }
    }
}