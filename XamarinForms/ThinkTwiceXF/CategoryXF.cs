using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using SkiaSharpGeneralLibrary.Interfaces;
using ThinkTwiceCP.Data;
using ThinkTwiceCP.Logic;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace ThinkTwiceXF
{
    public class CategoryXF : ContentView, IRepaintControl
    {
        private ButtonDiceGraphicsCP? _mains;
        private readonly SKCanvasView _thisDraw;
        public CategoryXF()
        {
            _thisDraw = new SKCanvasView();
            _thisDraw.EnableTouchEvents = true;
            _thisDraw.Touch += DrawTouch;
            _thisDraw.PaintSurface += PaintSurface;
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Start;
            Content = _thisDraw;
        }
        public void DoInvalidate()
        {
            _thisDraw.InvalidateSurface();
        }
        private void BeforePainting()
        {
            var thisDice = (CategoriesDice)BindingContext;
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
            //double ThisD = WidthRequest - Margin.Left - Margin.Right;
            _mains!.ActualWidthHeight = DiceSize.Height;
            _thisDraw.InvalidateSurface();
        }
        public SKSize DiceSize { get; set; } //try to do without bindings for this.  taking a risk.  hopefully it pays off.
        public static string GetDiceTag => "StandardDice"; //same as other dice.
        private ThinkTwiceGameContainer? _gameContainer;
        public void SendDiceInfo(CategoriesDice thisDice, ThinkTwiceGameContainer gameContainer) //it did send dice
        {
            _gameContainer = gameContainer;
            IGamePackageResolver thisR = (IGamePackageResolver)cons!;
            IProportionImage thisP = thisR.Resolve<IProportionImage>(GetDiceTag);
            _mains = new ButtonDiceGraphicsCP();
            _mains.PaintUI = this;
            _mains.MinimumWidthHeight = thisDice.HeightWidth;
            SetBinding(TextProperty, new Binding(nameof(CategoriesDice.Value)));
            SetBinding(HoldProperty, new Binding(nameof(CategoriesDice.Hold)));
            SetBinding(IsVisibleProperty, new Binding(nameof(CategoriesDice.Visible)));
            SKSize tempSize = new SKSize(thisDice.HeightWidth, thisDice.HeightWidth);
            DiceSize = tempSize.GetSizeUsed(thisP.Proportion * 1.3f);
            HeightRequest = DiceSize.Height;
            WidthRequest = DiceSize.Width;
            BindingContext = thisDice;
            Init();
        }
        private async void DrawTouch(object sender, SKTouchEventArgs e)
        {

            var dice = (CategoriesDice)BindingContext;
            if (dice.Visible == false)
            {
                return;
            }
            if (_gameContainer!.Command.IsExecuting)
            {
                return;
            }
            if (_gameContainer.CategoryClicked == null)
            {
                throw new BasicBlankException("Nobody is handling the game category clicked.  Rethink");
            }
            await _gameContainer.ProcessCustomCommandAsync(_gameContainer.CategoryClicked);
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(propertyName: "Text", returnType: typeof(string), declaringType: typeof(CategoryXF), defaultBindingMode: BindingMode.TwoWay, propertyChanged: TextPropertyChanged);

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
            var thisItem = (CategoryXF)bindable;
            thisItem._mains!.Text = (string)newValue;
        }
        public static readonly BindableProperty HoldProperty = BindableProperty.Create(propertyName: "Hold", returnType: typeof(bool), declaringType: typeof(CategoryXF), defaultValue: false, defaultBindingMode: BindingMode.TwoWay, propertyChanged: HoldPropertyChanged);
        public bool Hold
        {
            get
            {
                return (bool)GetValue(HoldProperty);
            }
            set
            {
                SetValue(HoldProperty, value);
            }
        }
        private static void HoldPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CategoryXF)bindable;
            thisItem._mains!.WillHold = (bool)newValue;
        }
    }
}