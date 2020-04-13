using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
namespace GrandfathersClockWPF
{
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion => 1.6f; //2.3 was standard size.  you can either increase or decrease as needed.
    }
}