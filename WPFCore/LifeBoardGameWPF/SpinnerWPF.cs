using BasicGamingUIWPFLibrary.GameGraphics.Base;
using CommonBasicStandardLibraries.Messenging;
using LifeBoardGameCP.Data;
using LifeBoardGameCP.Graphics;
using LifeBoardGameCP.ViewModels;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using System.Windows.Controls;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace LifeBoardGameWPF
{
    public class SpinnerWPF : GraphicsCommand
    {
        internal SpinnerCanvasWPF Element { get; set; }
        readonly SKElement _mainElement;
        private readonly SpinnerCP _thisBoard;
        private readonly LifeBoardGameGameContainer _gameContainer;
        public void BeforeClosing()
        {
            Element!.BeforeClosing();
        }
        public SpinnerWPF(IEventAggregator aggregator)
        {
            Element = new SpinnerCanvasWPF(aggregator);
            //_thisMod = Resolve<LifeBoardGameViewModel>();
            _thisBoard = Resolve<SpinnerCP>();
            _gameContainer = Resolve<LifeBoardGameGameContainer>();
            Name = nameof(SpinnerViewModel.SpinAsync); //hopefully this simple (?)
            //hopefully don't have to add to manuel list (?)
            //GlobalVariableClass thisG = Resolve<GlobalVariableClass>();
            //thisG.SpinnerCanvas = Element;
            //MouseUp += SpinnerWPF_MouseUp;
            Element.PaintSurface += ThisElement_PaintSurface;
            var thisSize = _thisBoard.SuggestedBoardSize;
            Width = thisSize;
            Height = thisSize;
            _mainElement = new SKElement();
            _mainElement.PaintSurface += MainElementPaint;
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(_mainElement);
            thisGrid.Children.Add(Element);
            Content = thisGrid;
        }

        private void MainElementPaint(object? sender, SKPaintSurfaceEventArgs e)
        {
            _thisBoard!.DrawSpinnerBoard(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
        private void ThisElement_PaintSurface(object? sender, SKPaintSurfaceEventArgs e) //iffy.
        {
            if (_gameContainer.HighSpeedPhase > 0)
            {
                _thisBoard!.DrawHighSpeedArrow(e.Surface.Canvas, e.Info.Width, e.Info.Height);
            }
            else
            {
                _thisBoard!.DrawNormalArrow(e.Surface.Canvas, _gameContainer.SpinnerPosition, e.Info.Height); //i think.
            }
        }
        //private void SpinnerWPF_MouseUp(object sender, MouseButtonEventArgs e)
        //{
        //    if (_thisMod!.SpinCommand!.CanExecute(null!))
        //        _thisMod.SpinCommand.Execute(null!);
        //}
    }
}
