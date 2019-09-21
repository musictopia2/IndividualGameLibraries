using BaseGPXPagesAndControlsXF.BasicControls.GameBoards;
using BasicGameFramework.BasicEventModels;
using CandylandCP;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace CandylandXF
{
    public class GameBoardXF : ContentView, IHandle<NewTurnEventModel>, IHandle<CandylandPlayerItem>
    {
        internal SkiaSharpGameBoardXF ThisElement;
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
            ThisElement = new SkiaSharpGameBoardXF();
            ThisElement.EnableTouchEvents = true;
            ThisElement.Touch += ThisElement_Touch;
            ThisElement.PaintSurface += ThisElement_PaintSurface;
            _otherElement = new SKCanvasView();
            _otherElement.PaintSurface += OtherPaint;
        }
        private void ThisElement_Touch(object sender, SKTouchEventArgs e)
        {
            if (HasLoaded == false)
                return;
            ThisElement.StartClick(e.Location.X, e.Location.Y);
        }
        private void OtherPaint(object? sender, SKPaintSurfaceEventArgs e)
        {
            if (HasLoaded == false)
                return;
            _privateBoard!.DrawGraphicsForBoard(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
        private bool HasLoaded = false;
        public void LoadBoard()
        {
            _privateBoard = Resolve<CandylandBoardGraphicsCP>();
            SKSize thisSize = _privateBoard.SuggestedSize();
            WidthRequest = thisSize.Width;
            HeightRequest = thisSize.Height;
            EventAggregator thisT = Resolve<EventAggregator>();
            thisT.Subscribe(this); //i think this is it.  if i am wrong, rethink
            _thisGame = Resolve<CandylandMainGameClass>();
            HasLoaded = true;
            Grid grid = new Grid();
            grid.Children.Add(_otherElement);
            grid.Children.Add(ThisElement);
            Content = grid;
            _otherElement.InvalidateSurface();
            ThisElement.InvalidateSurface(); //i think
        }
        private void ThisElement_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            if (HasLoaded == false)
                return;
            ThisElement.StartInvalidate(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
        public void Handle(NewTurnEventModel message)
        {
            ThisElement.InvalidateSurface();
        }
        public void Handle(CandylandPlayerItem message)
        {
            ThisElement.InvalidateSurface();
        }
    }
}