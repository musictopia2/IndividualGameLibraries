using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
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