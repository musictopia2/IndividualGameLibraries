using BasicGameFramework.GameGraphicsCP.Interfaces;
namespace LifeBoardGameWPF
{
    public class CardProportion : IProportionImage
    {
        float IProportionImage.Proportion => 1.5f;
    }
    public class BoardProportion : IProportionBoard
    {
        float IProportionBoard.Proportion => 1;
    }
}