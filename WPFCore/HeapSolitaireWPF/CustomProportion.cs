using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
namespace HeapSolitaireWPF
{
    public class CustomProportion : IProportionImage
    {
        //since new game will probably be on top, then needs to be a little smaller for cards to make up for it.
        float IProportionImage.Proportion => 1.7f; //2.3 was standard size.  you can either increase or decrease as needed.
    }
}