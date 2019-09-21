using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using ClueBoardGameCP;
using Xamarin.Forms;
namespace ClueBoardGameXF
{
    public class CardXF : BaseDeckGraphicsXF<CardInfo, CardCP>
    {
        public static readonly BindableProperty CardValueProperty = BindableProperty.Create(propertyName: "CardValue", returnType: typeof(EnumCardValues), declaringType: typeof(CardXF), defaultValue: EnumCardValues.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: CardValuePropertyChanged);
        public EnumCardValues CardValue
        {
            get
            {
                return (EnumCardValues)GetValue(CardValueProperty);
            }
            set
            {
                SetValue(CardValueProperty, value);
            }
        }
        private static void CardValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardXF)bindable;
            thisItem.MainObject!.CardValue = (EnumCardValues)newValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(CardValueProperty, new Binding(nameof(CardInfo.CardValue)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
}
