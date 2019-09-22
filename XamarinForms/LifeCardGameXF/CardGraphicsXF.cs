using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using LifeCardGameCP;
using Xamarin.Forms;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
namespace LifeCardGameXF
{
    public class CardGraphicsXF : BaseDeckGraphicsXF<LifeCardGameCardInformation, LifeCardGameGraphicsCP>
    {
        public static readonly BindableProperty ActionProperty = BindableProperty.Create(propertyName: "Action", returnType: typeof(EnumAction), declaringType: typeof(CardGraphicsXF), defaultValue: EnumAction.Unknown, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ActionPropertyChanged);
        public EnumAction Action
        {
            get
            {
                return (EnumAction)GetValue(ActionProperty);
            }
            set
            {
                SetValue(ActionProperty, value);
            }
        }
        private static void ActionPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.Action = (EnumAction)newValue;
        }
        public static readonly BindableProperty RequirementProperty = BindableProperty.Create(propertyName: "Requirement", returnType: typeof(EnumSpecialCardCategory), declaringType: typeof(CardGraphicsXF), defaultValue: EnumSpecialCardCategory.Unknown, defaultBindingMode: BindingMode.TwoWay, propertyChanged: RequirementPropertyChanged);
        public EnumSpecialCardCategory Requirement
        {
            get
            {
                return (EnumSpecialCardCategory)GetValue(RequirementProperty);
            }
            set
            {
                SetValue(RequirementProperty, value);
            }
        }
        private static void RequirementPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.Requirement = (EnumSpecialCardCategory)newValue;
        }
        public static readonly BindableProperty SpecialCategoryProperty = BindableProperty.Create(propertyName: "SpecialCategory", returnType: typeof(EnumSpecialCardCategory), declaringType: typeof(CardGraphicsXF), defaultValue: EnumSpecialCardCategory.Unknown, defaultBindingMode: BindingMode.TwoWay, propertyChanged: SpecialCategoryPropertyChanged);
        public EnumSpecialCardCategory SpecialCategory
        {
            get
            {
                return (EnumSpecialCardCategory)GetValue(SpecialCategoryProperty);
            }
            set
            {
                SetValue(SpecialCategoryProperty, value);
            }
        }
        private static void SpecialCategoryPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.SpecialCategory = (EnumSpecialCardCategory)newValue;
        }
        public static readonly BindableProperty FirstCardCategoryProperty = BindableProperty.Create(propertyName: "FirstCardCategory", returnType: typeof(EnumFirstCardCategory), declaringType: typeof(CardGraphicsXF), defaultValue: EnumFirstCardCategory.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: FirstCardCategoryPropertyChanged);
        public EnumFirstCardCategory FirstCardCategory
        {
            get
            {
                return (EnumFirstCardCategory)GetValue(FirstCardCategoryProperty);
            }
            set
            {
                SetValue(FirstCardCategoryProperty, value);
            }
        }
        private static void FirstCardCategoryPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.FirstCategory = (EnumFirstCardCategory)newValue;
        }
        public static readonly BindableProperty PointsProperty = BindableProperty.Create(propertyName: "Points", returnType: typeof(int), declaringType: typeof(CardGraphicsXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: PointsPropertyChanged);
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
        public static readonly BindableProperty DescriptionProperty = BindableProperty.Create(propertyName: "Description", returnType: typeof(string), declaringType: typeof(CardGraphicsXF), defaultValue: "", defaultBindingMode: BindingMode.TwoWay, propertyChanged: DescriptionPropertyChanged);
        public string Description
        {
            get
            {
                return (string)GetValue(DescriptionProperty);
            }
            set
            {
                SetValue(DescriptionProperty, value);
            }
        }
        private static void DescriptionPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.Description = (string)newValue;
        }

        public static readonly BindableProperty OpponentKeepsCardProperty = BindableProperty.Create(propertyName: "OpponentKeepsCard", returnType: typeof(bool), declaringType: typeof(CardGraphicsXF), defaultValue: false, defaultBindingMode: BindingMode.TwoWay, propertyChanged: OpponentKeepsCardPropertyChanged);
        public bool OpponentKeepsCard
        {
            get
            {
                return (bool)GetValue(OpponentKeepsCardProperty);
            }
            set
            {
                SetValue(OpponentKeepsCardProperty, value);
            }
        }
        private static void OpponentKeepsCardPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.OpponentKeepsCard = (bool)newValue;
        }
        public static readonly BindableProperty SwitchCategoryProperty = BindableProperty.Create(propertyName: "SwitchCategory", returnType: typeof(EnumSwitchCategory), declaringType: typeof(CardGraphicsXF), defaultValue: EnumSwitchCategory.Unknown, defaultBindingMode: BindingMode.TwoWay, propertyChanged: SwitchCategoryPropertyChanged);
        public EnumSwitchCategory SwitchCategory
        {
            get
            {
                return (EnumSwitchCategory)GetValue(SwitchCategoryProperty);
            }
            set
            {
                SetValue(SwitchCategoryProperty, value);
            }
        }
        private static void SwitchCategoryPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.SwitchCategory = (EnumSwitchCategory)newValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(ActionProperty, new Binding(nameof(LifeCardGameCardInformation.Action)));
            SetBinding(RequirementProperty, new Binding(nameof(LifeCardGameCardInformation.Requirement)));
            SetBinding(SpecialCategoryProperty, new Binding(nameof(LifeCardGameCardInformation.SpecialCategory)));
            SetBinding(FirstCardCategoryProperty, new Binding(nameof(LifeCardGameCardInformation.Category)));
            SetBinding(PointsProperty, new Binding(nameof(LifeCardGameCardInformation.Points)));
            SetBinding(DescriptionProperty, new Binding(nameof(LifeCardGameCardInformation.Description)));
            SetBinding(OpponentKeepsCardProperty, new Binding(nameof(LifeCardGameCardInformation.OpponentKeepsCard)));
            SetBinding(SwitchCategoryProperty, new Binding(nameof(LifeCardGameCardInformation.SwitchCategory)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
    public class LifeHandXF : BaseHandXF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsXF>, ILifeScroll
    {
        void ILifeScroll.RecalculatePositioning() //iffy
        {
            RecalculatePositioning();
        }
        async void ILifeScroll.ScrollToBottom() //iffy.
        {
            await ScrollToBottomAsync();
        }
    }
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.LargeTablet)
                    return 1.2f;
                return 0.95f; //experiment.
            }
        }
    }
}