using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIXFLibrary.GameGraphics.Base;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.ViewLinkersPlusBinders;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp.Views.Forms;
using TicTacToeCP.Data;
using TicTacToeCP.Logic;
using TicTacToeCP.ViewModels;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace TicTacToeXF
{
    public class SpaceXF : GraphicsCommand, IHandle<RepaintEventModel>
    {
        private readonly TicTacToeGraphicsCP _gameBoard1;
        public SpaceXF(SpaceInfoCP space)
        {
            ThisDraw.PaintSurface += DrawPaint;
            _gameBoard1 = Resolve<TicTacToeGraphicsCP>();
            this.SetName(nameof(TicTacToeMainViewModel.MakeMoveAsync));
            CommandParameter = space;
            GamePackageViewModelBinder.ManuelElements.Add(this); //hopefully this simple.
            EventAggregator thisE = Resolve<EventAggregator>();
            BindingContext = space;
            WidthRequest = _gameBoard1.SpaceSize;
            HeightRequest = _gameBoard1.SpaceSize;
            thisE.Subscribe(this, EnumRepaintCategories.FromSkiasharpboard.ToString());
        }
        private void DrawPaint(object? sender, SKPaintSurfaceEventArgs e)
        {
            var thisSpace = (SpaceInfoCP)BindingContext;
            _gameBoard1.DrawSpace(e.Surface.Canvas, thisSpace, e.Info.Width, e.Info.Height);
        }
        public void Handle(RepaintEventModel message)
        {
            ThisDraw.InvalidateSurface();
        }
    }
}
