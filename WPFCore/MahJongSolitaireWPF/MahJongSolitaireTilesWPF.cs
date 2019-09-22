using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using BaseMahjongTilesCP;
using BasicGameFramework.GameGraphicsCP.Tiles;
using System.Windows;
using System.Windows.Data;
namespace MahJongSolitaireWPF
{
    public class MahJongSolitaireTilesWPF : BaseDeckGraphicsWPF<MahjongSolitaireTileInfo, MahjongTilesGraphicsCP>
    {
        public static readonly DependencyProperty IndexProperty = DependencyProperty.Register("Index", typeof(int), typeof(MahJongSolitaireTilesWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(IndexPropertyChanged)));
        public int Index
        {
            get
            {
                return (int)GetValue(IndexProperty);
            }
            set
            {
                SetValue(IndexProperty, value);
            }
        }
        private static void IndexPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (MahJongSolitaireTilesWPF)sender;
            // code to change the control values
            thisItem.MainObject!.Index = (int)e.NewValue;
        }
        private Base3DTilesCP? _tileHelp;
        protected override void PopulateInitObject()
        {
            _tileHelp = new Base3DTilesCP();
            _tileHelp.PaintUI = this;
            MainObject!.Is3D = true;
            _tileHelp.Is3D = true;
            _tileHelp.ThisGraphics = Mains;
            MainObject.TileGraphics = _tileHelp;
            MainObject.Init();
        }
        public static readonly DependencyProperty NeedsLeftProperty = DependencyProperty.Register("NeedsLeft", typeof(bool), typeof(MahJongSolitaireTilesWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(NeedsLeftPropertyChanged)));
        public bool NeedsLeft
        {
            get
            {
                return (bool)GetValue(NeedsLeftProperty);
            }
            set
            {
                SetValue(NeedsLeftProperty, value);
            }
        }
        private static void NeedsLeftPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (MahJongSolitaireTilesWPF)sender;
            thisItem._tileHelp!.NeedsLeft = (bool)e.NewValue;
        }
        public static readonly DependencyProperty NeedsRightProperty = DependencyProperty.Register("NeedsRight", typeof(bool), typeof(MahJongSolitaireTilesWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(NeedsRightPropertyChanged)));
        public bool NeedsRight
        {
            get
            {
                return (bool)GetValue(NeedsRightProperty);
            }
            set
            {
                SetValue(NeedsRightProperty, value);
            }
        }
        private static void NeedsRightPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (MahJongSolitaireTilesWPF)sender;
            thisItem._tileHelp!.NeedsRight = (bool)e.NewValue;
        }
        public static readonly DependencyProperty NeedsTopProperty = DependencyProperty.Register("NeedsTop", typeof(bool), typeof(MahJongSolitaireTilesWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(NeedsTopPropertyChanged)));
        public bool NeedsTop
        {
            get
            {
                return (bool)GetValue(NeedsTopProperty);
            }
            set
            {
                SetValue(NeedsTopProperty, value);
            }
        }
        private static void NeedsTopPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (MahJongSolitaireTilesWPF)sender;
            thisItem._tileHelp!.NeedsTop = (bool)e.NewValue;
        }
        public static readonly DependencyProperty NeedsBottomProperty = DependencyProperty.Register("NeedsBottom", typeof(bool), typeof(MahJongSolitaireTilesWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(NeedsBottomPropertyChanged)));
        public bool NeedsBottom
        {
            get
            {
                return (bool)GetValue(NeedsBottomProperty);
            }
            set
            {
                SetValue(NeedsBottomProperty, value);
            }
        }
        private static void NeedsBottomPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (MahJongSolitaireTilesWPF)sender;
            thisItem._tileHelp!.NeedsBottom = (bool)e.NewValue;
        }
        public static readonly DependencyProperty TopProperty = DependencyProperty.Register("Top", typeof(float), typeof(MahJongSolitaireTilesWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(TopPropertyChanged)));
        public float Top
        {
            get
            {
                return (float)GetValue(TopProperty);
            }
            set
            {
                SetValue(TopProperty, value);
            }
        }
        private static void TopPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (MahJongSolitaireTilesWPF)sender;
            thisItem.Margin = new Thickness(thisItem.Left, (float)e.NewValue, 0, 0);
        }
        public static readonly DependencyProperty LeftProperty = DependencyProperty.Register("Left", typeof(float), typeof(MahJongSolitaireTilesWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(LeftPropertyChanged)));
        public float Left
        {
            get
            {
                return (float)GetValue(LeftProperty);
            }
            set
            {
                SetValue(LeftProperty, value);
            }
        }
        private static void LeftPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (MahJongSolitaireTilesWPF)sender;
            // code to change the control values
            thisItem.Margin = new Thickness((float)e.NewValue, thisItem.Top, 0, 0);
        }
        protected override void DoCardBindings()
        {
            SetBinding(IndexProperty, new Binding(nameof(MahjongSolitaireTileInfo.Index)));
            SetBinding(TopProperty, new Binding(nameof(MahjongSolitaireTileInfo.Top))); //one of them
            SetBinding(LeftProperty, new Binding(nameof(MahjongSolitaireTileInfo.Left))); //another one.
            SetBinding(NeedsBottomProperty, new Binding(nameof(MahjongSolitaireTileInfo.NeedsBottom)));
            SetBinding(NeedsLeftProperty, new Binding(nameof(MahjongSolitaireTileInfo.NeedsLeft)));
            SetBinding(NeedsRightProperty, new Binding(nameof(MahjongSolitaireTileInfo.NeedsRight)));
            SetBinding(NeedsTopProperty, new Binding(nameof(MahjongSolitaireTileInfo.NeedsTop)));
        }
    }
}