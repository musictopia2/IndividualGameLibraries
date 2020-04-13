using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIWPFLibrary.BasicControls.GameBoards;
using ChessCP.Logic;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Windows.Controls;
using System.Windows.Input;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace ChessWPF
{
    public class GameBoardWPF : UserControl, IHandle<RepaintEventModel>
    {
        internal SkiaSharpGameBoard Element { get; set; }
        //i think that scaling is not good for chess so this is the best i could do for now.
        public void LoadBoard()
        {
            GameBoardGraphicsCP PrivateBoard = Resolve<GameBoardGraphicsCP>();
            MouseUp += GameboardWPF_MouseUp;
            Element.PaintSurface += ThisElement_PaintSurface;
            SKSize thisSize = PrivateBoard.SuggestedSize();
            Width = thisSize.Width;
            Height = thisSize.Height;
            EventAggregator thisT = Resolve<EventAggregator>();
            thisT.Subscribe(this, EnumRepaintCategories.FromSkiasharpboard.ToString());
            Content = Element;
        }
        public GameBoardWPF()
        {
            Element = new SkiaSharpGameBoard();
        }
        private void ThisElement_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            Element.StartInvalidate(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }

        private void GameboardWPF_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var thisPos = e.GetPosition(Element);
            Element.StartClick(thisPos.X, thisPos.Y);
        }

        public void Handle(RepaintEventModel message)
        {
            Element.InvalidateVisual();
        }
    }
}
