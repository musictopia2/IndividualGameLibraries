using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using BaseMahjongTilesCP;
using BasicGameFramework.GameGraphicsCP.Tiles;
using Xamarin.Forms;
namespace MahJongSolitaireXF
{
    public class MahJongSolitaireTilesXF : BaseDeckGraphicsXF<MahjongSolitaireTileInfo, MahjongTilesGraphicsCP>
    {
        public static readonly BindableProperty IndexProperty = BindableProperty.Create(propertyName: "Index", returnType: typeof(int), declaringType: typeof(MahJongSolitaireTilesXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: IndexPropertyChanged);
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
        private static void IndexPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (MahJongSolitaireTilesXF)bindable;
            thisItem.MainObject!.Index = (int)newValue;
        }

        public static readonly BindableProperty NeedsLeftProperty = BindableProperty.Create(propertyName: "NeedsLeft", returnType: typeof(bool), declaringType: typeof(MahJongSolitaireTilesXF), defaultValue: false, defaultBindingMode: BindingMode.TwoWay, propertyChanged: NeedsLeftPropertyChanged);
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
        private static void NeedsLeftPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (MahJongSolitaireTilesXF)bindable;
            if (thisItem._tileHelp == null)
                return;
            thisItem._tileHelp.NeedsLeft = (bool)newValue;
        }
        public static readonly BindableProperty NeedsRightProperty = BindableProperty.Create(propertyName: "NeedsRight", returnType: typeof(bool), declaringType: typeof(MahJongSolitaireTilesXF), defaultValue: false, defaultBindingMode: BindingMode.TwoWay, propertyChanged: NeedsRightPropertyChanged);
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
        private static void NeedsRightPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (MahJongSolitaireTilesXF)bindable;
            if (thisItem._tileHelp == null)
                return;
            thisItem._tileHelp.NeedsRight = (bool)newValue;
        }
        public static readonly BindableProperty NeedsTopProperty = BindableProperty.Create(propertyName: "NeedsTop", returnType: typeof(bool), declaringType: typeof(MahJongSolitaireTilesXF), defaultValue: false, defaultBindingMode: BindingMode.TwoWay, propertyChanged: NeedsTopPropertyChanged);
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
        private static void NeedsTopPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (MahJongSolitaireTilesXF)bindable;
            if (thisItem._tileHelp == null)
                return;
            thisItem._tileHelp.NeedsTop = (bool)newValue;
        }
        public static readonly BindableProperty NeedsBottomProperty = BindableProperty.Create(propertyName: "NeedsBottom", returnType: typeof(bool), declaringType: typeof(MahJongSolitaireTilesXF), defaultValue: false, defaultBindingMode: BindingMode.TwoWay, propertyChanged: NeedsBottomPropertyChanged);
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
        private static void NeedsBottomPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (MahJongSolitaireTilesXF)bindable;
            if (thisItem._tileHelp == null)
                return;
            thisItem._tileHelp.NeedsBottom = (bool)newValue;
        }
        public static readonly BindableProperty TopProperty = BindableProperty.Create(propertyName: "Top", returnType: typeof(double), declaringType: typeof(MahJongSolitaireTilesXF), defaultValue: 0.0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: TopPropertyChanged);
        public double Top
        {
            get
            {
                return (double)GetValue(TopProperty);
            }
            set
            {
                SetValue(TopProperty, value);
            }
        }
        private static void TopPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (MahJongSolitaireTilesXF)bindable;
            thisItem.Margin = new Thickness(thisItem.Left, (double)newValue, 0, 0);
        }
        public static readonly BindableProperty LeftProperty = BindableProperty.Create(propertyName: "Left", returnType: typeof(double), declaringType: typeof(MahJongSolitaireTilesXF), defaultValue: 0.0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: LeftPropertyChanged);
        public double Left
        {
            get
            {
                return (double)GetValue(LeftProperty);
            }
            set
            {
                SetValue(LeftProperty, value);
            }
        }
        private static void LeftPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (MahJongSolitaireTilesXF)bindable;
            thisItem.Margin = new Thickness((double)(newValue), thisItem.Top, 0, 0);
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