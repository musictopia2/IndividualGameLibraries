using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using SkiaSharpGeneralLibrary.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ThinkTwiceCP.Data;
using ThinkTwiceCP.Logic;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

//i think this is the most common things i like to do
namespace ThinkTwiceWPF
{
    public class CategoryWPF : UserControl, IRepaintControl
    {
        private ButtonDiceGraphicsCP? _mains;
        private readonly SKElement _thisDraw;
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(CategoryWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(TextPropertyChanged)));
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
            var thisItem = (CategoryWPF)sender;
            thisItem._mains!.Text = e.NewValue.ToString()!;
        }
        public static readonly DependencyProperty DiceSizeProperty = DependencyProperty.Register("DiceSize", typeof(SKSize), typeof(CategoryWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(DiceSizePropertyChanged)));
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
        public bool Hold
        {
            get { return (bool)GetValue(HoldProperty); }
            set { SetValue(HoldProperty, value); }
        }
        public static readonly DependencyProperty HoldProperty =
            DependencyProperty.Register("Hold", typeof(bool), typeof(CategoryWPF),
                 new FrameworkPropertyMetadata(new PropertyChangedCallback(HoldPropertyChanged)));
        private static void HoldPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CategoryWPF)sender;
            thisItem._mains!.WillHold = (bool)e.NewValue; // hopefully will be that simple  this will trigger the paint event.  hoepfully this way will still work out.
        }
        private static void DiceSizePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) { }
        private ThinkTwiceGameContainer? _gameContainer;
        public static string GetDiceTag => "StandardDice"; //same as other dice.
        //private ICommand? Command;
        public void SendDiceInfo(CategoriesDice thisDice, ThinkTwiceGameContainer gameContainer) //it did send dice
        {
            IGamePackageResolver thisR = (IGamePackageResolver)cons!;
            IProportionImage thisP = thisR.Resolve<IProportionImage>(GetDiceTag);
            _gameContainer = gameContainer;
            _mains = new ButtonDiceGraphicsCP();
            _mains.PaintUI = this;
            _mains.MinimumWidthHeight = thisDice.HeightWidth;
            SetBinding(TextProperty, nameof(CategoriesDice.Value));
            SetBinding(HoldProperty, nameof(CategoriesDice.Hold));
            SetBinding(VisibilityProperty, GetVisibleBinding(nameof(CategoriesDice.Visible)));
            SKSize TempSize = new SKSize(thisDice.HeightWidth, thisDice.HeightWidth);
            DiceSize = TempSize.GetSizeUsed(thisP.Proportion);
            //Command = thisDice.CategoryClickCommand!;
            Height = DiceSize.Height;
            Width = DiceSize.Width;
            DataContext = thisDice;
            Init();
        }
        private void BeforePainting()
        {
            var thisDice = (CategoriesDice)DataContext;
            _mains!.MinimumWidthHeight = thisDice.HeightWidth;
        }
        public CategoryWPF()
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
        private async void BaseGraphics_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var dice = (CategoriesDice)DataContext;
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
    }
}