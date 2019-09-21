using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using SequenceDiceCP;
using System.Windows;
namespace SequenceDiceWPF
{
    public class MiscGraphicsWPF : BaseGraphicsWPF<MiscGraphicsCP>
    {
        public static readonly DependencyProperty WasPreviousProperty = DependencyProperty.Register("WasPrevious", typeof(bool), typeof(MiscGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(WasPreviousPropertyChanged)));

        public bool WasPrevious
        {
            get
            {
                return (bool)GetValue(WasPreviousProperty);
            }
            set
            {
                SetValue(WasPreviousProperty, value);
            }
        }
        private static void WasPreviousPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (MiscGraphicsWPF)sender;
            thisItem.Mains.WasPrevious = (bool)e.NewValue;
        }
        public static readonly DependencyProperty NumberProperty = DependencyProperty.Register("Number", typeof(int), typeof(MiscGraphicsWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(NumberPropertyChanged)));
        public int Number
        {
            get
            {
                return (int)GetValue(NumberProperty);
            }
            set
            {
                SetValue(NumberProperty, value);
            }
        }
        private static void NumberPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (MiscGraphicsWPF)sender;
            thisItem.Mains.Number = (int)e.NewValue;
        }
    }
}