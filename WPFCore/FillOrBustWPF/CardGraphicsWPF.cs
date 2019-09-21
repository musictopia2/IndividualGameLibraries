using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using FillOrBustCP;
using System.Windows;
using System.Windows.Data;
namespace FillOrBustWPF
{
    public class CardGraphicsWPF : BaseDeckGraphicsWPF<FillOrBustCardInformation, FillOrBustGraphicsCP>
    {
        public static readonly DependencyProperty CategoryProperty = DependencyProperty.Register("Category", typeof(EnumCardStatusList), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(EnumCardStatusList.Unknown, new PropertyChangedCallback(CategoryPropertyChanged)));
        public EnumCardStatusList Category
        {
            get
            {
                return (EnumCardStatusList)GetValue(CategoryProperty);
            }
            set
            {
                SetValue(CategoryProperty, value);
            }
        }
        private static void CategoryPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.Category = (EnumCardStatusList)e.NewValue;
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
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
        protected override void DoCardBindings()
        {
            SetBinding(CategoryProperty, new Binding(nameof(FillOrBustCardInformation.Status)));
            SetBinding(ValueProperty, new Binding(nameof(FillOrBustCardInformation.Value)));
        }
    }
}
