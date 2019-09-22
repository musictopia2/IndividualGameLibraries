using BasicGameFramework.GameGraphicsCP.Interfaces;
namespace PaydayWPF
{
    public class CustomProportionWPF : IProportionImage
    {
        float IProportionImage.Proportion => 1.5f; //had to be smaller this time.
    }
}