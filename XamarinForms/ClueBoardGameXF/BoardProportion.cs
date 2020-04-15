using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
namespace ClueBoardGameXF
{
    public class BoardProportion : IProportionBoard //since only large tablets are supported now.
    {
        float IProportionBoard.Proportion => 1.2f;
    }
}