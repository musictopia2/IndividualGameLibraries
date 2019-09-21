using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using MonopolyCardGameCP;
using Xamarin.Forms;
using static AndyCristinaGamePackageCP.DataClasses.GlobalStaticClass;
using AndyCristinaGamePackageCP.DataClasses;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
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
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.LargeTablet)
                    return 1.2f;
                return 0.95f; //experiment.
            }
        }
    }
}