using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;

namespace PokerWPF
{
    public class CustomProportion : IProportionImage, IWidthHeight
    {
        float IProportionImage.Proportion => 3.0f; //2.3 was standard size.  you can either increase or decrease as needed.
        int IWidthHeight.GetWidthHeight => 150;
    }
}