using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.GameGraphics.Base;
using FluxxCP.Cards;
using Xamarin.Forms;
namespace FluxxXF
{
    public class CardGraphicsXF : BaseDeckGraphicsXF<FluxxCardInformation, FluxxGraphicsCP>
    {
        public static readonly BindableProperty ValueProperty = BindableProperty.Create(propertyName: "Value", returnType: typeof(int), declaringType: typeof(CardGraphicsXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ValuePropertyChanged);
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
        private static void ValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.Value = (int)newValue;
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
    public class KeeperXF : BaseDeckGraphicsXF<KeeperCard, FluxxGraphicsCP>
    {
        public static readonly BindableProperty ValueProperty = BindableProperty.Create(propertyName: "Value", returnType: typeof(int), declaringType: typeof(KeeperXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ValuePropertyChanged);
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
        private static void ValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (KeeperXF)bindable;
            thisItem.MainObject!.Value = (int)newValue;
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
    public class GoalXF : BaseDeckGraphicsXF<GoalCard, FluxxGraphicsCP>
    {
        public static readonly BindableProperty ValueProperty = BindableProperty.Create(propertyName: "Value", returnType: typeof(int), declaringType: typeof(GoalXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ValuePropertyChanged);
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
        private static void ValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (GoalXF)bindable;
            thisItem.MainObject!.Value = (int)newValue;
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
    public class FluxxHandXF : BaseHandXF<FluxxCardInformation, FluxxGraphicsCP, CardGraphicsXF> { }
    public class KeeperHandXF : BaseHandXF<KeeperCard, FluxxGraphicsCP, KeeperXF> { }
    public class GoalHandXF : BaseHandXF<GoalCard, FluxxGraphicsCP, GoalXF> { }
}