using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIWPFLibrary.BasicControls.GameBoards;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Windows.Controls;
using XactikaCP.MiscImages;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace XactikaWPF
{
    public class StatBoardWPF : UserControl, IHandle<RepaintEventModel>
    {
        internal SkiaSharpGameBoard Element { get; set; }
        public void LoadBoard()
        {
            StatsBoardCP privateBoard = Resolve<StatsBoardCP>();
            Element.PaintSurface += ThisElement_PaintSurface;
            SKSize thisSize = privateBoard.SuggestedSize();
            Width = thisSize.Width;
            Height = thisSize.Height;
            EventAggregator thisT = Resolve<EventAggregator>();
            thisT.Subscribe(this, EnumRepaintCategories.FromSkiasharpboard.ToString());
            Content = Element;
        }
        public StatBoardWPF()
        {
            Element = new SkiaSharpGameBoard();
        }
        private void ThisElement_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            Element.StartInvalidate(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
        public void Handle(RepaintEventModel message)
        {
            Element.InvalidateVisual();
        }
    }
}
