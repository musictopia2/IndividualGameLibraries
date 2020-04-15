using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
namespace PokerXF
{
    public class CustomProportion : IProportionImage, IWidthHeight
    {
        float IProportionImage.Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return .7f;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 1.3f;
                return 1.7f;
            }
        }

        int IWidthHeight.GetWidthHeight
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 50;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 110;
                return 150;
            }
        }
    }
}