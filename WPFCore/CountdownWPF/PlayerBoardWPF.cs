using BaseGPXWindowsAndControlsCore.BasicControls.GameBoards;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.DIContainers;
using CommonBasicStandardLibraries.Messenging;
using CountdownCP;
using SkiaSharp;
using System.Windows;
using System.Windows.Controls;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace CountdownWPF
{
    public class PlayerBoardWPF : UserControl, IHandle<RepaintEventModel>
    {
        internal SkiaSharpGameBoard ThisElement;
        private bool _hasLoaded = false;
        public PlayerBoardWPF()
        {
            ThisElement = new SkiaSharpGameBoard();
            ThisElement.MouseUp += ThisElement_MouseUp;
            ThisElement.PaintSurface += ThisElement_PaintSurface;
        }
        PlayerBoardCP? _privateBoard;
        public void LoadBoard(CountdownPlayerItem thisPlayer)
        {
            _privateBoard = new PlayerBoardCP((IGamePackageResolver)cons, ThisElement, thisPlayer); //hopefully no problem.
            SKSize thisSize = _privateBoard.SuggestedSize();
            Width = thisSize.Width;
            Height = thisSize.Height;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            EventAggregator thisT = Resolve<EventAggregator>();
            thisT.Subscribe(this, EnumRepaintCategories.fromskiasharpboard.ToString()); //i think this is it.  if i am wrong, rethink
            Content = ThisElement;
            _hasLoaded = true;
            ThisElement.InvalidateVisual(); //i think
        }
        public void UpdateBoard(CountdownPlayerItem ThisPlayer)
        {
            _privateBoard!.UpdatePlayer(ThisPlayer);
        }
        private void ThisElement_PaintSurface(object? sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            if (_hasLoaded == false)
                return;
            ThisElement.StartInvalidate(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
        private void ThisElement_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_hasLoaded == false)
                return;
            var ThisPos = e.GetPosition(ThisElement);
            ThisElement.StartClick(ThisPos.X, ThisPos.Y);
        }
        public void Handle(RepaintEventModel message)
        {
            ThisElement.InvalidateVisual();
        }
    }
}