using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.GameGraphics.Base;
using LifeCardGameCP.Cards;
using LifeCardGameCP.Data;
using LifeCardGameCP.Logic;
using System.Windows;
using System.Windows.Data;
namespace LifeCardGameWPF
{
    public class CardGraphicsWPF : BaseDeckGraphicsWPF<LifeCardGameCardInformation, LifeCardGameGraphicsCP>//begin
    {
        public static readonly DependencyProperty ActionProperty = DependencyProperty.Register("Action", typeof(EnumAction), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(EnumAction.Unknown, new PropertyChangedCallback(ActionPropertyChanged)));
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
        private static void ActionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.Action = (EnumAction)e.NewValue;
        }
        public static readonly DependencyProperty RequirementProperty = DependencyProperty.Register("Requirement", typeof(EnumSpecialCardCategory), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(EnumSpecialCardCategory.Unknown, new PropertyChangedCallback(RequirementPropertyChanged)));
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
        private static void RequirementPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.Requirement = (EnumSpecialCardCategory)e.NewValue;
        }
        public static readonly DependencyProperty SpecialCategoryProperty = DependencyProperty.Register("SpecialCategory", typeof(EnumSpecialCardCategory), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(EnumSpecialCardCategory.Unknown, new PropertyChangedCallback(SpecialCategoryPropertyChanged)));
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
        private static void SpecialCategoryPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.SpecialCategory = (EnumSpecialCardCategory)e.NewValue;
        }

        public static readonly DependencyProperty FirstCardCategoryProperty = DependencyProperty.Register("FirstCardCategory", typeof(EnumFirstCardCategory), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(EnumFirstCardCategory.None, new PropertyChangedCallback(FirstCardCategoryPropertyChanged)));
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
        private static void FirstCardCategoryPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.FirstCategory = (EnumFirstCardCategory)e.NewValue;
        }
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register("Points", typeof(int), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(PointsPropertyChanged)));
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
        private static void PointsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.Points = (int)e.NewValue;
        }
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata("", new PropertyChangedCallback(DescriptionPropertyChanged)));
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
        private static void DescriptionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.Description = (string)e.NewValue;
        }
        public static readonly DependencyProperty OpponentKeepsCardProperty = DependencyProperty.Register("OpponentKeepsCard", typeof(bool), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OpponentKeepsCardPropertyChanged)));
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
        private static void OpponentKeepsCardPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.OpponentKeepsCard = (bool)e.NewValue;
        }
        public static readonly DependencyProperty SwitchCategoryProperty = DependencyProperty.Register("SwitchCategory", typeof(EnumSwitchCategory), typeof(CardGraphicsWPF), new FrameworkPropertyMetadata(EnumSwitchCategory.Unknown, new PropertyChangedCallback(SwitchCategoryPropertyChanged)));
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
        private static void SwitchCategoryPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardGraphicsWPF)sender;
            thisItem.MainObject!.SwitchCategory = (EnumSwitchCategory)e.NewValue;
        }//end
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
    public class LifeHandWPF : BaseHandWPF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsWPF>, ILifeScroll
    {
        void ILifeScroll.RecalculatePositioning()
        {
            RecalculatePositioning();
        }

        void ILifeScroll.ScrollToBottom()
        {
            ThisScroll!.ScrollToBottom();
        }
    }
}
