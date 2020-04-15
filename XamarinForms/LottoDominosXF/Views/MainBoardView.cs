using BasicGameFrameworkLibrary.Dominos;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.GameGraphics.Dominos;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using LottoDominosCP.Logic;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Dominos.DominosCP;

namespace LottoDominosXF.Views
{
    public class MainBoardView : CustomControlBase
    {
        public MainBoardView(GameBoardCP boardcp)
        {
            CardBoardXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>> boardui = new CardBoardXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>>();
            Content = boardui;
            boardui.LoadList(boardcp, ts.TagUsed);
        }
    }
}
