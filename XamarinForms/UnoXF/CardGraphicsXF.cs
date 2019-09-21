using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using UnoCP;
using Xamarin.Forms;
namespace UnoXF
{
    public class CardGraphicsXF : BaseColorCardsXF<UnoCardInformation, UnoGraphicsCP>
    {
        public static readonly BindableProperty WhichTypeProperty = BindableProperty.Create(propertyName: "WhichType", returnType: typeof(EnumCardTypeList), declaringType: typeof(CardGraphicsXF), defaultValue: EnumCardTypeList.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: WhichTypePropertyChanged);
        public EnumCardTypeList WhichType
        {
            get
            {
                return (EnumCardTypeList)GetValue(WhichTypeProperty);
            }
            set
            {
                SetValue(WhichTypeProperty, value);
            }
        }
        private static void WhichTypePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.WhichType = (EnumCardTypeList)newValue;
        }
        public static readonly BindableProperty DrawProperty = BindableProperty.Create(propertyName: "Draw", returnType: typeof(int), declaringType: typeof(CardGraphicsXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: DrawPropertyChanged);
        public int Draw
        {
            get
            {
                return (int)GetValue(DrawProperty);
            }
            set
            {
                SetValue(DrawProperty, value);
            }
        }
        private static void DrawPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.Draw = (int)newValue;
        }
        public static readonly BindableProperty NumberProperty = BindableProperty.Create(propertyName: "Number", returnType: typeof(int), declaringType: typeof(CardGraphicsXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: NumberPropertyChanged);
        public int Number
        {
            get
            {
                return (int)GetValue(NumberProperty);
            }
            set
            {
                SetValue(NumberProperty, value);
            }
        }
        private static void NumberPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.Number = (int)newValue;
        }
        protected override void DoCardBindings()
        {
            base.DoCardBindings(); // must have this line of code.  otherwise, the base class will not do their bindings.
            SetBinding(WhichTypeProperty, new Binding(nameof(UnoCardInformation.WhichType)));
            SetBinding(DrawProperty, new Binding(nameof(UnoCardInformation.Draw)));
            SetBinding(NumberProperty, new Binding(nameof(UnoCardInformation.Number)));
        }
    }
}