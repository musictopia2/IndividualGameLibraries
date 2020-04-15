using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIXFLibrary.BasicControls.GameBoards;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using XactikaCP.MiscImages;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace XactikaXF
{
    public class StatBoardXF : ContentView, IHandle<RepaintEventModel>
    {
        internal SkiaSharpGameBoardXF Element { get; set; }
        public void LoadBoard()
        {
            StatsBoardCP privateBoard = Resolve<StatsBoardCP>();
            Element.PaintSurface += ThisElement_PaintSurface;
            SKSize thisSize = privateBoard.SuggestedSize(); //hopefully this simple (?)
            WidthRequest = thisSize.Width;
            HeightRequest = thisSize.Height;
            EventAggregator thisT = Resolve<EventAggregator>();
            thisT.Subscribe(this, EnumRepaintCategories.FromSkiasharpboard.ToString());
            Content = Element;
        }
        public StatBoardXF()
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