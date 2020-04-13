using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
namespace AccordianSolitaireWPF
{
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion => 1.7f; //2.3 was standard size.  you can either increase or decrease as needed.
    }
}