using AndyCristinaGamePackageCP.DataClasses;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using static AndyCristinaGamePackageCP.DataClasses.GlobalStaticClass;
namespace MilkRunXF
{
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 1.2f;
                return 1.9f;
            }
        }
    }
}