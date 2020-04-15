using BasicGamingUIXFLibrary.GameGraphics.Base;
using TileRummyCP.Data;
using Xamarin.Forms;

namespace TileRummyXF
{
    public class TileXF : BaseDeckGraphicsXF<TileInfo, TileCP>
    {
        public static readonly BindableProperty WhatDrawProperty = BindableProperty.Create(propertyName: "WhatDraw", returnType: typeof(EnumDrawType), declaringType: typeof(TileXF), defaultValue: EnumDrawType.IsNone, defaultBindingMode: BindingMode.TwoWay, propertyChanged: WhatDrawPropertyChanged);
        public EnumDrawType WhatDraw
        {
            get
            {
                return (EnumDrawType)GetValue(WhatDrawProperty);
            }
            set
            {
                SetValue(WhatDrawProperty, value);
            }
        }
        private static void WhatDrawPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (TileXF)bindable;
            thisItem.MainObject!.WhatDraw = (EnumDrawType)newValue;
        }
        public static readonly BindableProperty ColorProperty = BindableProperty.Create(propertyName: "Color", returnType: typeof(EnumColorType), declaringType: typeof(TileXF), defaultValue: EnumColorType.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ColorPropertyChanged);
        public EnumColorType Color
        {
            get
            {
                return (EnumColorType)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }
        private static void ColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (TileXF)bindable;
            thisItem.MainObject!.Color = (EnumColorType)newValue;
        }
        public static readonly BindableProperty NumberProperty = BindableProperty.Create(propertyName: "Value", returnType: typeof(int), declaringType: typeof(TileXF), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ValuePropertyChanged);
        public int Number
        {
            get
            {
                return (int)GetValue(NumberProperty);
            }
            set
            {
                SetValue(NumberProperty, value);
            }
        }
        private static void ValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (TileXF)bindable;
            thisItem.MainObject!.Number = (int)newValue;
        }
        public static readonly BindableProperty IsJokerProperty = BindableProperty.Create(propertyName: "IsJoker", returnType: typeof(bool), declaringType: typeof(TileXF), defaultValue: false, defaultBindingMode: BindingMode.TwoWay, propertyChanged: IsJokerPropertyChanged);
        public bool IsJoker
        {
            get
            {
                return (bool)GetValue(IsJokerProperty);
            }
            set
            {
                SetValue(IsJokerProperty, value);
            }
        }
        private static void IsJokerPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (TileXF)bindable;
            thisItem.MainObject!.IsJoker = (bool)newValue;
        }
        protected override void DoCardBindings()
        {
            SetBinding(ColorProperty, new Binding(nameof(TileInfo.Color)));
            SetBinding(NumberProperty, new Binding(nameof(TileInfo.Number)));
            SetBinding(WhatDrawProperty, new Binding(nameof(TileInfo.WhatDraw)));
            SetBinding(IsJokerProperty, new Binding(nameof(TileInfo.IsJoker)));
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }

}
