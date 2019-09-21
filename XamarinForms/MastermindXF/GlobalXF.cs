using AndyCristinaGamePackageCP.DataClasses;
using static AndyCristinaGamePackageCP.DataClasses.GlobalStaticClass;
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
