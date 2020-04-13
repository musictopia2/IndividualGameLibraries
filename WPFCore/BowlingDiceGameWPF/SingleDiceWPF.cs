using BasicGamingUIWPFLibrary.GameGraphics.Base;
using BowlingDiceGameCP.Data;
using System.Windows;
namespace BowlingDiceGameWPF
{
    public class SingleDiceWPF : BaseGraphicsWPF<SingleDrawingDiceCP>
    {
        public static readonly DependencyProperty DidHitProperty = DependencyProperty.Register("DidHit", typeof(bool), typeof(SingleDiceWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(DidHitPropertyChanged)));
        public bool DidHit
        {
            get
            {
                return (bool)GetValue(DidHitProperty);
            }
            set
            {
                SetValue(DidHitProperty, value);
            }
        }
        private static void DidHitPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (SingleDiceWPF)sender;
            thisItem.Mains.DidHit = (bool)e.NewValue;
        }
    }
}