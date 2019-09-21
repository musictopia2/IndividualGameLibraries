using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using RackoCP;
using System.Windows;
using System.Windows.Data;
namespace RackoWPF
{
    public class CardGraphicsWPF : BaseDeckGraphicsWPF<RackoCardInformation, RackoGraphicsCP>
    {
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
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
        protected override void DoCardBindings()
        {
            SetBinding(ValueProperty, new Binding(nameof(RackoCardInformation.Value)));
        }
    }
}