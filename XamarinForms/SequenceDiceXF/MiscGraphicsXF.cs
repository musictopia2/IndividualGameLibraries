using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using SequenceDiceCP;
using Xamarin.Forms;
namespace SequenceDiceXF
{
    public class MiscGraphicsXF : BaseGraphicsXF<MiscGraphicsCP>
    {
        public static readonly BindableProperty WasPreviousProperty = BindableProperty.Create(propertyName: "WasPrevious", returnType: typeof(bool), declaringType: typeof(MiscGraphicsXF), defaultValue: false, defaultBindingMode: BindingMode.TwoWay, propertyChanged: WasPreviousPropertyChanged);
        public bool WasPrevious
        {
            get
            {
                return (bool)GetValue(WasPreviousProperty);
            }
            set
            {
                SetValue(WasPreviousProperty, value);
            }
        }
        private static void WasPreviousPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (MiscGraphicsXF)bindable;
            thisItem.Mains.WasPrevious = (bool)newValue;
        }
        public static readonly BindableProperty NumberProperty = BindableProperty.Create(propertyName: "Number", returnType: typeof(int), declaringType: typeof(MiscGraphicsXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: NumberPropertyChanged);
        public int Number
        {
            get
            {
                return (int)GetValue(NumberProperty);
            }
            set
            {
                SetValue(NumberProperty, value);
            }
        }
        private static void NumberPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (MiscGraphicsXF)bindable;
            thisItem.Mains.Number = (int)newValue;
        }
    }
}