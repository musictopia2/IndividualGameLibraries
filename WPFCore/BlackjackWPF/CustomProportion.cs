using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
namespace BlackjackWPF
{
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion => 2.3f; //2.3 was standard size.  you can either increase or decrease as needed.
    }
}