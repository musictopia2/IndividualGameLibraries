using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using PaydayCP;
using Xamarin.Forms;
namespace PaydayXF
{
    public abstract class PaydayBaseCardXF<CA, CP> : BaseDeckGraphicsXF<CA, CP>
        where CA : CardInformation, new()
        where CP : CardGraphicsCP, new()
    {
        public static readonly BindableProperty CardCategoryProperty = BindableProperty.Create(propertyName: "CardCategory", returnType: typeof(EnumCardCategory), declaringType: typeof(PaydayBaseCardXF<CA, CardGraphicsCP>), defaultValue: EnumCardCategory.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: CardCategoryPropertyChanged);

        public EnumCardCategory CardCategory
        {
            get
            {
                return (EnumCardCategory)GetValue(CardCategoryProperty);
            }
            set
            {
                SetValue(CardCategoryProperty, value);
            }
        }
        private static void CardCategoryPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (PaydayBaseCardXF<CA, CardGraphicsCP>)bindable;
            thisItem.MainObject!.CardCategory = (EnumCardCategory)newValue;
        }

        public static readonly BindableProperty IndexProperty = BindableProperty.Create(propertyName: "Index", returnType: typeof(int), declaringType: typeof(PaydayBaseCardXF<CA, CardGraphicsCP>), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: IndexPropertyChanged);

        public int Index
        {
            get
            {
                return (int)GetValue(IndexProperty);
            }
            set
            {
                SetValue(IndexProperty, value);
            }
        }
        private static void IndexPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (PaydayBaseCardXF<CA, CardGraphicsCP>)bindable;
            thisItem.MainObject!.Index = (int)newValue;
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
        protected override void DoCardBindings()
        {
            SetBinding(CardCategoryProperty, new Binding(nameof(CardInformation.CardCategory)));
            SetBinding(IndexProperty, new Binding(nameof(CardInformation.Index)));
        }
    }
    public class MailCardXF : PaydayBaseCardXF<MailCard, CardGraphicsCP> { }
    public class DealCardXF : PaydayBaseCardXF<DealCard, CardGraphicsCP> { }
}