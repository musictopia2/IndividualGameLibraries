using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
namespace BakersDozenSolitaireWPF
{
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion => 2.2f; //2.3 was standard size.  you can either increase or decrease as needed.
    }
}