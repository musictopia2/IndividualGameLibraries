using BaseGPXWindowsAndControlsCore.BasicControls.GameBoards;
using BingoCP;
using System.Windows.Controls;
namespace BingoWPF
{
    public class BingoBoardWPF : BasicGameBoard<SpaceInfoCP>
    {
        protected override Control GetControl(SpaceInfoCP thisItem, int index)
        {
            var thisBingo = new BingoSpaceWPF();
            thisBingo.ThisSpace = thisItem;
            thisBingo.Init();
            return thisBingo;
        }
    }
}