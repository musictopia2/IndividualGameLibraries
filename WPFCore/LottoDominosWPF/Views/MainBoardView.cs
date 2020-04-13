using BasicGameFrameworkLibrary.Dominos;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.GameGraphics.Dominos;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using LottoDominosCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Dominos.DominosCP;

namespace LottoDominosWPF.Views
{
    public class MainBoardView : UserControl, IUIView
    {
        public MainBoardView(GameBoardCP boardcp)
        {
            CardBoardWPF<SimpleDominoInfo, ts, DominosWPF<SimpleDominoInfo>> boardui = new CardBoardWPF<SimpleDominoInfo, ts, DominosWPF<SimpleDominoInfo>>();
            Content = boardui;
            boardui.LoadList(boardcp, ts.TagUsed);
            //hopefully this simple this time.
        }
        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
