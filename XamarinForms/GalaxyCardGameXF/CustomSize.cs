using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using CommonBasicStandardLibraries.Exceptions;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;

namespace GalaxyCardGameXF
{
    public class CustomSize : IProportionImage
    {
        float IProportionImage.Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    throw new BasicBlankException("Phone not supported this time");
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 1.0f;
                return 1.2f; //can tweak as needed.
            }
        }
    }
}
