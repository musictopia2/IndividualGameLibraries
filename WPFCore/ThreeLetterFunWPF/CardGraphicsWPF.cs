using BasicGamingUIWPFLibrary.GameGraphics.Base;
using CommonBasicStandardLibraries.CollectionClasses;
using SkiaSharp;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ThreeLetterFunCP.Data;
using ThreeLetterFunCP.GraphicsCP;
namespace ThreeLetterFunWPF
{
    public class CardGraphicsWPF : BaseDeckGraphicsWPF<ThreeLetterFunCardData, ThreeLetterFunCardGraphicsCP>
    {
        public static readonly DependencyProperty HiddenValueProperty = DependencyProperty.Register("HiddenValue", typeof(int), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(HiddenValuePropertyChanged)));
        public int HiddenValue
        {
            get
            {
                return (int)GetValue(HiddenValueProperty);
            }
            set
            {
                SetValue(HiddenValueProperty, value);
            }
        }
        private static void HiddenValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.HiddenValue = (int)e.NewValue;
        }
        public static readonly DependencyProperty CharListProperty = DependencyProperty.Register("CharList", typeof(CustomBasicList<char>), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(CharListPropertyChanged)));
        public CustomBasicList<char> CharList
        {
            get
            {
                return (CustomBasicList<char>)GetValue(CharListProperty);
            }
            set
            {
                SetValue(CharListProperty, value);
            }
        }
        private static void CharListPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.CharList = (CustomBasicList<char>)e.NewValue;
        }
        public static readonly DependencyProperty TilesProperty = DependencyProperty.Register("Tiles", typeof(CustomBasicList<TileInformation>), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(TilesPropertyChanged)));
        public CustomBasicList<TileInformation> Tiles
        {
            get
            {
                return (CustomBasicList<TileInformation>)GetValue(TilesProperty);
            }
            set
            {
                SetValue(TilesProperty, value);
            }
        }
        private static void TilesPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.Tiles = (CustomBasicList<TileInformation>)e.NewValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(HiddenValueProperty, new Binding(nameof(ThreeLetterFunCardData.HiddenValue)));
            SetBinding(CharListProperty, new Binding(nameof(ThreeLetterFunCardData.CharList)));
            SetBinding(TilesProperty, new Binding(nameof(ThreeLetterFunCardData.Tiles)));
        }
        protected override void BeforeProcessClick(ICommand thisCommand, object thisParameter, SKPoint clickPoint)
        {
            var thisValue = MainObject!.GetClickLocation(clickPoint);
            var thisCard = (ThreeLetterFunCardData)thisParameter;
            thisCard.ClickLocation = thisValue; // i think this simple.
        }
        protected override void PopulateInitObject() //this is needed too.
        {
            MainObject!.Init();
        }
    }
}
