using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
namespace MarthaSolitaireXF
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
                    return 1.3f;
                return 1.7f;
            }
        }
    }
}