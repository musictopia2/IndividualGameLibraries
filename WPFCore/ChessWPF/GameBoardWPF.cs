using BaseGPXWindowsAndControlsCore.BasicControls.GameBoards;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using ChessCP;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Windows.Controls;
using System.Windows.Input;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace ChessWPF
{
    public class CustomProportion : IProportionBoard
    {
        float IProportionBoard.Proportion => 1;
    }
    public class GameBoardWPF : UserControl, IHandle<RepaintEventModel>
    {
        internal SkiaSharpGameBoard ThisElement;
        //i think that scaling is not good for chess so this is the best i could do for now.
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
            var thisPos = e.GetPosition(ThisElement);
            ThisElement.StartClick(thisPos.X, thisPos.Y);
        }

        public void Handle(RepaintEventModel message)
        {
            ThisElement.InvalidateVisual();
        }
    }
}
