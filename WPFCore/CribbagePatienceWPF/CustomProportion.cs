using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
namespace CribbagePatienceWPF
{
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion => 2.0f; //2.3 was standard size.  you can either increase or decrease as needed.
    }
}