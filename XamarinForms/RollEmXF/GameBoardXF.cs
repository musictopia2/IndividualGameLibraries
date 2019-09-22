using BaseGPXPagesAndControlsXF.BasicControls.GameBoards;
using BasicGameFramework.BasicEventModels;
using CommonBasicStandardLibraries.Messenging;
using RollEmCP;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace RollEmXF
{
    public class GameBoardXF : ContentView, IHandle<RepaintEventModel>
    {
        internal SkiaSharpGameBoardXF ThisElement;
        public void LoadBoard()
        {
            GameBoardGraphicsCP privateBoard = Resolve<GameBoardGraphicsCP>();
            ThisElement.EnableTouchEvents = true;
            ThisElement.Touch += ThisElement_Touch;
            ThisElement.PaintSurface += ThisElement_PaintSurface;
            SKSize thisSize = privateBoard.SuggestedSize();
            WidthRequest = thisSize.Width;
            HeightRequest = thisSize.Height;
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Start;
            EventAggregator thisT = Resolve<EventAggregator>();
            thisT.Subscribe(this, EnumRepaintCategories.fromskiasharpboard.ToString());
            Content = ThisElement;
        }
        private void ThisElement_Touch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            ThisElement.StartClick(e.Location.X, e.Location.Y);
        }
        public GameBoardXF()
        {
            ThisElement = new SkiaSharpGameBoardXF();
        }
        private void ThisElement_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            ThisElement.StartInvalidate(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
        public void Handle(RepaintEventModel message)
        {
            ThisElement.InvalidateSurface();
        }
    }
}