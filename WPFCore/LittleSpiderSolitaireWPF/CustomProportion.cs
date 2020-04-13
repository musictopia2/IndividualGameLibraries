using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
namespace LittleSpiderSolitaireWPF
{
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion => 2.5f; //2.3 was standard size.  you can either increase or decrease as needed.
    }
}