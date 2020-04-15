using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGamingUIXFLibrary.GameGraphics.Base;
using CandylandCP.GraphicsCP;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
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