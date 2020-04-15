using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIXFLibrary.BasicControls.GameBoards;
using CommonBasicStandardLibraries.Messenging;
using RollEmCP.Logic;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace RollEmXF
{
    public class GameBoardXF : ContentView, IHandle<RepaintEventModel>
    {

        internal void Dispose()
        {
            EventAggregator thisT = Resolve<EventAggregator>();
            thisT.Unsubscribe(this);
        }
        internal SkiaSharpGameBoardXF Element { get; set; }
        public void LoadBoard()
        {
            GameBoardGraphicsCP privateBoard = Resolve<GameBoardGraphicsCP>();
            Element.EnableTouchEvents = true;
            Element.Touch += ThisElement_Touch;
            Element.PaintSurface += ThisElement_PaintSurface;
            SKSize thisSize = privateBoard.SuggestedSize();
            WidthRequest = thisSize.Width;
            HeightRequest = thisSize.Height;
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Start;
            EventAggregator thisT = Resolve<EventAggregator>();
            thisT.Subscribe(this, EnumRepaintCategories.FromSkiasharpboard.ToString());
            Content = Element;
        }
        private void ThisElement_Touch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            Element.StartClick(e.Location.X, e.Location.Y);
        }
        public GameBoardXF()
        {
            Element = new SkiaSharpGameBoardXF();
        }
        private void ThisElement_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            Element.StartInvalidate(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
        public void Handle(RepaintEventModel message)
        {
            Element.InvalidateSurface();
        }
    }
}