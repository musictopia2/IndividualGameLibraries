using LifeBoardGameCP;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using System.Windows.Controls;
using System.Windows.Input;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace LifeBoardGameWPF
{
    public class SpinnerCanvasWPF : SKElement, ISpinnerCanvas
    {
        public int Position { get; set; }
        public int HighSpeedPhase { get; set; }
        public void Repaint()
        {
            InvalidateVisual();
        }
    }
    public class SpinnerWPF : UserControl
    {
        internal SpinnerCanvasWPF? ThisElement;
        SKElement? _mainElement;
        private SpinnerCP? _thisBoard; //you have to register.
        LifeBoardGameViewModel? _thisMod;
        public void LoadBoard()
        {
            ThisElement = new SpinnerCanvasWPF();
            _thisMod = Resolve<LifeBoardGameViewModel>();
            _thisBoard = Resolve<SpinnerCP>();
            GlobalVariableClass thisG = Resolve<GlobalVariableClass>();
            thisG.SpinnerCanvas = ThisElement;
            MouseUp += SpinnerWPF_MouseUp;
            ThisElement.PaintSurface += ThisElement_PaintSurface;
            var thisSize = _thisBoard.SuggestedBoardSize;
            Width = thisSize;
            Height = thisSize;
            _mainElement = new SKElement();
            _mainElement.PaintSurface += MainElementPaint;
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(_mainElement);
            thisGrid.Children.Add(ThisElement);
            Content = thisGrid;
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
        private void SpinnerWPF_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_thisMod!.SpinCommand!.CanExecute(null!))
                _thisMod.SpinCommand.Execute(null!);
        }
    }
}