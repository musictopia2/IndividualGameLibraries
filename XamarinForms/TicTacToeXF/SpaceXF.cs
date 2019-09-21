using BasicGameFramework.BasicEventModels;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp.Views.Forms;
using TicTacToeCP;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace TicTacToeXF
{
    public class SpaceXF : ContentView, IHandle<RepaintEventModel>
    {
        private readonly SKCanvasView _thisDraw;
        private readonly TicTacToeViewModel _thisMod;
        private readonly TicTacToeGraphicsCP _gameBoard1;
        public SpaceXF(SpaceInfoCP space)
        {
            _thisDraw = new SKCanvasView();
            _thisDraw.PaintSurface += DrawPaint;
            _thisDraw.EnableTouchEvents = true;
            _thisDraw.Touch += DrawTouch;
            _thisMod = Resolve<TicTacToeViewModel>();
            _gameBoard1 = Resolve<TicTacToeGraphicsCP>();
            EventAggregator thisE = Resolve<EventAggregator>();
            BindingContext = space;
            WidthRequest = _gameBoard1.SpaceSize;
            HeightRequest = _gameBoard1.SpaceSize;
            thisE.Subscribe(this, EnumRepaintCategories.fromskiasharpboard.ToString());
            Content = _thisDraw;
        }
        private void DrawTouch(object sender, SKTouchEventArgs e)
        {
            if (_thisMod.SpaceCommand!.CanExecute(BindingContext) == true)
                _thisMod.SpaceCommand.Execute(BindingContext);
        }
        private void DrawPaint(object? sender, SKPaintSurfaceEventArgs e)
        {
            var thisSpace = (SpaceInfoCP)BindingContext;
            _gameBoard1.DrawSpace(e.Surface.Canvas, thisSpace, e.Info.Width, e.Info.Height);
        }
        public void Handle(RepaintEventModel message)
        {
            _thisDraw.InvalidateSurface();
        }
    }
}
