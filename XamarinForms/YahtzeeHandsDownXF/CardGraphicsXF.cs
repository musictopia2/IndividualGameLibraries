using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using System;
using Xamarin.Forms;
using YahtzeeHandsDownCP;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
namespace YahtzeeHandsDownXF
{
    public class CardGraphicsXF : BaseDeckGraphicsXF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP>
    {
        public static readonly BindableProperty ColorProperty = BindableProperty.Create(propertyName: "Color", returnType: typeof(EnumColor), declaringType: typeof(CardGraphicsXF), defaultValue: EnumColor.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ColorPropertyChanged);
        public EnumColor Color
        {
            get
            {
                return (EnumColor)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }
        private static void ColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.Color = (EnumColor)newValue;
        }
        public static readonly BindableProperty FirstValueProperty = BindableProperty.Create(propertyName: "FirstValue", returnType: typeof(int), declaringType: typeof(CardGraphicsXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: FirstValuePropertyChanged);
        public int FirstValue
        {
            get
            {
                return (int)GetValue(FirstValueProperty);
            }
            set
            {
                SetValue(FirstValueProperty, FirstValue);
            }
        }
        private static void FirstValuePropertyChanged(BindableObject bindable, object oldFirstValue, object newFirstValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.FirstValue = (int)newFirstValue;
        }
        public static readonly BindableProperty SecondValueProperty = BindableProperty.Create(propertyName: "SecondValue", returnType: typeof(int), declaringType: typeof(CardGraphicsXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: SecondValuePropertyChanged);
        public int SecondValue
        {
            get
            {
                return (int)GetValue(SecondValueProperty);
            }
            set
            {
                SetValue(SecondValueProperty, SecondValue);
            }
        }
        private static void SecondValuePropertyChanged(BindableObject bindable, object oldSecondValue, object newSecondValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.SecondValue = (int)newSecondValue;
        }
        public static readonly BindableProperty IsWildProperty = BindableProperty.Create(propertyName: "IsWild", returnType: typeof(bool), declaringType: typeof(CardGraphicsXF), defaultValue: false, defaultBindingMode: BindingMode.TwoWay, propertyChanged: IsWildPropertyChanged);
        public bool IsWild
        {
            get
            {
                return (bool)GetValue(IsWildProperty);
            }
            set
            {
                SetValue(IsWildProperty, IsWild);
            }
        }
        private static void IsWildPropertyChanged(BindableObject bindable, object oldIsWild, object newIsWild)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.IsWild = (bool)newIsWild;
        }
        protected override void DoCardBindings()
        {
            SetBinding(ColorProperty, new Binding(nameof(YahtzeeHandsDownCardInformation.Color)));
            SetBinding(FirstValueProperty, new Binding(nameof(YahtzeeHandsDownCardInformation.FirstValue)));
            SetBinding(SecondValueProperty, new Binding(nameof(YahtzeeHandsDownCardInformation.SecondValue)));
            SetBinding(IsWildProperty, new Binding(nameof(YahtzeeHandsDownCardInformation.IsWild)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
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
    public class ComboCardWPF : BaseDeckGraphicsXF<ComboCardInfo, ComboCP>
    {
        public ComboCardInfo? CurrentCombo //taking a risk.
        {
            get
            {
                return MainObject!.ThisCombo;
            }
            set
            {
                MainObject!.ThisCombo = value;
            }
        }
        protected override void DoCardBindings()
        {
            //hopefully the risk pays off this time.  if not rethink this one.
        }

        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
    public class ComboHandXF : BaseHandXF<ComboCardInfo, ComboCP, ComboCardWPF>
    {
        protected override void FinishBindings(ComboCardWPF thisDeck, ComboCardInfo thisCard)
        {
            thisDeck.CurrentCombo = thisCard;
        }
    }
    public class ChanceSinglePileXF : BasePileXF<ChanceCardInfo, ChanceGraphicsCP, ChanceCardXF> { }
    public class ComboProportion : IProportionImage
    {
        float IProportionImage.Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.LargeTablet)
                    return 0.9f;
                return .7f; //experiment to see what works for this case.
            }
        }
    }
}