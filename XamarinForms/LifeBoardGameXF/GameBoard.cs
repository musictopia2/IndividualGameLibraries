using BasicGamingUIXFLibrary.BasicControls.GameBoards;
using BasicGameFrameworkLibrary.BasicEventModels;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using LifeBoardGameCP.Graphics;

namespace LifeBoardGameXF
{
    public class GameBoardXF : ContentView, IHandle<RepaintEventModel>
    {
        private readonly IEventAggregator _aggregator;

        internal SkiaSharpGameBoardXF Element { get; set; }

        public GameBoardXF(IEventAggregator aggregator)
        {
            Element = new SkiaSharpGameBoardXF();
            _aggregator = aggregator;
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

        internal void BeforeClosing()
        {
            _aggregator.Unsubscribe(this); //just in case.
        }

        private void ThisElement_Touch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            Element.StartClick(e.Location.X, e.Location.Y);
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