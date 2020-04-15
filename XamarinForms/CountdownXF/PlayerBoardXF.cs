using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIXFLibrary.BasicControls.GameBoards;
using CommonBasicStandardLibraries.Messenging;
using CountdownCP.Data;
using CountdownCP.Graphics;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace CountdownXF
{
    public class PlayerBoardXF : ContentView, IHandle<RepaintEventModel>
    {
        internal SkiaSharpGameBoardXF Element { get; set; }
        PlayerBoardCP? _privateBoard;
        private bool _hasLoaded = false;
        IEventAggregator? _aggregator;
        internal void Dispose()
        {
            if (_aggregator != null)
            {
                _aggregator.Unsubscribe(this);
            }
        }

        public void LoadBoard(CountdownPlayerItem thisPlayer, CountdownGameContainer gameContainer)
        {
            _privateBoard = new PlayerBoardCP(gameContainer, Element, thisPlayer); //hopefully no problem.
            _aggregator = gameContainer.Aggregator;
            SKSize thisSize = _privateBoard.SuggestedSize();
            WidthRequest = thisSize.Width;
            VerticalOptions = LayoutOptions.Start;
            HorizontalOptions = LayoutOptions.Start;
            HeightRequest = thisSize.Height;
            EventAggregator thisT = Resolve<EventAggregator>();
            thisT.Subscribe(this, EnumRepaintCategories.FromSkiasharpboard.ToString());
            Content = Element;
            _hasLoaded = true;
            Element.InvalidateSurface(); //i think
        }
        private void ThisElement_Touch(object sender, SKTouchEventArgs e)
        {
            if (_hasLoaded == false)
                return;
            Element.StartClick(e.Location.X, e.Location.Y);
        }
        public PlayerBoardXF()
        {
            Element = new SkiaSharpGameBoardXF();
            Element.EnableTouchEvents = true;
            Element.Touch += ThisElement_Touch;
            Element.PaintSurface += ThisElement_PaintSurface;
        }
        private void ThisElement_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            if (_hasLoaded == false)
                return;
            Element.StartInvalidate(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
        public void Handle(RepaintEventModel message)
        {
            Element.InvalidateSurface();
        }
    }
}