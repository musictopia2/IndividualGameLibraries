using BasicGameFramework.GameGraphicsCP.Interfaces;
namespace MillebournesWPF
{
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion => 3.0f; //try to use standard on tablets.
    }
}