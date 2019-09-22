using BaseGPXWindowsAndControlsCore.BasicControls.GameBoards;
using BasicGameFramework.BasicEventModels;
using CandylandCP;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using System.Windows.Controls;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace CandylandWPF
{
    public class GameboardWPF : UserControl, IHandle<NewTurnEventModel>, IHandle<CandylandPlayerItem>
    {
        internal SkiaSharpGameBoard ThisElement;
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
            ThisElement = new SkiaSharpGameBoard();
            ThisElement.MouseUp += ThisElement_MouseUp;
            ThisElement.PaintSurface += ThisElement_PaintSurface;
            _otherElement = new SKElement();
            _otherElement.PaintSurface += OtherPaint;
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
            Width = thisSize.Width;
            Height = thisSize.Height;
            EventAggregator thisT = Resolve<EventAggregator>();
            thisT.Subscribe(this); //i think this is it.  if i am wrong, rethink
            _thisGame = Resolve<CandylandMainGameClass>();
            HasLoaded = true;
            Grid grid = new Grid();
            grid.Children.Add(_otherElement);
            grid.Children.Add(ThisElement);
            Content = grid;
            ThisElement.InvalidateVisual(); //i think
            _otherElement.InvalidateVisual();
        }
        private void ThisElement_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            if (HasLoaded == false)
                return;
            ThisElement.StartInvalidate(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
        private void ThisElement_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (HasLoaded == false)
                return;
            var thisPos = e.GetPosition(ThisElement);
            ThisElement.StartClick(thisPos.X, thisPos.Y);
        }
        public void Handle(NewTurnEventModel message)
        {
            ThisElement.InvalidateVisual();
        }
        public void Handle(CandylandPlayerItem message)
        {
            ThisElement.InvalidateVisual();
        }
    }
}