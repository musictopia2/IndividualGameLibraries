using BasicGamingUIXFLibrary.GameGraphics.Base;
using LifeBoardGameCP.Cards;
using LifeBoardGameCP.Data;
using LifeBoardGameCP.Graphics;
using Xamarin.Forms;
namespace LifeBoardGameXF
{
    public class CardXF : BaseDeckGraphicsXF<LifeBaseCard, CardCP>
    {
        public static readonly BindableProperty WhatTypeProperty = BindableProperty.Create(propertyName: "WhatType", returnType: typeof(EnumCardCategory), declaringType: typeof(CardXF), defaultValue: EnumCardCategory.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: WhatTypePropertyChanged);
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
        private static void WhatTypePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardXF)bindable;
            thisItem.MainObject!.CardCategory = (EnumCardCategory)newValue;
        }
        public static readonly BindableProperty CareerCategoryProperty = BindableProperty.Create(propertyName: "CareerCategory", returnType: typeof(EnumCareerType), declaringType: typeof(CardXF), defaultValue: EnumCareerType.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: CareerCategoryPropertyChanged);
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
        private static void CareerCategoryPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardXF)bindable;
            thisItem.MainObject!.Career = (EnumCareerType)newValue;
        }
        public static readonly BindableProperty HouseCategoryProperty = BindableProperty.Create(propertyName: "HouseCategory", returnType: typeof(EnumHouseType), declaringType: typeof(CardXF), defaultValue: EnumHouseType.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: HouseCategoryPropertyChanged);
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
        private static void HouseCategoryPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardXF)bindable;
            thisItem.MainObject!.House = (EnumHouseType)newValue;
        }
        public static readonly BindableProperty HouseValueProperty = BindableProperty.Create(propertyName: "HouseValue", returnType: typeof(decimal), declaringType: typeof(CardXF), defaultValue: 0.0m, defaultBindingMode: BindingMode.TwoWay, propertyChanged: HouseValuePropertyChanged);
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
        private static void HouseValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardXF)bindable;
            thisItem.MainObject!.HouseValue = (decimal)newValue;
        }
        public static readonly BindableProperty CollectAmountProperty = BindableProperty.Create(propertyName: "CollectAmount", returnType: typeof(decimal), declaringType: typeof(CardXF), defaultValue: 0.0m, defaultBindingMode: BindingMode.TwoWay, propertyChanged: CollectAmountPropertyChanged);
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
        private static void CollectAmountPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardXF)bindable;
            thisItem.MainObject!.CollectAmount = (decimal)newValue;
        }
        public static readonly BindableProperty TaxesDueProperty = BindableProperty.Create(propertyName: "TaxesDue", returnType: typeof(decimal), declaringType: typeof(CardXF), defaultValue: 0.0m, defaultBindingMode: BindingMode.TwoWay, propertyChanged: TaxesDuePropertyChanged);
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
        private static void TaxesDuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardXF)bindable;
            thisItem.MainObject!.TaxAmount = (decimal)newValue;
        }
        public static readonly BindableProperty StockValueProperty = BindableProperty.Create(propertyName: "StockValue", returnType: typeof(int), declaringType: typeof(CardXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: StockValuePropertyChanged);
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
        private static void StockValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardXF)bindable;
            thisItem.MainObject!.StockValue = (int)newValue;
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