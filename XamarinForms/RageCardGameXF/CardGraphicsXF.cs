using BasicGamingUIXFLibrary.GameGraphics.Base;
using RageCardGameCP.Cards;
using RageCardGameCP.Data;
using Xamarin.Forms;

namespace RageCardGameXF
{
    public class CardGraphicsXF : BaseDeckGraphicsXF<RageCardGameCardInformation, RageCardGameGraphicsCP>//begin
    {
        public static readonly BindableProperty ColorProperty = BindableProperty.Create(propertyName: "Color", returnType: typeof(EnumColor), declaringType: typeof(CardGraphicsXF), defaultValue: EnumColor.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ColorPropertyChanged);
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
        public static readonly BindableProperty SpecialTypeProperty = BindableProperty.Create(propertyName: "SpecialType", returnType: typeof(EnumSpecialType), declaringType: typeof(CardGraphicsXF), defaultValue: EnumSpecialType.Blank, defaultBindingMode: BindingMode.TwoWay, propertyChanged: SpecialTypePropertyChanged);
        public EnumSpecialType SpecialType
        {
            get
            {
                return (EnumSpecialType)GetValue(SpecialTypeProperty);
            }
            set
            {
                SetValue(SpecialTypeProperty, value);
            }
        }
        private static void SpecialTypePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.SpecialType = (EnumSpecialType)newValue;
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
        protected override void DoCardBindings()
        {
            SetBinding(ColorProperty, new Binding(nameof(RageCardGameCardInformation.Color)));
            SetBinding(SpecialTypeProperty, new Binding(nameof(RageCardGameCardInformation.SpecialType)));
            SetBinding(ValueProperty, new Binding(nameof(RageCardGameCardInformation.Value)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
}
