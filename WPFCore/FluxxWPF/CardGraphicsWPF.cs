using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using FluxxCP;
using System.Windows;
using System.Windows.Data;
namespace FluxxWPF
{
    public class CardGraphicsWPF : BaseDeckGraphicsWPF<FluxxCardInformation, FluxxGraphicsCP>
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(ValuePropertyChanged)));
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
            SetBinding(ValueProperty, new Binding(nameof(FluxxCardInformation.Index)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
    public class KeeperWPF : BaseDeckGraphicsWPF<KeeperCard, FluxxGraphicsCP>
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(KeeperWPF), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(ValuePropertyChanged)));
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
            var thisItem = (KeeperWPF)sender;
            thisItem.MainObject!.Value = (int)e.NewValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(ValueProperty, new Binding(nameof(FluxxCardInformation.Index)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
    public class GoalWPF : BaseDeckGraphicsWPF<GoalCard, FluxxGraphicsCP>
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(GoalWPF), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(ValuePropertyChanged)));
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
            var thisItem = (GoalWPF)sender;
            thisItem.MainObject!.Value = (int)e.NewValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(ValueProperty, new Binding(nameof(FluxxCardInformation.Index)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
    public class FluxxHandWPF : BaseHandWPF<FluxxCardInformation, FluxxGraphicsCP, CardGraphicsWPF> { }
    public class KeeperHandWPF : BaseHandWPF<KeeperCard, FluxxGraphicsCP, KeeperWPF> { }
    public class GoalHandWPF : BaseHandWPF<GoalCard, FluxxGraphicsCP, GoalWPF> { }
}