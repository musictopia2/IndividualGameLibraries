using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using HitTheDeckCP;
using Xamarin.Forms;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace HitTheDeckXF
{
    public class CardGraphicsXF : BaseDeckGraphicsXF<HitTheDeckCardInformation, HitTheDeckGraphicsCP>
    {
        public static readonly BindableProperty CardColorProperty = BindableProperty.Create(propertyName: "CardColor", returnType: typeof(string), declaringType: typeof(CardGraphicsXF), defaultValue: cs.White, defaultBindingMode: BindingMode.TwoWay, propertyChanged: CardColorPropertyChanged);
        public string CardColor
        {
            get
            {
                return (string)GetValue(CardColorProperty);
            }
            set
            {
                SetValue(CardColorProperty, value);
            }
        }
        private static void CardColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.MainGraphics!.BackgroundColor = (string)newValue;
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
        public static readonly BindableProperty CardTypeProperty = BindableProperty.Create(propertyName: "CardType", returnType: typeof(EnumTypeList), declaringType: typeof(CardGraphicsXF), defaultValue: EnumTypeList.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: CardTypePropertyChanged);
        public EnumTypeList CardType
        {
            get
            {
                return (EnumTypeList)GetValue(CardTypeProperty);
            }
            set
            {
                SetValue(CardTypeProperty, value);
            }
        }
        private static void CardTypePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.CardType = (EnumTypeList)newValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(CardColorProperty, new Binding(nameof(HitTheDeckCardInformation.Color)));
            SetBinding(ValueProperty, new Binding(nameof(HitTheDeckCardInformation.Number))); // i think
            SetBinding(CardTypeProperty, new Binding(nameof(HitTheDeckCardInformation.CardType)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
}