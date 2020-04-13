using BasicGamingUIWPFLibrary.GameGraphics.Base;
using System.Windows;
using System.Windows.Data;
using YahtzeeHandsDownCP.Cards;
using YahtzeeHandsDownCP.Data;
namespace YahtzeeHandsDownWPF
{
    public class CardGraphicsWPF : BaseDeckGraphicsWPF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP>
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
        public static readonly DependencyProperty FirstValueProperty = DependencyProperty.Register("FirstValue", typeof(int), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(FirstValuePropertyChanged)));
        public int FirstValue
        {
            get
            {
                return (int)GetValue(FirstValueProperty);
            }
            set
            {
                SetValue(FirstValueProperty, value);
            }
        }
        private static void FirstValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.FirstValue = (int)e.NewValue;
        }
        public static readonly DependencyProperty SecondValueProperty = DependencyProperty.Register("SecondValue", typeof(int), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(SecondValuePropertyChanged)));
        public int SecondValue
        {
            get
            {
                return (int)GetValue(SecondValueProperty);
            }
            set
            {
                SetValue(SecondValueProperty, value);
            }
        }
        private static void SecondValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.SecondValue = (int)e.NewValue;
        }
        public static readonly DependencyProperty IsWildProperty = DependencyProperty.Register("IsWild", typeof(bool), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(IsWildPropertyChanged)));
        public bool IsWild
        {
            get
            {
                return (bool)GetValue(IsWildProperty);
            }
            set
            {
                SetValue(IsWildProperty, value);
            }
        }
        private static void IsWildPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.IsWild = (bool)e.NewValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(ColorProperty, new Binding(nameof(YahtzeeHandsDownCardInformation.Color)));
            SetBinding(FirstValueProperty, new Binding(nameof(YahtzeeHandsDownCardInformation.FirstValue)));
            SetBinding(SecondValueProperty, new Binding(nameof(YahtzeeHandsDownCardInformation.SecondValue)));
            SetBinding(IsWildProperty, new Binding(nameof(YahtzeeHandsDownCardInformation.IsWild)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
}
