using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using HitTheDeckCP;
using System.Windows;
namespace HitTheDeckWPF
{
    public class CardGraphicsWPF : BaseDeckGraphicsWPF<HitTheDeckCardInformation, HitTheDeckGraphicsCP>
    {
        public static readonly DependencyProperty CardColorProperty = DependencyProperty.Register("CardColor", typeof(string), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(CardColorPropertyChanged)));
        public string CardColor
        {
            get
            {
                return (string)GetValue(CardColorProperty);
            }
            set
            {
                SetValue(CardColorProperty, value);
            }
        }
        private static void CardColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.MainGraphics!.BackgroundColor = (string)e.NewValue;
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
        public static readonly DependencyProperty CardTypeProperty = DependencyProperty.Register("CardType", typeof(EnumTypeList), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(CardTypePropertyChanged)));
        public EnumTypeList CardType
        {
            get
            {
                return (EnumTypeList)GetValue(CardTypeProperty);
            }
            set
            {
                SetValue(CardTypeProperty, value);
            }
        }
        private static void CardTypePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.CardType = (EnumTypeList)e.NewValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(CardColorProperty, nameof(HitTheDeckCardInformation.Color));
            SetBinding(ValueProperty, nameof(HitTheDeckCardInformation.Number)); // i think
            SetBinding(CardTypeProperty, nameof(HitTheDeckCardInformation.CardType));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
}