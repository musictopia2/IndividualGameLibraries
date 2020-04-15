using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
namespace LifeBoardGameXF
{
    public class BoardProportion : IProportionBoard
    {
        float IProportionBoard.Proportion => .9f;
    }
}