using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using RummyDiceCP;
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
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(propertyName: "Command", returnType: typeof(ICommand), declaringType: typeof(RummyDiceGraphicsXF), defaultBindingMode: BindingMode.TwoWay, propertyChanged: CommandPropertyChanged);
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
        private static void CommandPropertyChanged(BindableObject bindable, object oldValue, object newValue) { }
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(propertyName: "CommandParameter", returnType: typeof(object), declaringType: typeof(RummyDiceGraphicsXF), defaultBindingMode: BindingMode.TwoWay, propertyChanged: CommandParameterPropertyChanged);
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
        private static void CommandParameterPropertyChanged(BindableObject bindable, object oldValue, object newValue) { }
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
        public void SendDiceInfo(RummyDiceInfo thisDice)
        {
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
        private void Touch(object sender, SKTouchEventArgs e)
        {
            var tempCommand = Command;
            if (tempCommand != null)
            {
                if (tempCommand.CanExecute(CommandParameter) == true)
                    tempCommand.Execute(CommandParameter);
            }
        }
    }
}