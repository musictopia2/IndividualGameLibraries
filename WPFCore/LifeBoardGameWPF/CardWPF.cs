using BaseGPXWindowsAndControlsCore.GameGraphics.Base;
using LifeBoardGameCP;
using System.Windows;
using System.Windows.Data;
namespace LifeBoardGameWPF
{
    public class CardWPF : BaseDeckGraphicsWPF<LifeBaseCard, CardCP>
    {
        public static readonly DependencyProperty WhatTypeProperty = DependencyProperty.Register("WhatType", typeof(EnumCardCategory), typeof(CardWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(WhatTypePropertyChanged)));
        public EnumCardCategory WhatType
        {
            get
            {
                return (EnumCardCategory)GetValue(WhatTypeProperty);
            }
            set
            {
                SetValue(WhatTypeProperty, value);
            }
        }
        private static void WhatTypePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardWPF)sender;
            thisItem.MainObject!.CardCategory = (EnumCardCategory)e.NewValue;
        }
        public static readonly DependencyProperty CareerCategoryProperty = DependencyProperty.Register("CareerCategory", typeof(EnumCareerType), typeof(CardWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(CareerCategoryPropertyChanged)));
        public EnumCareerType CareerCategory
        {
            get
            {
                return (EnumCareerType)GetValue(CareerCategoryProperty);
            }
            set
            {
                SetValue(CareerCategoryProperty, value);
            }
        }
        private static void CareerCategoryPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardWPF)sender;
            thisItem.MainObject!.Career = (EnumCareerType)e.NewValue;
        }
        public static readonly DependencyProperty HouseCategoryProperty = DependencyProperty.Register("HouseCategory", typeof(EnumHouseType), typeof(CardWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(HouseCategoryPropertyChanged)));
        public EnumHouseType HouseCategory
        {
            get
            {
                return (EnumHouseType)GetValue(HouseCategoryProperty);
            }
            set
            {
                SetValue(HouseCategoryProperty, value);
            }
        }
        private static void HouseCategoryPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardWPF)sender;
            thisItem.MainObject!.House = (EnumHouseType)e.NewValue;
        }
        public static readonly DependencyProperty HouseValueProperty = DependencyProperty.Register("HouseValue", typeof(decimal), typeof(CardWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(HouseValuePropertyChanged)));
        public decimal HouseValue
        {
            get
            {
                return (decimal)GetValue(HouseValueProperty);
            }
            set
            {
                SetValue(HouseValueProperty, value);
            }
        }
        private static void HouseValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardWPF)sender;
            thisItem.MainObject!.HouseValue = (decimal)e.NewValue;
        }
        public static readonly DependencyProperty CollectAmountProperty = DependencyProperty.Register("CollectAmount", typeof(decimal), typeof(CardWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(CollectAmountPropertyChanged)));
        public decimal CollectAmount
        {
            get
            {
                return (decimal)GetValue(CollectAmountProperty);
            }
            set
            {
                SetValue(CollectAmountProperty, value);
            }
        }
        private static void CollectAmountPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardWPF)sender;
            thisItem.MainObject!.CollectAmount = (decimal)e.NewValue;
        }
        public static readonly DependencyProperty TaxesDueProperty = DependencyProperty.Register("TaxesDue", typeof(decimal), typeof(CardWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(TaxesDuePropertyChanged)));
        public decimal TaxesDue
        {
            get
            {
                return (decimal)GetValue(TaxesDueProperty);
            }
            set
            {
                SetValue(TaxesDueProperty, value);
            }
        }
        private static void TaxesDuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardWPF)sender;
            thisItem.MainObject!.TaxAmount = (decimal)e.NewValue;
        }
        public static readonly DependencyProperty StockValueProperty = DependencyProperty.Register("StockValue", typeof(int), typeof(CardWPF), new FrameworkPropertyMetadata(new PropertyChangedCallback(StockValuePropertyChanged)));
        public int StockValue
        {
            get
            {
                return (int)GetValue(StockValueProperty);
            }
            set
            {
                SetValue(StockValueProperty, value);
            }
        }
        private static void StockValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisItem = (CardWPF)sender;
            thisItem.MainObject!.StockValue = (int)e.NewValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(WhatTypeProperty, new Binding(nameof(LifeBaseCard.CardCategory)));
            SetBinding(CareerCategoryProperty, new Binding(nameof(CareerInfo.Career)));
            SetBinding(HouseCategoryProperty, new Binding(nameof(HouseInfo.HouseCategory)));
            SetBinding(HouseValueProperty, new Binding(nameof(HouseInfo.HousePrice)));
            SetBinding(CollectAmountProperty, new Binding(nameof(SalaryInfo.PayCheck)));
            SetBinding(TaxesDueProperty, new Binding(nameof(SalaryInfo.TaxesDue)));
            SetBinding(StockValueProperty, new Binding(nameof(StockInfo.Value)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
}