using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
namespace TeeItUpXF
{
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return .6f; //experiment
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 1.1f;
                return 2.0f; //experiment.
            }
        }
    }
}