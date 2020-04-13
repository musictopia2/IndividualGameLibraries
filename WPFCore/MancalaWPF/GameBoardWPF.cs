using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIWPFLibrary.BasicControls.GameBoards;
using CommonBasicStandardLibraries.Messenging;
using MancalaCP.Logic;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Windows.Controls;
using System.Windows.Input;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace MancalaWPF
{
    public class GameBoardWPF : UserControl, IHandle<RepaintEventModel>
    {
        internal SkiaSharpGameBoard Element { get; set; }
        public void LoadBoard()
        {
            GameBoardGraphicsCP privateBoard = Resolve<GameBoardGraphicsCP>();
            MouseUp += GameboardWPF_MouseUp;
            Element.PaintSurface += ThisElement_PaintSurface;
            SKSize thisSize = privateBoard.SuggestedSize();
            Width = thisSize.Width;
            Height = thisSize.Height;
            EventAggregator aggregator = Resolve<EventAggregator>();
            aggregator.Subscribe(this, EnumRepaintCategories.FromSkiasharpboard.ToString());
            Content = Element;
        }
        public GameBoardWPF()
        {
            Element = new SkiaSharpGameBoard();
        }
        private void ThisElement_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            Element.StartInvalidate(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
        private void GameboardWPF_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var ThisPos = e.GetPosition(Element);
            Element.StartClick(ThisPos.X, ThisPos.Y);
        }
        public void Handle(RepaintEventModel message)
        {
            Element.InvalidateVisual();
        }
    }
}