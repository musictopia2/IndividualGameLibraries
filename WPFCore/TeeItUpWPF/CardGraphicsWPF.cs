using BasicGamingUIWPFLibrary.GameGraphics.Base;
using System.Windows;
using TeeItUpCP.Cards;
namespace TeeItUpWPF
{
    public class CardGraphicsWPF : BaseDeckGraphicsWPF<TeeItUpCardInformation, TeeItUpGraphicsCP>//begin
    {
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register("Points", typeof(int), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(-6, (new PropertyChangedCallback(PointsPropertyChanged))));
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
        public static readonly DependencyProperty IsMulliganProperty = DependencyProperty.Register("IsMulligan", typeof(bool), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(IsMulliganPropertyChanged)));
        public bool IsMulligan
        {
            get
            {
                return (bool)GetValue(IsMulliganProperty);
            }
            set
            {
                SetValue(IsMulliganProperty, value);
            }
        }
        private static void IsMulliganPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.IsMulligan = (bool)e.NewValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(PointsProperty, nameof(TeeItUpCardInformation.Points));
            SetBinding(IsMulliganProperty, nameof(TeeItUpCardInformation.IsMulligan));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
}
