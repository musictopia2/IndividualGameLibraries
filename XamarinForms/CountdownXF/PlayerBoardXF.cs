using BaseGPXPagesAndControlsXF.BasicControls.GameBoards;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.DIContainers;
using CommonBasicStandardLibraries.Messenging;
using CountdownCP;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace CountdownXF
{
    public class PlayerBoardXF : ContentView, IHandle<RepaintEventModel>
    {
        internal SkiaSharpGameBoardXF ThisElement;
        PlayerBoardCP? _privateBoard;
        private bool _hasLoaded = false;
        public void LoadBoard(CountdownPlayerItem thisPlayer)
        {
            _privateBoard = new PlayerBoardCP((IGamePackageResolver)cons, ThisElement, thisPlayer); //hopefully no problem.
            SKSize thisSize = _privateBoard.SuggestedSize();
            WidthRequest = thisSize.Width;
            VerticalOptions = LayoutOptions.Start;
            HorizontalOptions = LayoutOptions.Start;
            HeightRequest = thisSize.Height;
            EventAggregator thisT = Resolve<EventAggregator>();
            thisT.Subscribe(this, EnumRepaintCategories.fromskiasharpboard.ToString());
            Content = ThisElement;
            _hasLoaded = true;
            ThisElement.InvalidateSurface(); //i think
        }
        public void UpdateBoard(CountdownPlayerItem ThisPlayer)
        {
            _privateBoard!.UpdatePlayer(ThisPlayer);
        }
        private void ThisElement_Touch(object sender, SKTouchEventArgs e)
        {
            if (_hasLoaded == false)
                return;
            ThisElement.StartClick(e.Location.X, e.Location.Y);
        }
        public PlayerBoardXF()
        {
            ThisElement = new SkiaSharpGameBoardXF();
            ThisElement.EnableTouchEvents = true;
            ThisElement.Touch += ThisElement_Touch;
            ThisElement.PaintSurface += ThisElement_PaintSurface;
        }
        private void ThisElement_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            if (_hasLoaded == false)
                return;
            ThisElement.StartInvalidate(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
        public void Handle(RepaintEventModel message)
        {
            ThisElement.InvalidateSurface();
        }
    }
}