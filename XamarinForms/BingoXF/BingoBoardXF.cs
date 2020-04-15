using BasicGamingUIXFLibrary.BasicControls.GameBoards;
using BingoCP.Data;
using Xamarin.Forms;
namespace BingoXF
{
    public class BingoBoardXF : BasicGameBoardXF<SpaceInfoCP>
    {
        protected override View GetControl(SpaceInfoCP thisItem, int index)
        {
            var thisBingo = new BingoSpaceXF();
            thisBingo.ThisSpace = thisItem;
            thisBingo.Init();
            return thisBingo;
        }
    }
}