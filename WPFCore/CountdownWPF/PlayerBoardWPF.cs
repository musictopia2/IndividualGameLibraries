using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIWPFLibrary.BasicControls.GameBoards;
using CommonBasicStandardLibraries.Messenging;
using CountdownCP.Data;
using CountdownCP.Graphics;
using SkiaSharp;
using System.Windows;
using System.Windows.Controls;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace CountdownWPF
{
    public class PlayerBoardWPF : UserControl, IHandle<RepaintEventModel>
    {
        internal SkiaSharpGameBoard Element { get; set; }
        private bool _hasLoaded = false;
        internal void Dispose()
        {
            if (_aggregator != null)
            {
                _aggregator.Unsubscribe(this);
            }
        }
        public PlayerBoardWPF()
        {
            Element = new SkiaSharpGameBoard();
            Element.MouseUp += ThisElement_MouseUp;
            Element.PaintSurface += ThisElement_PaintSurface;
        }
        PlayerBoardCP? _privateBoard;
        IEventAggregator? _aggregator;
        public void LoadBoard(CountdownPlayerItem thisPlayer, CountdownGameContainer gameContainer)
        {
            _privateBoard = new PlayerBoardCP(gameContainer, Element, thisPlayer); //hopefully no problem.
            SKSize thisSize = _privateBoard.SuggestedSize();
            Width = thisSize.Width;
            Height = thisSize.Height;
            _aggregator = gameContainer.Aggregator;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            EventAggregator thisT = Resolve<EventAggregator>();
            thisT.Subscribe(this, EnumRepaintCategories.FromSkiasharpboard.ToString()); //i think this is it.  if i am wrong, rethink
            Content = Element;
            _hasLoaded = true;
            Element.InvalidateVisual(); //i think
        }
        public void UpdateBoard(CountdownPlayerItem ThisPlayer)
        {
            _privateBoard!.UpdatePlayer(ThisPlayer);
        }
        private void ThisElement_PaintSurface(object? sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            if (_hasLoaded == false)
                return;
            Element.StartInvalidate(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
        private void ThisElement_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_hasLoaded == false)
                return;
            var position = e.GetPosition(Element);
            Element.StartClick(position.X, position.Y);
        }
        public void Handle(RepaintEventModel message)
        {
            Element.InvalidateVisual();
        }
    }
}