using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIWPFLibrary.BasicControls.GameBoards;
using CandylandCP.Data;
using CandylandCP.GraphicsCP;
using CandylandCP.Logic;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using System.Windows.Controls;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
//i think this is the most common things i like to do
namespace CandylandWPF
{
    public class GameboardWPF : UserControl, IHandle<NewTurnEventModel>, IHandle<CandylandPlayerItem>
    {
        internal SkiaSharpGameBoard Element1 { get; set; }
        private readonly SKElement _otherElement;
        CandylandBoardGraphicsCP? _privateBoard;
        CandylandMainGameClass? _thisGame;
        public string PieceForCurrentPlayer()
        {
            var thisPlayer = _thisGame!.SingleInfo;
            return _privateBoard!.ColorForPiece(thisPlayer!);
        }
        public GameboardWPF()
        {
            Element1 = new SkiaSharpGameBoard();
            Element1.MouseUp += ThisElement_MouseUp;
            Element1.PaintSurface += ThisElement_PaintSurface;
            _otherElement = new SKElement();
            _otherElement.PaintSurface += OtherPaint;
        }
        private void OtherPaint(object? sender, SKPaintSurfaceEventArgs e)
        {
            if (_hasLoaded == false)
                return;
            _privateBoard!.DrawGraphicsForBoard(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
        private bool _hasLoaded = false;
        public void LoadBoard()
        {
            _privateBoard = Resolve<CandylandBoardGraphicsCP>();
            SKSize thisSize = _privateBoard.SuggestedSize();
            Width = thisSize.Width;
            Height = thisSize.Height;
            EventAggregator thisT = Resolve<EventAggregator>();
            thisT.Subscribe(this); //i think this is it.  if i am wrong, rethink
            _thisGame = Resolve<CandylandMainGameClass>();
            _hasLoaded = true;
            Grid grid = new Grid();
            grid.Children.Add(_otherElement);
            grid.Children.Add(Element1);
            Content = grid;
            Element1.InvalidateVisual(); //i think
            _otherElement.InvalidateVisual();
        }
        private void ThisElement_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            if (_hasLoaded == false)
                return;
            Element1.StartInvalidate(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
        private void ThisElement_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_hasLoaded == false)
                return;
            var thisPos = e.GetPosition(Element1);
            Element1.StartClick(thisPos.X, thisPos.Y);
        }
        public void Handle(NewTurnEventModel message)
        {
            Element1.InvalidateVisual();
        }
        public void Handle(CandylandPlayerItem message)
        {
            Element1.InvalidateVisual();
        }
    }
}
