using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
namespace ChessWPF
{
    public class CustomProportion : IProportionBoard
    {
        float IProportionBoard.Proportion => 1;
    }
}