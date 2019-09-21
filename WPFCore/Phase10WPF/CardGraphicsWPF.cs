using BaseGPXWindowsAndControlsCore.GameGraphics.Cards;
using Phase10CP;
using System.Windows;
using System.Windows.Data;
namespace Phase10WPF
{
    public class CardGraphicsWPF : BaseColorCardsWPF<Phase10CardInformation, Phase10GraphicsCP>
    {
        public static readonly DependencyProperty CardCategoryProperty = DependencyProperty.Register("CardCategory", typeof(EnumCardCategory), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(EnumCardCategory.Blank, new PropertyChangedCallback(CardCategoryPropertyChanged)));
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
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.CardCategory = (EnumCardCategory)e.NewValue;
        }
        protected override void DoCardBindings()
        {
            base.DoCardBindings();
            SetBinding(CardCategoryProperty, new Binding(nameof(Phase10CardInformation.CardCategory)));
        }
    }
}