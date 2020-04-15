using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using RummyDiceCP.Data;
using RummyDiceCP.Logic;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using SkiaSharpGeneralLibrary.Interfaces;
using System.Windows.Input;
using Xamarin.Forms;
namespace RummyDiceXF
{
    public class RummyDiceGraphicsXF : ContentView, ISelectableObject, IRepaintControl
    {
        private RummyDiceGraphicsCP? _mains; //just keep mains
        private readonly SKCanvasView _thisDraw;
        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(propertyName: "IsSelected", defaultValue: false, returnType: typeof(bool), declaringType: typeof(RummyDiceGraphicsXF), defaultBindingMode: BindingMode.TwoWay, propertyChanged: IsSelectedPropertyChanged);
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                base.SetValue(IsSelectedProperty, value);
            }
        }
        private static void IsSelectedPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (RummyDiceGraphicsXF)bindable;
            thisItem._mains!.IsSelected = (bool)newValue;
        }
        public static readonly BindableProperty ColorProperty = BindableProperty.Create(propertyName: "Color", returnType: typeof(EnumColorType), declaringType: typeof(RummyDiceGraphicsXF), defaultValue: EnumColorType.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ColorPropertyChanged);
        public EnumColorType Color
        {
            get
            {
                return (EnumColorType)GetValue(ColorProperty);
            }
            set
            {
                base.SetValue(ColorProperty, value);
            }
        }
        public ICommand? Command { get; set; }
        private static void ColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (RummyDiceGraphicsXF)bindable;
            thisItem._mains!.Color = (EnumColorType)newValue;
        }
        public static readonly BindableProperty ValueProperty = BindableProperty.Create(propertyName: "Value", returnType: typeof(string), declaringType: typeof(RummyDiceGraphicsXF), defaultBindingMode: BindingMode.TwoWay, propertyChanged: ValuePropertyChanged);
        public string Value
        {
            get
            {
                return (string)GetValue(ValueProperty);
            }
            set
            {
                base.SetValue(ValueProperty, value);
            }
        }
        private static void ValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (RummyDiceGraphicsXF)bindable;
            thisItem._mains!.Value = (string)newValue;
        }
        public void Init() // the actual is not working at this moment until the property change happens
        {
            double thisD = WidthRequest - Margin.Left - Margin.Right;
            _mains!.ActualWidthHeight = (float)thisD;
            _thisDraw.InvalidateSurface();
        }
        private RummyBoardCP? _board;
        public void SendDiceInfo(RummyDiceInfo thisDice, RummyBoardCP board)
        {
            _board = board;
            _mains = new RummyDiceGraphicsCP(this);
            _mains.MinimumWidthHeight = thisDice.HeightWidth;
            SetBinding(IsSelectedProperty, new Binding(nameof(RummyDiceInfo.IsSelected))); //maybe i forgot this too.
            SetBinding(IsVisibleProperty, new Binding(nameof(RummyDiceInfo.Visible))); //i think.
            SetBinding(ColorProperty, new Binding(nameof(RummyDiceInfo.Color)));
            SetBinding(ValueProperty, new Binding(nameof(RummyDiceInfo.Display))); //decided to bind to display now.
            SKSize tempSize = new SKSize(thisDice.HeightWidth, thisDice.HeightWidth);
            IProportionImage thisP = new CustomProportionXF();
            SKSize diceSize = tempSize.GetSizeUsed(thisP.Proportion);
            BindingContext = thisDice;
            HeightRequest = diceSize.Height;
            WidthRequest = diceSize.Width;
            Init();
        }
        public void DoInvalidate()
        {
            _thisDraw.InvalidateSurface();
        }
        public RummyDiceGraphicsXF()
        {
            _thisDraw = new SKCanvasView();
            _thisDraw.PaintSurface += ThisDraw_PaintSurface;
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Start;
            _thisDraw.EnableTouchEvents = true;
            _thisDraw.Touch += Touch;
            Content = _thisDraw;
        }
        private void ThisDraw_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            _mains!.DrawDice(e.Surface.Canvas);
        }
        private async void Touch(object sender, SKTouchEventArgs e)
        {
            if (_board == null && Command == null)
            {
                return;
            }
            RummyDiceInfo dice = (RummyDiceInfo)BindingContext;
            if (Command != null)
            {
                if (Command.CanExecute(null) == false)
                {
                    return;
                }
                Command.Execute(dice);
                return;
            }

            await _board!.SelectDiceAsync((RummyDiceInfo)BindingContext);
        }
    }
}