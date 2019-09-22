using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using XactikaCP;
using Xamarin.Forms;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
namespace XactikaXF
{
    public class CardGraphicsXF : BaseDeckGraphicsXF<XactikaCardInformation, XactikaGraphicsCP>
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
        public static readonly BindableProperty HowManyBallsProperty = BindableProperty.Create(propertyName: "HowManyBalls", returnType: typeof(int), declaringType: typeof(CardGraphicsXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: HowManyBallsPropertyChanged);
        public int HowManyBalls
        {
            get
            {
                return (int)GetValue(HowManyBallsProperty);
            }
            set
            {
                SetValue(HowManyBallsProperty, HowManyBalls);
            }
        }
        private static void HowManyBallsPropertyChanged(BindableObject bindable, object oldHowManyBalls, object newHowManyBalls)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.HowManyBalls = (int)newHowManyBalls;
        }
        public static readonly BindableProperty HowManyConesProperty = BindableProperty.Create(propertyName: "HowManyCones", returnType: typeof(int), declaringType: typeof(CardGraphicsXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: HowManyConesPropertyChanged);
        public int HowManyCones
        {
            get
            {
                return (int)GetValue(HowManyConesProperty);
            }
            set
            {
                SetValue(HowManyConesProperty, HowManyCones);
            }
        }
        private static void HowManyConesPropertyChanged(BindableObject bindable, object oldHowManyCones, object newHowManyCones)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.HowManyCones = (int)newHowManyCones;
        }
        public static readonly BindableProperty HowManyCubesProperty = BindableProperty.Create(propertyName: "HowManyCubes", returnType: typeof(int), declaringType: typeof(CardGraphicsXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: HowManyCubesPropertyChanged);
        public int HowManyCubes
        {
            get
            {
                return (int)GetValue(HowManyCubesProperty);
            }
            set
            {
                SetValue(HowManyCubesProperty, HowManyCubes);
            }
        }
        private static void HowManyCubesPropertyChanged(BindableObject bindable, object oldHowManyCubes, object newHowManyCubes)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.HowManyCubes = (int)newHowManyCubes;
        }
        public static readonly BindableProperty HowManyStarsProperty = BindableProperty.Create(propertyName: "HowManyStars", returnType: typeof(int), declaringType: typeof(CardGraphicsXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: HowManyStarsPropertyChanged);
        public int HowManyStars
        {
            get
            {
                return (int)GetValue(HowManyStarsProperty);
            }
            set
            {
                SetValue(HowManyStarsProperty, HowManyStars);
            }
        }
        private static void HowManyStarsPropertyChanged(BindableObject bindable, object oldHowManyStars, object newHowManyStars)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.HowManyStars = (int)newHowManyStars;
        }
        protected override void DoCardBindings()
        {
            SetBinding(HowManyBallsProperty, new Binding(nameof(XactikaCardInformation.HowManyBalls)));
            SetBinding(HowManyConesProperty, new Binding(nameof(XactikaCardInformation.HowManyCones)));
            SetBinding(HowManyCubesProperty, new Binding(nameof(XactikaCardInformation.HowManyCubes)));
            SetBinding(HowManyStarsProperty, new Binding(nameof(XactikaCardInformation.HowManyStars)));
            SetBinding(ValueProperty, new Binding(nameof(XactikaCardInformation.Value)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
    public class XactikaPieceXF : BaseGraphicsXF<PieceCP>
    {
        public static readonly BindableProperty HowManyProperty = BindableProperty.Create(propertyName: "HowMany", returnType: typeof(int), declaringType: typeof(XactikaPieceXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: HowManyPropertyChanged);
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
        private static void HowManyPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (XactikaPieceXF)bindable;
            thisItem.Mains!.HowMany = (int)newValue;
        }
        public static readonly BindableProperty ShapeUsedProperty = BindableProperty.Create(propertyName: "ShapeUsed", returnType: typeof(EnumShapes), declaringType: typeof(XactikaPieceXF), defaultValue: EnumShapes.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ShapeUsedPropertyChanged);
        public EnumShapes ShapeUsed
        {
            get
            {
                return (EnumShapes)GetValue(ShapeUsedProperty);
            }
            set
            {
                SetValue(ShapeUsedProperty, value);
            }
        }
        private static void ShapeUsedPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (XactikaPieceXF)bindable;
            thisItem.Mains!.ShapeUsed = (EnumShapes)newValue;
        }
    }
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return .65f;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return .8f;
                return 1.1f;
            }
        }
    }
}