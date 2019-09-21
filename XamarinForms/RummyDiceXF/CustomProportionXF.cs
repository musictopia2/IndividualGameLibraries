using AndyCristinaGamePackageCP.DataClasses;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using static AndyCristinaGamePackageCP.DataClasses.GlobalStaticClass;
namespace RummyDiceXF
{
    public class CustomProportionXF : IProportionImage
    {
        float IProportionImage.Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return .75f;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 1.65f;
                return 3.0f;
            }
        }
    }
}