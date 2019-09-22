using BaseGPXWindowsAndControlsCore.BasicControls.GameBoards;
using BasicGameFramework.BasicEventModels;
using CheckersCP;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using System.Windows.Controls;
using System.Windows.Input;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace CheckersWPF
{
    public class GameBoardWPF : UserControl, IHandle<RepaintEventModel>
    {
        internal SkiaSharpGameBoard ThisElement;
        public void LoadBoard()
        {
            GameBoardGraphicsCP PrivateBoard = Resolve<GameBoardGraphicsCP>();
            MouseUp += GameboardWPF_MouseUp;
            ThisElement.PaintSurface += ThisElement_PaintSurface;
            SKSize thisSize = PrivateBoard.SuggestedSize();
            Width = thisSize.Width;
            Height = thisSize.Height;
            EventAggregator thisT = Resolve<EventAggregator>();
            thisT.Subscribe(this, EnumRepaintCategories.fromskiasharpboard.ToString());
            Content = ThisElement;
        }
        public GameBoardWPF()
        {
            ThisElement = new SkiaSharpGameBoard();
        }
        private void ThisElement_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            ThisElement.StartInvalidate(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
        private void GameboardWPF_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var ThisPos = e.GetPosition(ThisElement);
            ThisElement.StartClick(ThisPos.X, ThisPos.Y);
        }
        public void Handle(RepaintEventModel message)
        {
            ThisElement.InvalidateVisual();
        }
    }
}