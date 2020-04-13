using BasicGamingUIWPFLibrary.GameGraphics.Base;
using PaydayCP.Cards;
using PaydayCP.Data;
using PaydayCP.Graphics;
using System.Windows;
using System.Windows.Data;

namespace PaydayWPF
{
    public abstract class PaydayBaseCardWPF<CA, CP> : BaseDeckGraphicsWPF<CA, CP>

        where CA : CardInformation, new()
        where CP : CardGraphicsCP, new()
    {
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
        public static readonly DependencyProperty CardCategoryProperty = DependencyProperty.Register("CardCategory", typeof(EnumCardCategory), typeof(PaydayBaseCardWPF<CA, CardGraphicsCP>), new FrameworkPropertyMetadata(EnumCardCategory.None, new PropertyChangedCallback(CardCategoryPropertyChanged)));
        public EnumCardCategory CardCategory
        {
            get
            {
                return (EnumCardCategory)GetValue(CardCategoryProperty);
            }
            set
            {
                SetValue(CardCategoryProperty, value);
            }
        }
        private static void CardCategoryPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (PaydayBaseCardWPF<CA, CardGraphicsCP>)sender;
            thisItem.MainObject!.CardCategory = (EnumCardCategory)e.NewValue;
        }
        public static readonly DependencyProperty IndexProperty = DependencyProperty.Register("Index", typeof(int), typeof(PaydayBaseCardWPF<CA, CardGraphicsCP>), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(IndexPropertyChanged)));
        public int Index
        {
            get
            {
                return (int)GetValue(IndexProperty);
            }
            set
            {
                SetValue(IndexProperty, value);
            }
        }
        private static void IndexPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (PaydayBaseCardWPF<CA, CardGraphicsCP>)sender;
            thisItem.MainObject!.Index = (int)e.NewValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(PaydayBaseCardWPF<CA, CardGraphicsCP>.CardCategoryProperty, new Binding(nameof(CardInformation.CardCategory)));
            SetBinding(PaydayBaseCardWPF<CA, CardGraphicsCP>.IndexProperty, new Binding(nameof(CardInformation.Index)));
        }
    }
    public class MailCardWPF : PaydayBaseCardWPF<MailCard, CardGraphicsCP> { }
    public class DealCardWPF : PaydayBaseCardWPF<DealCard, CardGraphicsCP> { }
}
