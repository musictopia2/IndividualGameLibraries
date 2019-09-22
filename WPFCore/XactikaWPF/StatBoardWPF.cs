using BaseGPXWindowsAndControlsCore.BasicControls.GameBoards;
using BasicGameFramework.BasicEventModels;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Windows.Controls;
using XactikaCP;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace XactikaWPF
{
    public class StatBoardWPF : UserControl, IHandle<RepaintEventModel>
    {
        internal SkiaSharpGameBoard ThisElement;
        public void LoadBoard()
        {
            StatsBoardCP privateBoard = Resolve<StatsBoardCP>();
            ThisElement.PaintSurface += ThisElement_PaintSurface;
            SKSize thisSize = privateBoard.SuggestedSize();
            Width = thisSize.Width;
            Height = thisSize.Height;
            EventAggregator thisT = Resolve<EventAggregator>();
            thisT.Subscribe(this, EnumRepaintCategories.fromskiasharpboard.ToString());
            Content = ThisElement;
        }
        public StatBoardWPF()
        {
            ThisElement = new SkiaSharpGameBoard();
        }
        private void ThisElement_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            ThisElement.StartInvalidate(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
        public void Handle(RepaintEventModel message)
        {
            ThisElement.InvalidateVisual();
        }
    }
}