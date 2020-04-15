using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
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