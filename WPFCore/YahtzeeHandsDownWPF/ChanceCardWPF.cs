using BasicGamingUIWPFLibrary.GameGraphics.Base;
using YahtzeeHandsDownCP.Cards;
using System.Windows;
using System.Windows.Data;
namespace YahtzeeHandsDownWPF
{
    public class ChanceCardWPF : BaseDeckGraphicsWPF<ChanceCardInfo, ChanceGraphicsCP>
    {
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register("Points", typeof(int), typeof(ChanceCardWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(PointsPropertyChanged)));
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
            var thisItem = (ChanceCardWPF)sender;
            thisItem.MainObject!.Points = (int)e.NewValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(PointsProperty, new Binding(nameof(ChanceCardInfo.Points)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
}
