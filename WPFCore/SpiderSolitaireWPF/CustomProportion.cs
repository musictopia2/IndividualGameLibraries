using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
namespace SpiderSolitaireWPF
{
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion => 1.9f; //2.3 was standard size.  you can either increase or decrease as needed.
    }
}