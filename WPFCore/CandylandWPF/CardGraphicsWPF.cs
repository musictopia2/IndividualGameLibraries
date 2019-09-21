using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using CandylandCP;
using System.Windows;
using System.Windows.Data;
namespace CandylandWPF
{
    public class CardGraphicsWPF : BaseDeckGraphicsWPF<CandylandCardData, CandylandCardGraphicsCP>
    {
        public static readonly DependencyProperty WhichTypeProperty = DependencyProperty.Register("WhichType", typeof(EnumCandyLandType), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(EnumCandyLandType.None, new PropertyChangedCallback(WhichTypePropertyChanged)));
        public EnumCandyLandType WhichType
        {
            get
            {
                return (EnumCandyLandType)GetValue(WhichTypeProperty);
            }
            set
            {
                SetValue(WhichTypeProperty, value);
            }
        }
        private static void WhichTypePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.WhichCard = (EnumCandyLandType)e.NewValue;
        }
        public static readonly DependencyProperty HowManyProperty = DependencyProperty.Register("HowMany", typeof(int), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(HowManyPropertyChanged)));
        public int HowMany
        {
            get
            {
                return (int)GetValue(HowManyProperty);
            }
            set
            {
                SetValue(HowManyProperty, value);
            }
        }
        private static void HowManyPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.HowMany = (int)e.NewValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(CardGraphicsWPF.WhichTypeProperty, new Binding(nameof(CandylandCardData.WhichCard)));
            SetBinding(CardGraphicsWPF.HowManyProperty, new Binding(nameof(CandylandCardData.HowMany)));
        }
        protected override void PopulateInitObject() //this is needed too.
        {
            MainObject!.Init();
        }
    }
}