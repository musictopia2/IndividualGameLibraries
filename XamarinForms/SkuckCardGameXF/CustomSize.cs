using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;

namespace SkuckCardGameXF
{
    public class CustomSize : IWidthHeight
    { //for now, only normal and small no large.  could add large if necessary (?)  of course, you are open 
        int IWidthHeight.GetWidthHeight
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 40;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 50;
                return 80;
            }
        }
    }
}
