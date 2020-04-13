using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGamingUIWPFLibrary.GameGraphics.Base;
using RummyDiceCP.Data;
using RummyDiceCP.Logic;
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
        private readonly SKElement _thisDraw;

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

        public ICommand? Command { get; set; }

        private RummyBoardCP? _board;
        public void SendDiceInfo(RummyDiceInfo thisDice, RummyBoardCP board)
        {
            _mains = new RummyDiceGraphicsCP(this);
            _board = board;
            MouseUp += RummyDiceGraphicsWPF_MouseUp;
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

        private async void RummyDiceGraphicsWPF_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_board == null && Command == null)
            {
                return;
            }
            RummyDiceInfo dice = (RummyDiceInfo)DataContext;
            if (Command != null)
            {
                if (Command.CanExecute(null) == false)
                {
                    return;
                }
                Command.Execute(dice);
                return;
            }

            await _board!.SelectDiceAsync((RummyDiceInfo)DataContext);
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
            Content = _thisDraw;
        }
        private void ThisDraw_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            _mains!.DrawDice(e.Surface.Canvas);
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