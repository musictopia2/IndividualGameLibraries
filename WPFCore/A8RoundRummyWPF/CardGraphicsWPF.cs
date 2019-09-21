using A8RoundRummyCP;
using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using System.Windows;
using System.Windows.Data;
namespace A8RoundRummyWPF
{
    public class CardGraphicsWPF : BaseDeckGraphicsWPF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP>
    {
        public static readonly DependencyProperty CardTypeProperty = DependencyProperty.Register("CardType", typeof(EnumCardType), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(EnumCardType.None, new PropertyChangedCallback(CardTypePropertyChanged)));
        public EnumCardType CardType
        {
            get
            {
                return (EnumCardType)GetValue(CardTypeProperty);
            }
            set
            {
                SetValue(CardTypeProperty, value);
            }
        }
        private static void CardTypePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.CardType = (EnumCardType)e.NewValue;
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(ValuePropertyChanged)));
        public int Value
        {
            get
            {
                return (int)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }
        private static void ValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.Value = (int)e.NewValue;
        }
        public static readonly DependencyProperty ShapeProperty = DependencyProperty.Register("Shape", typeof(EnumCardShape), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(EnumCardShape.Blank, new PropertyChangedCallback(ShapePropertyChanged)));
        public EnumCardShape Shape
        {
            get
            {
                return (EnumCardShape)GetValue(ShapeProperty);
            }
            set
            {
                SetValue(ShapeProperty, value);
            }
        }
        private static void ShapePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.Shape = (EnumCardShape)e.NewValue;
        }
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(EnumColor), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(EnumColor.Blank, new PropertyChangedCallback(ColorPropertyChanged)));
        public EnumColor Color
        {
            get
            {
                return (EnumColor)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }
        private static void ColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.Color = (EnumColor)e.NewValue;
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
        protected override void DoCardBindings()
        {
            SetBinding(CardTypeProperty, new Binding(nameof(A8RoundRummyCardInformation.CardType)));
            SetBinding(ValueProperty, new Binding(nameof(A8RoundRummyCardInformation.Value)));
            SetBinding(ShapeProperty, new Binding(nameof(A8RoundRummyCardInformation.Shape)));
            SetBinding(ColorProperty, new Binding(nameof(A8RoundRummyCardInformation.Color)));
        }
    }
}