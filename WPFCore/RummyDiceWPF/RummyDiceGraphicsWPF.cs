using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using RummyDiceCP;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using SkiaSharpGeneralLibrary.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
namespace RummyDiceWPF
{
    public class RummyDiceGraphicsWPF : UserControl, ISelectableObject, IRepaintControl
    {
        private RummyDiceGraphicsCP? _mains; //just keep mains

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(RummyDiceGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(CommandPropertyChanged)));
        private readonly SKElement _thisDraw;
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }
        private static void CommandPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) { }
        private static void CommandParameterPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) { }
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(RummyDiceGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(CommandParameterPropertyChanged)));
        public object CommandParameter
        {
            get
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(RummyDiceGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(IsSelectedPropertyChanged)));
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }
        private static void IsSelectedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (RummyDiceGraphicsWPF)sender;
            thisItem._mains!.IsSelected = (bool)e.NewValue;
        }
        public void Init() // the actual is not working at this moment until the property change happens
        {
            double thisD = Width - Margin.Left - Margin.Right;
            _mains!.ActualWidthHeight = (float)thisD;
            _thisDraw.InvalidateVisual();
        }
        public void SendDiceInfo(RummyDiceInfo thisDice)
        {
            _mains = new RummyDiceGraphicsCP(this);
            _mains.MinimumWidthHeight = thisDice.HeightWidth;
            SetBinding(IsSelectedProperty, new Binding(nameof(RummyDiceInfo.IsSelected))); //maybe i forgot this too.
            SetBinding(VisibilityProperty, new Binding(nameof(RummyDiceInfo.Visible))); //i think.
            SetBinding(ColorProperty, new Binding(nameof(RummyDiceInfo.Color)));
            SetBinding(ValueProperty, new Binding(nameof(RummyDiceInfo.Display))); //decided to bind to display now.
            SKSize tempSize = new SKSize(thisDice.HeightWidth, thisDice.HeightWidth);
            IProportionImage thisP = new CustomProportionWPF();
            SKSize diceSize = tempSize.GetSizeUsed(thisP.Proportion);
            DataContext = thisDice;
            Height = diceSize.Height;
            Width = diceSize.Width;
            Init();
        }
        public void DoInvalidate()
        {
            _thisDraw.InvalidateVisual();
        }
        public RummyDiceGraphicsWPF()
        {
            _thisDraw = new SKElement();
            _thisDraw.PaintSurface += ThisDraw_PaintSurface;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            MouseUp += BaseGraphics_MouseUp;
            Content = _thisDraw;
        }
        private void ThisDraw_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            _mains!.DrawDice(e.Surface.Canvas);
        }
        private void BaseGraphics_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var tempCommand = Command;
            if (tempCommand != null)
            {
                if (tempCommand.CanExecute(CommandParameter) == true)
                    tempCommand.Execute(CommandParameter);
            }
        }
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(EnumColorType), typeof(RummyDiceGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(ColorPropertyChanged)));
        public EnumColorType Color
        {
            get
            {
                return (EnumColorType)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }
        private static void ColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (RummyDiceGraphicsWPF)sender;
            thisItem._mains!.Color = (EnumColorType)e.NewValue;
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(RummyDiceGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(ValuePropertyChanged)));
        public string Value
        {
            get
            {
                return (string)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }
        private static void ValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (RummyDiceGraphicsWPF)sender;
            thisItem._mains!.Value = (string)e.NewValue;
        }
    }
}