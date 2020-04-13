using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIWPFLibrary.BasicControls.GameBoards;
using CommonBasicStandardLibraries.Messenging;
using DominosMexicanTrainCP.Logic;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Windows.Controls;
using System.Windows.Input;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace DominosMexicanTrainWPF
{
    public class TrainStationWPF : UserControl, IHandle<RepaintEventModel>
    {
        internal SkiaSharpGameBoard Element { get; set; }
        public void LoadBoard()
        {
            TrainStationGraphicsCP privateBoard = Resolve<TrainStationGraphicsCP>();
            MouseUp += GameboardWPF_MouseUp;
            Element.PaintSurface += ThisElement_PaintSurface;
            SKSize ThisSize = privateBoard.SuggestedSize();
            Width = ThisSize.Width;
            Height = ThisSize.Height;
            EventAggregator thisT = Resolve<EventAggregator>();
            thisT.Subscribe(this, EnumRepaintCategories.FromSkiasharpboard.ToString());
            Content = Element;
        }
        public TrainStationWPF()
        {
            Element = new SkiaSharpGameBoard();
        }
        private void ThisElement_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            Element.StartInvalidate(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
        private void GameboardWPF_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var thisPos = e.GetPosition(Element);
            Element.StartClick(thisPos.X, thisPos.Y);
        }
        public void Handle(RepaintEventModel message)
        {
            Element.InvalidateVisual();
        }
    }
}