using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;

namespace XactikaXF
{
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return .65f;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return .8f;
                return 1.1f;
            }
        }
    }
}