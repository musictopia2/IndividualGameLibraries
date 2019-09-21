using A8RoundRummyCP;
using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using Xamarin.Forms;
namespace A8RoundRummyXF
{
    public class CardGraphicsXF : BaseDeckGraphicsXF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP>
    {
        public static readonly BindableProperty CardTypeProperty = BindableProperty.Create(propertyName: "CardType", returnType: typeof(EnumCardType), declaringType: typeof(CardGraphicsXF), defaultValue: EnumCardType.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: CardTypePropertyChanged);
        public EnumCardType CardType
        {
            get
            {
                return (EnumCardType)GetValue(CardTypeProperty);
            }
            set
            {
                SetValue(CardTypeProperty, value);
            }
        }
        private static void CardTypePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.CardType = (EnumCardType)newValue;
        }
        public static readonly BindableProperty ValueProperty = BindableProperty.Create(propertyName: "Value", returnType: typeof(int), declaringType: typeof(CardGraphicsXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ValuePropertyChanged);
        public int Value
        {
            get
            {
                return (int)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }
        private static void ValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.Value = (int)newValue;
        }

        public static readonly BindableProperty ShapeProperty = BindableProperty.Create(propertyName: "Shape", returnType: typeof(EnumCardShape), declaringType: typeof(CardGraphicsXF), defaultValue: EnumCardShape.Blank, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ShapePropertyChanged);

        public EnumCardShape Shape
        {
            get
            {
                return (EnumCardShape)GetValue(ShapeProperty);
            }
            set
            {
                SetValue(ShapeProperty, value);
            }
        }
        private static void ShapePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.Shape = (EnumCardShape)newValue;
        }
        public static readonly BindableProperty ColorProperty = BindableProperty.Create(propertyName: "Color", returnType: typeof(EnumColor), declaringType: typeof(CardGraphicsXF), defaultValue: EnumColor.Blank, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ColorPropertyChanged);
        public EnumColor Color
        {
            get
            {
                return (EnumColor)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }
        private static void ColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.Color = (EnumColor)newValue;
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
        protected override void DoCardBindings()
        {
            SetBinding(CardTypeProperty, new Binding(nameof(A8RoundRummyCardInformation.CardType)));
            SetBinding(ValueProperty, new Binding(nameof(A8RoundRummyCardInformation.Value)));
            SetBinding(ShapeProperty, new Binding(nameof(A8RoundRummyCardInformation.Shape)));
            SetBinding(ColorProperty, new Binding(nameof(A8RoundRummyCardInformation.Color)));
        }
    }
}