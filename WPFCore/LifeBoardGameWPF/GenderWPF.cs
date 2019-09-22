using BasicGameFramework.BasicEventModels;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using LifeBoardGameCP;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace LifeBoardGameWPF
{
    public class GenderWPF : UserControl, IHandle<NewTurnEventModel>, IHandle<RepaintEventModel>
    {
        private readonly EnumGender _thisGender;
        private readonly SKElement _thisElement;
        private readonly SKPaint _thisPaint;
        readonly LifeBoardGameViewModel _thisMod;
        public GenderWPF(EnumGender gender)
        {
            _thisMod = Resolve<LifeBoardGameViewModel>();
            _thisGender = gender;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            Width = 200;
            Height = 200;
            if (gender == EnumGender.Boy)
                _thisPaint = MiscHelpers.GetSolidPaint(SKColors.Blue);
            else if (gender == EnumGender.Girl)
                _thisPaint = MiscHelpers.GetSolidPaint(SKColors.DeepPink);
            else
                throw new BasicBlankException("Must choose boy or girl");
            _thisElement = new SKElement();
            _thisElement.PaintSurface += ThisElement_PaintSurface;
            EventAggregator thisE = Resolve<EventAggregator>();
            thisE.Subscribe(this, "");
            thisE.Subscribe(this, EnumRepaintCategories.frombeginning.ToString());
            Visibility = Visibility.Collapsed; //has to be proven true;
            MouseUp += GenderWPF_MouseUp;
            Content = _thisElement;
        }
        private void GenderWPF_MouseUp(object sender, MouseButtonEventArgs e)
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
            Visibility = Visibility.Visible;
        }
        public void Handle(RepaintEventModel Message)
        {
            if (_thisMod.GenderSelected == _thisGender)
                Visibility = Visibility.Visible;
            else
                Visibility = Visibility.Collapsed;
        }
    }
}