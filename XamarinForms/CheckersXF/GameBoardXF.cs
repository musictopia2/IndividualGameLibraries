using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIXFLibrary.BasicControls.GameBoards;
using CheckersCP.Logic;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace CheckersXF
{
    public class GameBoardXF : ContentView, IHandle<RepaintEventModel>
    {
        internal SkiaSharpGameBoardXF Element { get; set; }
        public void LoadBoard()
        {
            GameBoardGraphicsCP PrivateBoard = Resolve<GameBoardGraphicsCP>();

            Element.EnableTouchEvents = true;
            Element.Touch += ThisElement_Touch;


            Element.PaintSurface += ThisElement_PaintSurface;
            SKSize thisSize = PrivateBoard.SuggestedSize();

            WidthRequest = thisSize.Width;
            VerticalOptions = LayoutOptions.Start;
            HorizontalOptions = LayoutOptions.Start;

            HeightRequest = WidthRequest;
            EventAggregator thisT = Resolve<EventAggregator>();
            thisT.Subscribe(this, EnumRepaintCategories.FromSkiasharpboard.ToString());
            Content = Element;
        }
        private void ThisElement_Touch(object sender, SKTouchEventArgs e)
        {
            Element.StartClick(e.Location.X, e.Location.Y);
        }
        public GameBoardXF()
        {
            Element = new SkiaSharpGameBoardXF();
        }
        private void ThisElement_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            //HeightRequest = e.Info.Width - 100;//try this way now.
            Element.StartInvalidate(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
        public void Handle(RepaintEventModel message)
        {
            Element.InvalidateSurface();
        }
    }
}