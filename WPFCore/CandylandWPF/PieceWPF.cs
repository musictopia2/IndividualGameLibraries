using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using CandylandCP;
namespace CandylandWPF
{
    public class PieceWPF : BaseGraphicsWPF<CandylandPieceGraphicsCP>
    {
        public void SetSizes()
        {
            Width = 55;
            Height = 73;
            Init(); //i think this is it.
        }
    }
}