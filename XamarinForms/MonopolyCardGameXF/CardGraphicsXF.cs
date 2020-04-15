using BasicGamingUIXFLibrary.GameGraphics.Base;
using Xamarin.Forms;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using MonopolyCardGameCP.Cards;
using MonopolyCardGameCP.Logic;

namespace MonopolyCardGameXF
{
    public class CardGraphicsXF : BaseDeckGraphicsXF<MonopolyCardGameCardInformation, MonopolyCardGameGraphicsCP>
    {
        public static readonly BindableProperty CardValueProperty = BindableProperty.Create(propertyName: "CardValue", returnType: typeof(int), declaringType: typeof(CardGraphicsXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: CardValuePropertyChanged);
        public int CardValue
        {
            get
            {
                return (int)GetValue(CardValueProperty);
            }
            set
            {
                SetValue(CardValueProperty, value);
            }
        }
        private static void CardValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.CardValue = (int)newValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(CardValueProperty, new Binding(nameof(MonopolyCardGameCardInformation.CardValue)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
    public class MonopolyHandXF : BaseHandXF<MonopolyCardGameCardInformation, MonopolyCardGameGraphicsCP, CardGraphicsXF>, IMonopolyScroll
    {
        async void IMonopolyScroll.ScrollToBottom() //iffy.
        {
            await ScrollToBottomAsync();
        }
    }
}