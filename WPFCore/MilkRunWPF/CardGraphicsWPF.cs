using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using MilkRunCP;
using System.Windows;
namespace MilkRunWPF
{
    public class CardGraphicsWPF : BaseDeckGraphicsWPF<MilkRunCardInformation, MilkRunGraphicsCP>
    {
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register("Points", typeof(int), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(PointsPropertyChanged)));
        public int Points
        {
            get
            {
                return (int)GetValue(PointsProperty);
            }
            set
            {
                SetValue(PointsProperty, value);
            }
        }
        private static void PointsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.Points = (int)e.NewValue;
        }
        public static readonly DependencyProperty MilkTypeProperty = DependencyProperty.Register("MilkType", typeof(EnumMilkType), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(MilkTypePropertyChanged)));
        public EnumMilkType MilkType
        {
            get
            {
                return (EnumMilkType)GetValue(MilkTypeProperty);
            }
            set
            {
                SetValue(MilkTypeProperty, value);
            }
        }
        private static void MilkTypePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.MilkType = (EnumMilkType)e.NewValue;
        }
        public static readonly DependencyProperty CardCategoryProperty = DependencyProperty.Register("CardCategory", typeof(EnumCardCategory), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(CardCategoryPropertyChanged)));
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
            SetBinding(PointsProperty, nameof(MilkRunCardInformation.Points));
            SetBinding(MilkTypeProperty, nameof(MilkRunCardInformation.MilkCategory));
            SetBinding(CardCategoryProperty, nameof(MilkRunCardInformation.CardCategory));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
}