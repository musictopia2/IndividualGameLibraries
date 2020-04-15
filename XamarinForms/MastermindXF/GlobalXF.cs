using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
namespace MastermindXF
{
    public static class GlobalXF
    {
        public static int GuessWidthHeight
        {
            get
            {
                if (ScreenUsed == EnumScreen.LargeTablet)
                    return 60;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 50;
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 25;
                return 60;
            }
        }
    }
}
