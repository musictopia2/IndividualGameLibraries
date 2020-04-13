using BasicGamingUIWPFLibrary.BasicControls.GameBoards;
using System.Windows.Controls;
using TicTacToeCP.Data;
namespace TicTacToeWPF
{
    public class GameBoardWPF : BasicGameBoard<SpaceInfoCP>
    {
        protected override Control GetControl(SpaceInfoCP thisItem, int index)
        {
            return new SpaceWPF(thisItem);
        }
    }
}
