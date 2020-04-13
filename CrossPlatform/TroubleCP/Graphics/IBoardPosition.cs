using SkiaSharp;

namespace TroubleCP.Graphics
{
    public interface IBoardPosition
    {
        SKPoint RecommendedPointForDice(SKPoint pt_Center, float actualWidth, float actualHeight);
        int CalculateDiffTopRegular(int space, int recommendedAmount); //to make it more flexible.
    }
}