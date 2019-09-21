using LifeBoardGameCP;
using SkiaSharp.Views.Forms;
using System;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace LifeBoardGameXF
{
    public class SpinnerCanvasXF : SKCanvasView, ISpinnerCanvas
    {
        public int Position { get; set; }
        public int HighSpeedPhase { get; set; }
        public void Repaint()
        {
            InvalidateSurface();
        }
    }
    public class SpinnerXF : ContentView
    {
        internal SpinnerCanvasXF? ThisElement;
        SKCanvasView? _mainElement;
        private SpinnerCP? _thisBoard; //you have to register.
        LifeBoardGameViewModel? _thisMod;
        public void LoadBoard()
        {
            ThisElement = new SpinnerCanvasXF();
            _thisMod = Resolve<LifeBoardGameViewModel>();
            _thisBoard = Resolve<SpinnerCP>();
            GlobalVariableClass thisG = Resolve<GlobalVariableClass>();
            thisG.SpinnerCanvas = ThisElement;
            ThisElement.PaintSurface += ThisElement_PaintSurface;
            var thisSize = _thisBoard.SuggestedBoardSize; //error is has to rethink for xamarin forms.
            WidthRequest = thisSize;
            HeightRequest = thisSize;
            _mainElement = new SKCanvasView();
            _mainElement.Touch += MainTouch;
            _mainElement.EnableTouchEvents = true;
            _mainElement.PaintSurface += MainElementPaint;
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(_mainElement);
            thisGrid.Children.Add(ThisElement);
            Content = thisGrid;
        }
        private void MainTouch(object sender, SKTouchEventArgs e)
        {
            if (_thisMod!.SpinCommand!.CanExecute(null!))
                _thisMod.SpinCommand.Execute(null!);
        }
        private void MainElementPaint(object? sender, SKPaintSurfaceEventArgs e)
        {
            _thisBoard!.DrawSpinnerBoard(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
        private void ThisElement_PaintSurface(object? sender, SKPaintSurfaceEventArgs e) //iffy.
        {
            if (ThisElement!.HighSpeedPhase > 0)
                _thisBoard!.DrawHighSpeedArrow(e.Surface.Canvas, e.Info.Width, e.Info.Height);
            else
                _thisBoard!.DrawNormalArrow(e.Surface.Canvas, ThisElement.Position, e.Info.Height); //i think.
        }
    }
}