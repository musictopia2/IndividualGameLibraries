using BasicGamingUIWPFLibrary.GameGraphics.Base;
using SorryCardGameCP.Cards;
using SorryCardGameCP.Data;
using System.Windows;
using System.Windows.Data;
namespace SorryCardGameWPF
{
    public class CardGraphicsWPF : BaseDeckGraphicsWPF<SorryCardGameCardInformation, SorryCardGameGraphicsCP>
    {
        public static readonly DependencyProperty CategoryProperty = DependencyProperty.Register("Category", typeof(EnumCategory), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(EnumCategory.Blank, new PropertyChangedCallback(CategoryPropertyChanged)));
        public EnumCategory Category
        {
            get
            {
                return (EnumCategory)GetValue(CategoryProperty);
            }
            set
            {
                SetValue(CategoryProperty, value);
            }
        }
        private static void CategoryPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.Category = (EnumCategory)e.NewValue;
        }
        public static readonly DependencyProperty SorryProperty = DependencyProperty.Register("Sorry", typeof(EnumSorry), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(EnumSorry.Blank, new PropertyChangedCallback(SorryPropertyChanged)));
        public EnumSorry Sorry
        {
            get
            {
                return (EnumSorry)GetValue(SorryProperty);
            }
            set
            {
                SetValue(SorryProperty, value);
            }
        }
        private static void SorryPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.Sorry = (EnumSorry)e.NewValue;
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

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(string), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(ColorPropertyChanged)));
        public string Color
        {
            get
            {
                return (string)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }
        private static void ColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.Color = (string)e.NewValue;
        }
        protected override void BeforePainting() { }
        protected override void DoCardBindings()
        {
            SetBinding(CategoryProperty, new Binding(nameof(SorryCardGameCardInformation.Category)));
            SetBinding(SorryProperty, new Binding(nameof(SorryCardGameCardInformation.Sorry)));
            SetBinding(ValueProperty, new Binding(nameof(SorryCardGameCardInformation.Value)));
            SetBinding(ColorProperty, new Binding(nameof(SorryCardGameCardInformation.Color)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
}
