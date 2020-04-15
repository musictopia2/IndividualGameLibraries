using BasicGamingUIXFLibrary.BasicControls.GameBoards;
using BasicGameFrameworkLibrary.BasicEventModels;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CandylandCP.Data;
using CandylandCP.GraphicsCP;
using CandylandCP.Logic;

namespace CandylandXF
{
    public class GameBoardXF : ContentView, IHandle<NewTurnEventModel>, IHandle<CandylandPlayerItem>
    {
        internal SkiaSharpGameBoardXF Element { get; set; }
        private readonly SKCanvasView _otherElement;
        CandylandBoardGraphicsCP? _privateBoard;
        CandylandMainGameClass? _thisGame;
        public string PieceForCurrentPlayer()
        {
            var thisPlayer = _thisGame!.SingleInfo;
            return _privateBoard!.ColorForPiece(thisPlayer!);
        }
        public GameBoardXF()
        {
            Element = new SkiaSharpGameBoardXF();
            Element.EnableTouchEvents = true;
            Element.Touch += ThisElement_Touch;
            Element.PaintSurface += ThisElement_PaintSurface;
            _otherElement = new SKCanvasView();
            _otherElement.PaintSurface += OtherPaint;
        }
        private void ThisElement_Touch(object sender, SKTouchEventArgs e)
        {
            if (_hasLoaded == false)
                return;
            Element.StartClick(e.Location.X, e.Location.Y);
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
            WidthRequest = thisSize.Width;
            HeightRequest = thisSize.Height;
            EventAggregator thisT = Resolve<EventAggregator>();
            thisT.Subscribe(this); //i think this is it.  if i am wrong, rethink
            _thisGame = Resolve<CandylandMainGameClass>();
            _hasLoaded = true;
            Grid grid = new Grid();
            grid.Children.Add(_otherElement);
            grid.Children.Add(Element);
            Content = grid;
            _otherElement.InvalidateSurface();
            Element.InvalidateSurface(); //i think
        }
        private void ThisElement_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            if (_hasLoaded == false)
                return;
            Element.StartInvalidate(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
        public void Handle(NewTurnEventModel message)
        {
            Element.InvalidateSurface();
        }
        public void Handle(CandylandPlayerItem message)
        {
            Element.InvalidateSurface();
        }
    }
}