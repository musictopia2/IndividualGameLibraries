using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using Phase10CP;
using Xamarin.Forms;
namespace Phase10XF
{
    public class CardGraphicsXF : BaseColorCardsXF<Phase10CardInformation, Phase10GraphicsCP>
    {
        public static readonly BindableProperty CardCategoryProperty = BindableProperty.Create(propertyName: "CardCategory", returnType: typeof(EnumCardCategory), declaringType: typeof(CardGraphicsXF), defaultValue: EnumCardCategory.Blank, defaultBindingMode: BindingMode.TwoWay, propertyChanged: CardCategoryPropertyChanged);
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
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.CardCategory = (EnumCardCategory)newValue;
        }
        protected override void DoCardBindings()
        {
            base.DoCardBindings();
            SetBinding(CardCategoryProperty, new Binding(nameof(Phase10CardInformation.CardCategory)));
        }
    }
}