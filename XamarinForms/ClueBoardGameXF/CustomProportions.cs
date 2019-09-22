using BasicGameFramework.GameGraphicsCP.Interfaces;
namespace ClueBoardGameXF
{
    public class BoardProportion : IProportionBoard //since only large tablets are supported now.
    {
        float IProportionBoard.Proportion => 1.2f;
    }
    public class CardProportion : IProportionImage
    {
        float IProportionImage.Proportion => 1.1f;
    }
}