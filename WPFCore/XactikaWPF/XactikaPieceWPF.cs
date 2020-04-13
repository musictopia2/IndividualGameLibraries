using BasicGamingUIWPFLibrary.GameGraphics.Base;
using System.Windows;
using XactikaCP.MiscImages;
using XactikaCP.Data;
namespace XactikaWPF
{
    public class XactikaPieceWPF : BaseGraphicsWPF<PieceCP>
    {
        public static readonly DependencyProperty ShapeUsedProperty = DependencyProperty.Register("ShapeUsed", typeof(EnumShapes), typeof(XactikaPieceWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(ShapeUsedPropertyChanged)));
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
        private static void ShapeUsedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (XactikaPieceWPF)sender;
            thisItem.Mains.ShapeUsed = (EnumShapes)e.NewValue;
        }
        public static readonly DependencyProperty HowManyProperty = DependencyProperty.Register("HowMany", typeof(int), typeof(XactikaPieceWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(HowManyPropertyChanged)));
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
        private static void HowManyPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (XactikaPieceWPF)sender;
            thisItem.Mains.HowMany = (int)e.NewValue;
        }
    }
}
