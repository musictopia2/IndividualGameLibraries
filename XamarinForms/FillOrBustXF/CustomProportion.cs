using AndyCristinaGamePackageCP.DataClasses;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using static AndyCristinaGamePackageCP.DataClasses.GlobalStaticClass;
namespace FillOrBustXF
{
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 1.0f;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 1.8f;
                return 2.6f;
            }
        }
    }
}