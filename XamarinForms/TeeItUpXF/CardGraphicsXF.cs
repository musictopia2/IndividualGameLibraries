using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using TeeItUpCP;
using Xamarin.Forms;
namespace TeeItUpXF
{
    public class CardGraphicsXF : BaseDeckGraphicsXF<TeeItUpCardInformation, TeeItUpGraphicsCP>
    {
        public static readonly BindableProperty PointsProperty = BindableProperty.Create(propertyName: "Points", returnType: typeof(int), declaringType: typeof(CardGraphicsXF), defaultValue: -6, defaultBindingMode: BindingMode.TwoWay, propertyChanged: PointsPropertyChanged);

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
        private static void PointsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.Points = (int)newValue;
        }
        public static readonly BindableProperty IsMulliganProperty = BindableProperty.Create(propertyName: "IsMulligan", returnType: typeof(bool), declaringType: typeof(CardGraphicsXF), defaultValue: false, defaultBindingMode: BindingMode.TwoWay, propertyChanged: IsMulliganPropertyChanged);
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
        private static void IsMulliganPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.IsMulligan = (bool)newValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(PointsProperty, new Binding(nameof(TeeItUpCardInformation.Points)));
            SetBinding(IsMulliganProperty, new Binding(nameof(TeeItUpCardInformation.IsMulligan)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
}