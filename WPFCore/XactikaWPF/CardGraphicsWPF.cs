using BasicGamingUIWPFLibrary.GameGraphics.Base;
using System.Windows;
using System.Windows.Data;
using XactikaCP.Cards;
namespace XactikaWPF
{
    public class CardGraphicsWPF : BaseDeckGraphicsWPF<XactikaCardInformation, XactikaGraphicsCP>
    {
        public static readonly DependencyProperty HowManyBallsProperty = DependencyProperty.Register("HowManyBalls", typeof(int), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(HowManyBallsPropertyChanged)));
        public int HowManyBalls
        {
            get
            {
                return (int)GetValue(HowManyBallsProperty);
            }
            set
            {
                SetValue(HowManyBallsProperty, value);
            }
        }
        private static void HowManyBallsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.HowManyBalls = (int)e.NewValue;
        }
        public static readonly DependencyProperty HowManyConesProperty = DependencyProperty.Register("HowManyCones", typeof(int), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(HowManyConesPropertyChanged)));
        public int HowManyCones
        {
            get
            {
                return (int)GetValue(HowManyConesProperty);
            }
            set
            {
                SetValue(HowManyConesProperty, value);
            }
        }
        private static void HowManyConesPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.HowManyCones = (int)e.NewValue;
        }
        public static readonly DependencyProperty HowManyCubesProperty = DependencyProperty.Register("HowManyCubes", typeof(int), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(HowManyCubesPropertyChanged)));
        public int HowManyCubes
        {
            get
            {
                return (int)GetValue(HowManyCubesProperty);
            }
            set
            {
                SetValue(HowManyCubesProperty, value);
            }
        }
        private static void HowManyCubesPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.HowManyCubes = (int)e.NewValue;
        }
        public static readonly DependencyProperty HowManyStarsProperty = DependencyProperty.Register("HowManyStars", typeof(int), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(HowManyStarsPropertyChanged)));
        public int HowManyStars
        {
            get
            {
                return (int)GetValue(HowManyStarsProperty);
            }
            set
            {
                SetValue(HowManyStarsProperty, value);
            }
        }
        private static void HowManyStarsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.HowManyStars = (int)e.NewValue;
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
        protected override void DoCardBindings()
        {
            SetBinding(HowManyBallsProperty, new Binding(nameof(XactikaCardInformation.HowManyBalls)));
            SetBinding(HowManyConesProperty, new Binding(nameof(XactikaCardInformation.HowManyCones)));
            SetBinding(HowManyCubesProperty, new Binding(nameof(XactikaCardInformation.HowManyCubes)));
            SetBinding(HowManyStarsProperty, new Binding(nameof(XactikaCardInformation.HowManyStars)));
            SetBinding(ValueProperty, new Binding(nameof(XactikaCardInformation.Value)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
}
