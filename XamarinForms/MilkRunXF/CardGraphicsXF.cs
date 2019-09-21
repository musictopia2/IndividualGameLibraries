using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using MilkRunCP;
using Xamarin.Forms;
namespace MilkRunXF
{
    public class CardGraphicsXF : BaseDeckGraphicsXF<MilkRunCardInformation, MilkRunGraphicsCP>
    {
        public static readonly BindableProperty PointsProperty = BindableProperty.Create(propertyName: "Points", returnType: typeof(int), declaringType: typeof(CardGraphicsXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: PointsPropertyChanged);
        public int Points
        {
            get
            {
                return (int)GetValue(PointsProperty);
            }
            set
            {
                SetValue(PointsProperty, value);
            }
        }
        private static void PointsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.Points = (int)newValue;
        }
        public static readonly BindableProperty MilkTypeProperty = BindableProperty.Create(propertyName: "MilkType", returnType: typeof(EnumMilkType), declaringType: typeof(CardGraphicsXF), defaultValue: EnumMilkType.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: MilkTypePropertyChanged);
        public EnumMilkType MilkType
        {
            get
            {
                return (EnumMilkType)GetValue(MilkTypeProperty);
            }
            set
            {
                SetValue(MilkTypeProperty, value);
            }
        }
        private static void MilkTypePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.MilkType = (EnumMilkType)newValue;
        }
        public static readonly BindableProperty CardCategoryProperty = BindableProperty.Create(propertyName: "CardCategory", returnType: typeof(EnumCardCategory), declaringType: typeof(CardGraphicsXF), defaultValue: EnumCardCategory.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: CardCategoryPropertyChanged);
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
            SetBinding(PointsProperty, new Binding(nameof(MilkRunCardInformation.Points)));
            SetBinding(MilkTypeProperty, new Binding(nameof(MilkRunCardInformation.MilkCategory)));
            SetBinding(CardCategoryProperty, new Binding(nameof(MilkRunCardInformation.CardCategory)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
}