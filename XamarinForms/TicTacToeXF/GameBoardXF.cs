using BaseGPXPagesAndControlsXF.BasicControls.GameBoards;
using TicTacToeCP;
using Xamarin.Forms;
namespace TicTacToeXF
{
    public class GameBoardXF : BasicGameBoardXF<SpaceInfoCP>
    {
        protected override View GetControl(SpaceInfoCP thisItem, int index)
        {
            return new SpaceXF(thisItem);
        }
    }
}