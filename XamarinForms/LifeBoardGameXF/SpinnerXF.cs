using BasicGamingUIXFLibrary.GameGraphics.Base;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.ViewLinkersPlusBinders;
using CommonBasicStandardLibraries.Messenging;
using LifeBoardGameCP.Data;
using LifeBoardGameCP.Graphics;
using LifeBoardGameCP.ViewModels;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace LifeBoardGameXF
{
    public class SpinnerXF : GraphicsCommand
    {
        internal SpinnerCanvasXF? Element { get; set; }
        private readonly SpinnerCP? _thisBoard; //you have to register.
        private readonly LifeBoardGameGameContainer _gameContainer;

        public SpinnerXF(IEventAggregator aggregator)
        {
            Element = new SpinnerCanvasXF(aggregator);

            _thisBoard = Resolve<SpinnerCP>();
            _gameContainer = Resolve<LifeBoardGameGameContainer>();
            GamePackageViewModelBinder.ManuelElements.Clear();
            this.SetName(nameof(SpinnerViewModel.SpinAsync)); //hopefully this simple (?)
            GamePackageViewModelBinder.ManuelElements.Add(this);
            Element.PaintSurface += ThisElement_PaintSurface;
            var thisSize = _thisBoard.SuggestedBoardSize;
            WidthRequest = thisSize;
            HeightRequest = thisSize;
            ThisDraw.PaintSurface += MainElementPaint;
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(ThisDraw);
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
    }
}