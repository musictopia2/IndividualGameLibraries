using BasicGameFramework.GameGraphicsCP.Interfaces;
namespace FluxxXF
{
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion
        {
            get
            {
                return 1.0f;
            }
        }
    }
}