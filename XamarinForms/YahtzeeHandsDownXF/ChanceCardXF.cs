using BasicGamingUIXFLibrary.GameGraphics.Base;
using Xamarin.Forms;
using YahtzeeHandsDownCP.Cards;

namespace YahtzeeHandsDownXF
{
    public class ChanceCardXF : BaseDeckGraphicsXF<ChanceCardInfo, ChanceGraphicsCP>
    {
        public static readonly BindableProperty PointsProperty = BindableProperty.Create(propertyName: "Points", returnType: typeof(int), declaringType: typeof(ChanceCardXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: PointsPropertyChanged);
        public int Points
        {
            get
            {
                return (int)GetValue(PointsProperty);
            }
            set
            {
                SetValue(PointsProperty, Points);
            }
        }
        private static void PointsPropertyChanged(BindableObject bindable, object oldPoints, object newPoints)
        {
            var thisItem = (ChanceCardXF)bindable;
            thisItem.MainObject!.Points = (int)newPoints;
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