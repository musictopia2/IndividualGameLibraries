using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using CandylandCP;
using Xamarin.Forms;
namespace CandylandXF
{
    public class CardGraphicsXF : BaseDeckGraphicsXF<CandylandCardData, CandylandCardGraphicsCP>
    {
        public static readonly BindableProperty WhichTypeProperty = BindableProperty.Create(propertyName: "WhichType", returnType: typeof(EnumCandyLandType), declaringType: typeof(CardGraphicsXF), defaultValue: EnumCandyLandType.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: WhichTypePropertyChanged);
        public EnumCandyLandType WhichType
        {
            get
            {
                return (EnumCandyLandType)GetValue(WhichTypeProperty);
            }
            set
            {
                SetValue(WhichTypeProperty, value);
            }
        }
        private static void WhichTypePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.WhichCard = (EnumCandyLandType)newValue;
        }

        public static readonly BindableProperty HowManyProperty = BindableProperty.Create(propertyName: "HowMany", returnType: typeof(int), declaringType: typeof(CardGraphicsXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: HowManyPropertyChanged);
        public int HowMany
        {
            get
            {
                return (int)GetValue(HowManyProperty);
            }
            set
            {
                SetValue(HowManyProperty, value);
            }
        }
        private static void HowManyPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.HowMany = (int)newValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(WhichTypeProperty, new Binding(nameof(CandylandCardData.WhichCard)));
            SetBinding(HowManyProperty, new Binding(nameof(CandylandCardData.HowMany)));
        }
        protected override void PopulateInitObject() //this is needed too.
        {
            MainObject!.Init();
        }
    }
}