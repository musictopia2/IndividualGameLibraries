using BasicGamingUIXFLibrary.GameGraphics.Base;
using SorryCardGameCP.Cards;
using SorryCardGameCP.Data;
using Xamarin.Forms;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;

namespace SorryCardGameXF
{
    public class CardGraphicsXF : BaseDeckGraphicsXF<SorryCardGameCardInformation, SorryCardGameGraphicsCP>//begin
    {
        public static readonly BindableProperty CategoryProperty = BindableProperty.Create(propertyName: "Category", returnType: typeof(EnumCategory), declaringType: typeof(CardGraphicsXF), defaultValue: EnumCategory.Blank, defaultBindingMode: BindingMode.TwoWay, propertyChanged: CategoryPropertyChanged);
        public EnumCategory Category
        {
            get
            {
                return (EnumCategory)GetValue(CategoryProperty);
            }
            set
            {
                SetValue(CategoryProperty, value);
            }
        }
        private static void CategoryPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.Category = (EnumCategory)newValue;
        }
        public static readonly BindableProperty SorryProperty = BindableProperty.Create(propertyName: "Sorry", returnType: typeof(EnumSorry), declaringType: typeof(CardGraphicsXF), defaultValue: EnumSorry.Blank, defaultBindingMode: BindingMode.TwoWay, propertyChanged: SorryPropertyChanged);
        public EnumSorry Sorry
        {
            get
            {
                return (EnumSorry)GetValue(SorryProperty);
            }
            set
            {
                SetValue(SorryProperty, value);
            }
        }
        private static void SorryPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.Sorry = (EnumSorry)newValue;
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
        public static readonly BindableProperty ColorProperty = BindableProperty.Create(propertyName: "Color", returnType: typeof(string), declaringType: typeof(CardGraphicsXF), defaultValue: cs.Transparent, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ColorPropertyChanged);
        public string Color
        {
            get
            {
                return (string)GetValue(ColorProperty);
            }
            set
            {
                base.SetValue(ColorProperty, value);
            }
        }
        private static void ColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.Color = (string)newValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(CategoryProperty, new Binding(nameof(SorryCardGameCardInformation.Category)));
            SetBinding(SorryProperty, new Binding(nameof(SorryCardGameCardInformation.Sorry)));
            SetBinding(ValueProperty, new Binding(nameof(SorryCardGameCardInformation.Value)));
            SetBinding(ColorProperty, new Binding(nameof(SorryCardGameCardInformation.Color)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
}
