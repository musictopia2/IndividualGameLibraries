using BasicGamingUIXFLibrary.GameGraphics.Base;
using Xamarin.Forms;
using YahtzeeHandsDownCP.Cards;
using YahtzeeHandsDownCP.Data;

namespace YahtzeeHandsDownXF
{
    public class CardGraphicsXF : BaseDeckGraphicsXF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP>
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
        public static readonly BindableProperty FirstValueProperty = BindableProperty.Create(propertyName: "FirstValue", returnType: typeof(int), declaringType: typeof(CardGraphicsXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: FirstValuePropertyChanged);
        public int FirstValue
        {
            get
            {
                return (int)GetValue(FirstValueProperty);
            }
            set
            {
                SetValue(FirstValueProperty, FirstValue);
            }
        }
        private static void FirstValuePropertyChanged(BindableObject bindable, object oldFirstValue, object newFirstValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.FirstValue = (int)newFirstValue;
        }
        public static readonly BindableProperty SecondValueProperty = BindableProperty.Create(propertyName: "SecondValue", returnType: typeof(int), declaringType: typeof(CardGraphicsXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: SecondValuePropertyChanged);
        public int SecondValue
        {
            get
            {
                return (int)GetValue(SecondValueProperty);
            }
            set
            {
                SetValue(SecondValueProperty, SecondValue);
            }
        }
        private static void SecondValuePropertyChanged(BindableObject bindable, object oldSecondValue, object newSecondValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.SecondValue = (int)newSecondValue;
        }
        public static readonly BindableProperty IsWildProperty = BindableProperty.Create(propertyName: "IsWild", returnType: typeof(bool), declaringType: typeof(CardGraphicsXF), defaultValue: false, defaultBindingMode: BindingMode.TwoWay, propertyChanged: IsWildPropertyChanged);
        public bool IsWild
        {
            get
            {
                return (bool)GetValue(IsWildProperty);
            }
            set
            {
                SetValue(IsWildProperty, IsWild);
            }
        }
        private static void IsWildPropertyChanged(BindableObject bindable, object oldIsWild, object newIsWild)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.IsWild = (bool)newIsWild;
        }
        protected override void DoCardBindings()
        {
            SetBinding(ColorProperty, new Binding(nameof(YahtzeeHandsDownCardInformation.Color)));
            SetBinding(FirstValueProperty, new Binding(nameof(YahtzeeHandsDownCardInformation.FirstValue)));
            SetBinding(SecondValueProperty, new Binding(nameof(YahtzeeHandsDownCardInformation.SecondValue)));
            SetBinding(IsWildProperty, new Binding(nameof(YahtzeeHandsDownCardInformation.IsWild)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }

}