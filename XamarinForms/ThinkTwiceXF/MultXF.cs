using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using SkiaSharpGeneralLibrary.Interfaces;
using ThinkTwiceCP.Data;
using ThinkTwiceCP.Logic;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace ThinkTwiceXF
{
    public class MultXF : ContentView, IRepaintControl
    {
        private ButtonDiceGraphicsCP? _mains;
        private readonly SKCanvasView _thisDraw;
        public static string GetDiceTag => "StandardDice"; //same as other dice.
        public MultXF()
        {
            _thisDraw = new SKCanvasView();
            _thisDraw.PaintSurface += PaintSurface;
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Start;
            Content = _thisDraw;
        }
        public void SendDiceInfo(Multiplier thisDice) //it did send dice
        {
            IGamePackageResolver thisR = (IGamePackageResolver)cons!;
            IProportionImage thisP = thisR.Resolve<IProportionImage>(GetDiceTag);
            _mains = new ButtonDiceGraphicsCP();
            _mains.PaintUI = this;
            _mains.MinimumWidthHeight = thisDice.HeightWidth;
            SetBinding(TextProperty, new Binding(nameof(Multiplier.Value)));
            SKSize tempSize = new SKSize(thisDice.HeightWidth, thisDice.HeightWidth);
            DiceSize = tempSize.GetSizeUsed(thisP.Proportion * 1.3f);
            HeightRequest = DiceSize.Height;
            WidthRequest = DiceSize.Width;
            BindingContext = thisDice;
            Init();
        }
        private void BeforePainting()
        {
            var thisDice = (Multiplier)BindingContext;
            _mains!.MinimumWidthHeight = thisDice.HeightWidth;
        }
        private void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (_mains == null)
                return;
            BeforePainting();
            _mains.DrawDice(e.Surface.Canvas);
        }
        private void Init() // the actual is not working at this moment until the property change happens
        {
            double ThisD = WidthRequest - Margin.Left - Margin.Right;
            _mains!.ActualWidthHeight = (float)ThisD;
            _thisDraw.InvalidateSurface();
        }
        public void DoInvalidate()
        {
            _thisDraw.InvalidateSurface();
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(propertyName: "Text", returnType: typeof(string), declaringType: typeof(MultXF), defaultBindingMode: BindingMode.TwoWay, propertyChanged: TextPropertyChanged);

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
        private static void TextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (MultXF)bindable;
            thisItem._mains!.Text = (string)newValue;
        }
        public SKSize DiceSize { get; set; } //try to do without bindings for this.  taking a risk.  hopefully it pays off.
    }
}
