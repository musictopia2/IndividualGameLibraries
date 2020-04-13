using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
namespace ClockSolitaireWPF
{
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion => 1.5f; //2.3 was standard size.  you can either increase or decrease as needed.
    }
}