using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using FillOrBustCP;
using Xamarin.Forms;
namespace FillOrBustXF
{
    public class CardGraphicsXF : BaseDeckGraphicsXF<FillOrBustCardInformation, FillOrBustGraphicsCP>
    {
        public static readonly BindableProperty CategoryProperty = BindableProperty.Create(propertyName: "Category", returnType: typeof(EnumCardStatusList), declaringType: typeof(CardGraphicsXF), defaultValue: EnumCardStatusList.Unknown, defaultBindingMode: BindingMode.TwoWay, propertyChanged: CategoryPropertyChanged);
        public EnumCardStatusList Category
        {
            get
            {
                return (EnumCardStatusList)GetValue(CategoryProperty);
            }
            set
            {
                SetValue(CategoryProperty, value);
            }
        }
        private static void CategoryPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.Category = (EnumCardStatusList)newValue;
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
            thisItem.MainObject!.Value = (int) newValue;
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
        protected override void DoCardBindings()
        {
            SetBinding(CategoryProperty, new Binding(nameof(FillOrBustCardInformation.Status)));
            SetBinding(ValueProperty, new Binding(nameof(FillOrBustCardInformation.Value)));
        }
    }
}
