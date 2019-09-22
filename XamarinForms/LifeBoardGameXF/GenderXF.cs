using BasicGameFramework.BasicEventModels;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using LifeBoardGameCP;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using SkiaSharpGeneralLibrary.SKExtensions;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace LifeBoardGameXF
{
    public class GenderXF : ContentView, IHandle<NewTurnEventModel>, IHandle<RepaintEventModel>
    {
        private readonly EnumGender _thisGender;
        private readonly SKCanvasView _thisElement;
        private readonly SKPaint _thisPaint;
        readonly LifeBoardGameViewModel _thisMod;
        public GenderXF(EnumGender gender)
        {
            _thisMod = Resolve<LifeBoardGameViewModel>();
            _thisGender = gender;
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Start;
            WidthRequest = 200;
            HeightRequest = 200;
            if (gender == EnumGender.Boy)
                _thisPaint = MiscHelpers.GetSolidPaint(SKColors.Blue);
            else if (gender == EnumGender.Girl)
                _thisPaint = MiscHelpers.GetSolidPaint(SKColors.DeepPink);
            else
                throw new BasicBlankException("Must choose boy or girl");
            _thisElement = new SKCanvasView();
            _thisElement.PaintSurface += ThisElement_PaintSurface;
            _thisElement.EnableTouchEvents = true;
            _thisElement.Touch += TouchEvent;
            EventAggregator thisE = Resolve<EventAggregator>();
            thisE.Subscribe(this, "");
            thisE.Subscribe(this, EnumRepaintCategories.frombeginning.ToString());
            IsVisible = false; //has to be proven true;
            Content = _thisElement;
        }
        private void TouchEvent(object sender, SKTouchEventArgs e)
        {
            if (_thisMod.GenderCommand!.CanExecute(_thisGender))
                _thisMod.GenderCommand.Execute(_thisGender);
        }
        private void ThisElement_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            var thisRect = SKRect.Create(0, 0, e.Info.Width, e.Info.Height);
            e.Surface.Canvas.DrawOval(thisRect, _thisPaint);
        }
        public void Handle(NewTurnEventModel message)
        {
            IsVisible = true;
        }
        public void Handle(RepaintEventModel Message)
        {
            if (_thisMod.GenderSelected == _thisGender)
                IsVisible = true;
            else
                IsVisible = false;
        }
    }
}