using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using SkiaSharpGeneralLibrary.Interfaces;
using System.Windows;
using System.Windows.Controls;
using ThinkTwiceCP;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace ThinkTwiceWPF
{
    public class MultWPF : UserControl, IRepaintControl
    {
        private ButtonDiceGraphicsCP? _mains;
        private readonly SKElement _thisDraw;
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MultWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(TextPropertyChanged)));
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }
        private static void TextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (MultWPF)sender;
            thisItem!._mains!.Text = e.NewValue.ToString()!;
        }
        public static readonly DependencyProperty DiceSizeProperty = DependencyProperty.Register("DiceSize", typeof(SKSize), typeof(MultWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(DiceSizePropertyChanged)));
        public SKSize DiceSize
        {
            get
            {
                return (SKSize)GetValue(DiceSizeProperty);
            }
            set
            {
                SetValue(DiceSizeProperty, value);
            }
        }
        private static void DiceSizePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) { }
        public static string GetDiceTag => "StandardDice"; //same as other dice.
        public void SendDiceInfo(Multiplier thisDice) //it did send dice
        {
            IGamePackageResolver thisR = (IGamePackageResolver)cons;
            IProportionImage thisP = thisR.Resolve<IProportionImage>(GetDiceTag);
            _mains = new ButtonDiceGraphicsCP();
            _mains.PaintUI = this;
            _mains.MinimumWidthHeight = thisDice.HeightWidth;
            SetBinding(TextProperty, nameof(Multiplier.Value));
            SKSize tempSize = new SKSize(thisDice.HeightWidth, thisDice.HeightWidth);
            DiceSize = tempSize.GetSizeUsed(thisP.Proportion);
            Height = DiceSize.Height;
            Width = DiceSize.Width;
            DataContext = thisDice;
            Init();
        }
        private void BeforePainting()
        {
            var thisDice = (Multiplier)DataContext;
            _mains!.MinimumWidthHeight = thisDice.HeightWidth;
        }
        public MultWPF()
        {
            _thisDraw = new SKElement();
            _thisDraw.PaintSurface += ThisDraw_PaintSurface;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            Content = _thisDraw;
        }
        private void ThisDraw_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            if (_mains == null)
                return;
            BeforePainting();
            _mains.DrawDice(e.Surface.Canvas);
        }
        private void Init() // the actual is not working at this moment until the property change happens
        {
            double ThisD = Width - Margin.Left - Margin.Right;
            _mains!.ActualWidthHeight = (float)ThisD;
            _thisDraw.InvalidateVisual();
        }
        public void DoInvalidate()
        {
            _thisDraw.InvalidateVisual();
        }
    }
}