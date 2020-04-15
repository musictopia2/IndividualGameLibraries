using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;

namespace MahJongSolitaireXF
{
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion
        {
            get
            {
                return .95f; //only large is supported now.

            }
        }
    }
}
