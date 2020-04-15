using BasicGamingUIXFLibrary.GameGraphics.Base;
using MillebournesCP.Cards;
using MillebournesCP.Data;
using Xamarin.Forms;

namespace MillebournesXF
{
    public class CardGraphicsXF : BaseDeckGraphicsXF<MillebournesCardInformation, MillebournesGraphicsCP>//begin
    {
        public static readonly BindableProperty CategoryProperty = BindableProperty.Create(propertyName: "Category", returnType: typeof(EnumCompleteCategories), declaringType: typeof(CardGraphicsXF), defaultValue: EnumCompleteCategories.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: CategoryPropertyChanged);
        public EnumCompleteCategories Category
        {
            get
            {
                return (EnumCompleteCategories)GetValue(CategoryProperty);
            }
            set
            {
                SetValue(CategoryProperty, value);
            }
        }
        private static void CategoryPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.Category = (EnumCompleteCategories)newValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(CategoryProperty, new Binding(nameof(MillebournesCardInformation.CompleteCategory)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
}
