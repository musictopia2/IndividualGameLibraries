using BasicGamingUIWPFLibrary.GameGraphics.Base;
using RageCardGameCP.Cards;
using RageCardGameCP.Data;
using System.Windows;
using System.Windows.Data;
namespace RageCardGameWPF
{
    public class CardGraphicsWPF : BaseDeckGraphicsWPF<RageCardGameCardInformation, RageCardGameGraphicsCP>
    {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(EnumColor), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(ColorPropertyChanged)));
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
        public static readonly DependencyProperty SpecialTypeProperty = DependencyProperty.Register("SpecialType", typeof(EnumSpecialType), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(EnumSpecialType.Blank, new PropertyChangedCallback(SpecialTypePropertyChanged)));
        public EnumSpecialType SpecialType
        {
            get
            {
                return (EnumSpecialType)GetValue(SpecialTypeProperty);
            }
            set
            {
                SetValue(SpecialTypeProperty, value);
            }
        }
        private static void SpecialTypePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.SpecialType = (EnumSpecialType)e.NewValue;
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
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
        protected override void DoCardBindings()
        {
            SetBinding(ColorProperty, new Binding(nameof(RageCardGameCardInformation.Color)));
            SetBinding(SpecialTypeProperty, new Binding(nameof(RageCardGameCardInformation.SpecialType)));
            SetBinding(ValueProperty, new Binding(nameof(RageCardGameCardInformation.Value)));
        }
    }
}
