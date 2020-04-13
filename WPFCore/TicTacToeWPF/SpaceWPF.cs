using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIWPFLibrary.GameGraphics.Base;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp.Views.WPF;
using TicTacToeCP.Data;
using TicTacToeCP.Logic;
using TicTacToeCP.ViewModels;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace TicTacToeWPF
{
    public class SpaceWPF : GraphicsCommand, IHandle<RepaintEventModel>
    {
        private readonly SKElement _thisDraw;
        //private readonly TicTacToeViewModel _thisMod;
        private readonly TicTacToeGraphicsCP _gameBoard1;
        public SpaceWPF(SpaceInfoCP space)
        {
            _thisDraw = new SKElement();
            _thisDraw.PaintSurface += DrawPaint;
            Name = nameof(TicTacToeMainViewModel.MakeMoveAsync);
            CommandParameter = space;
            //_thisMod = Resolve<TicTacToeViewModel>();
            _gameBoard1 = Resolve<TicTacToeGraphicsCP>();
            EventAggregator thisE = Resolve<EventAggregator>();
            DataContext = space;
            Width = _gameBoard1.SpaceSize;
            Height = _gameBoard1.SpaceSize;
            thisE.Subscribe(this, EnumRepaintCategories.FromSkiasharpboard.ToString());
            GamePackageViewModelBinder.ManuelElements.Add(this); //hopefully this simple.
            //MouseUp += SpaceWPF_MouseUp;
            Content = _thisDraw;
        }
        //private void SpaceWPF_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    if (_thisMod.SpaceCommand!.CanExecute(DataContext) == true)
        //        _thisMod.SpaceCommand.Execute(DataContext);
        //}
        private void DrawPaint(object? sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            var thisSpace = (SpaceInfoCP)DataContext;
            _gameBoard1.DrawSpace(e.Surface.Canvas, thisSpace, e.Info.Width, e.Info.Height);
        }
        public void Handle(RepaintEventModel message)
        {
            _thisDraw.InvalidateVisual();
        }
    }
}
