using BasicGamingUIXFLibrary.GameGraphics.Base;
using Xamarin.Forms;
using XactikaCP.MiscImages;
using XactikaCP.Data;

namespace XactikaXF
{
    public class XactikaPieceXF : BaseGraphicsXF<PieceCP>
    {
        public static readonly BindableProperty HowManyProperty = BindableProperty.Create(propertyName: "HowMany", returnType: typeof(int), declaringType: typeof(XactikaPieceXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: HowManyPropertyChanged);
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
            var thisItem = (XactikaPieceXF)bindable;
            thisItem.Mains!.HowMany = (int)newValue;
        }
        public static readonly BindableProperty ShapeUsedProperty = BindableProperty.Create(propertyName: "ShapeUsed", returnType: typeof(EnumShapes), declaringType: typeof(XactikaPieceXF), defaultValue: EnumShapes.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ShapeUsedPropertyChanged);
        public EnumShapes ShapeUsed
        {
            get
            {
                return (EnumShapes)GetValue(ShapeUsedProperty);
            }
            set
            {
                SetValue(ShapeUsedProperty, value);
            }
        }
        private static void ShapeUsedPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (XactikaPieceXF)bindable;
            thisItem.Mains!.ShapeUsed = (EnumShapes)newValue;
        }
    }
}