using BasicGamingUIXFLibrary.GameGraphics.Base;
using CommonBasicStandardLibraries.CollectionClasses;
using SkiaSharp;
using System.Windows.Input;
using ThreeLetterFunCP.Data;
using ThreeLetterFunCP.GraphicsCP;
using Xamarin.Forms;

namespace ThreeLetterFunXF
{
    public class CardGraphicsXF : BaseDeckGraphicsXF<ThreeLetterFunCardData, ThreeLetterFunCardGraphicsCP>
    {
        public static readonly BindableProperty HiddenValueProperty = BindableProperty.Create(propertyName: "HiddenValue", returnType: typeof(int), declaringType: typeof(CardGraphicsXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: HiddenValuePropertyChanged);
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
        private static void HiddenValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.HiddenValue = (int)newValue;
        }
        public static readonly BindableProperty CharListProperty = BindableProperty.Create(propertyName: "CharList", returnType: typeof(CustomBasicList<char>), declaringType: typeof(CardGraphicsXF), defaultValue: new CustomBasicList<char>(), defaultBindingMode: BindingMode.TwoWay, propertyChanged: CharListPropertyChanged);
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
        private static void CharListPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.CharList = (CustomBasicList<char>)newValue;
        }
        public static readonly BindableProperty TilesProperty = BindableProperty.Create(propertyName: "Tiles", returnType: typeof(CustomBasicList<TileInformation>), declaringType: typeof(CardGraphicsXF), defaultValue: new CustomBasicList<TileInformation>(), defaultBindingMode: BindingMode.TwoWay, propertyChanged: TilesPropertyChanged);
        public CustomBasicList<TileInformation> Tiles
        {
            get
            {
                return (CustomBasicList<TileInformation>)GetValue(CharListProperty);
            }
            set
            {
                SetValue(TilesProperty, value);
            }
        }
        private static void TilesPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.Tiles = (CustomBasicList<TileInformation>)newValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(HiddenValueProperty, new Binding(nameof(ThreeLetterFunCardData.HiddenValue)));
            SetBinding(CharListProperty, new Binding(nameof(ThreeLetterFunCardData.CharList)));
            SetBinding(TilesProperty, new Binding(nameof(ThreeLetterFunCardData.Tiles)));
        }
        //could be iffy.  if worse comes to worst, could cause problems on this game (?)
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