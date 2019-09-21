using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using RackoCP;
using Xamarin.Forms;
namespace RackoXF
{
    public class CardGraphicsXF : BaseDeckGraphicsXF<RackoCardInformation, RackoGraphicsCP>
    {
        public static readonly BindableProperty ValueProperty = BindableProperty.Create(propertyName: "Value", returnType: typeof(int), declaringType: typeof(CardGraphicsXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ValuePropertyChanged);
        public int Value
        {
            get
            {
                return (int)GetValue(ValueProperty);
            }
            set
            {
                base.SetValue(ValueProperty, value);
            }
        }
        private static void ValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.Value = (int)newValue;
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
        protected override void DoCardBindings()
        {
            SetBinding(ValueProperty, new Binding(nameof(RackoCardInformation.Value)));
        }
    }
}