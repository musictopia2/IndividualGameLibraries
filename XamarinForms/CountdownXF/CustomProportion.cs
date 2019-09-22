using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
namespace CountdownXF
{
    public class CustomProportion : IProportionBoard
    {
        float IProportionBoard.Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.Desktop)
                    return 1.9f;
                if (ScreenUsed == EnumScreen.LargeTablet)
                    return 1.8f;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 1.4f;
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 1.0f;
                throw new BasicBlankException("Screen not supported");
            }
        }
    }
}
