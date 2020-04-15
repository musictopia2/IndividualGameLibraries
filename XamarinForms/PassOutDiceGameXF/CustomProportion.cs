using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using CommonBasicStandardLibraries.Exceptions;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;

namespace PassOutDiceGameXF
{
    public class CustomProportion : IProportionBoard
    {
        float IProportionBoard.Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.LargeTablet)
                    return 2.4f;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 1.7f;
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 1.1f;
                throw new BasicBlankException("Screen not supported");
            }
        }
    }
}
