using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using FiveCrownsCP;
using System.Windows;
using System.Windows.Data;
namespace FiveCrownsWPF
{
    public class CardGraphicsWPF : BaseDeckGraphicsWPF<FiveCrownsCardInformation, FiveCrownsGraphicsCP>
    {
        public static readonly DependencyProperty CardValueProperty = DependencyProperty.Register("CardValue", typeof(EnumCardValueList), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(CardValuePropertyChanged)));
        public EnumCardValueList CardValue
        {
            get
            {
                return (EnumCardValueList)GetValue(CardValueProperty);
            }
            set
            {
                SetValue(CardValueProperty, value);
            }
        }
        private static void CardValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.CardValue = (EnumCardValueList)e.NewValue;
        }
        public static readonly DependencyProperty SuitProperty = DependencyProperty.Register("Suit", typeof(EnumSuitList), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(EnumSuitList.None, new PropertyChangedCallback(SuitPropertyChanged)));
        public EnumSuitList Suit
        {
            get
            {
                return (EnumSuitList)GetValue(SuitProperty);
            }
            set
            {
                SetValue(SuitProperty, value);
            }
        }
        private static void SuitPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.Suit = (EnumSuitList)e.NewValue;
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
        protected override void DoCardBindings()
        {
            SetBinding(CardValueProperty, new Binding(nameof(FiveCrownsCardInformation.CardValue)));
            SetBinding(SuitProperty, new Binding(nameof(FiveCrownsCardInformation.Suit))); // i think
        }
    }
}