using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using BowlingDiceGameCP;
using Xamarin.Forms;
namespace BowlingDiceGameXF
{
    public class SingleDiceXF : BaseGraphicsXF<SingleDrawingDiceCP>
    {
        public static readonly BindableProperty DidHitProperty = BindableProperty.Create(propertyName: "DidHit", returnType: typeof(bool), declaringType: typeof(SingleDiceXF), defaultValue: false, defaultBindingMode: BindingMode.TwoWay, propertyChanged: DidHitPropertyChanged);
        public bool DidHit
        {
            get
            {
                return (bool)GetValue(DidHitProperty);
            }
            set
            {
                SetValue(DidHitProperty, value);
            }
        }
        private static void DidHitPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (SingleDiceXF)bindable;
            thisItem.Mains.DidHit = (bool)newValue;
        }
    }
}