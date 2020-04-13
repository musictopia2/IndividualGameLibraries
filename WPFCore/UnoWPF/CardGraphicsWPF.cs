using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using System.Windows;
using UnoCP.Cards;
using UnoCP.Data;
namespace UnoWPF
{
    public class CardGraphicsWPF : BaseColorCardsWPF<UnoCardInformation, UnoGraphicsCP>
    {
        public static readonly DependencyProperty WhichTypeProperty = DependencyProperty.Register("WhichType", typeof(EnumCardTypeList), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(WhichTypePropertyChanged)));
        public EnumCardTypeList WhichType
        {
            get
            {
                return (EnumCardTypeList)GetValue(WhichTypeProperty);
            }
            set
            {
                SetValue(WhichTypeProperty, value);
            }
        }
        private static void WhichTypePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.WhichType = (EnumCardTypeList)e.NewValue;
        }
        public static readonly DependencyProperty DrawProperty = DependencyProperty.Register("Draw", typeof(int), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(DrawPropertyChanged)));
        public int Draw
        {
            get
            {
                return (int)GetValue(DrawProperty);
            }
            set
            {
                SetValue(DrawProperty, value);
            }
        }
        private static void DrawPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.Draw = (int)e.NewValue;
        }
        public static readonly DependencyProperty NumberProperty = DependencyProperty.Register("Number", typeof(int), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(NumberPropertyChanged)));
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
        private static void NumberPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.Number = (int)e.NewValue;
        }
        protected override void DoCardBindings()
        {
            base.DoCardBindings();
            SetBinding(WhichTypeProperty, nameof(UnoCardInformation.WhichType));
            SetBinding(DrawProperty, nameof(UnoCardInformation.Draw));
            SetBinding(NumberProperty, nameof(UnoCardInformation.Number));
        }
    }
}
