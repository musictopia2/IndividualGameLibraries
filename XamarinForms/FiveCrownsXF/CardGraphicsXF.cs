using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGamingUIXFLibrary.GameGraphics.Base;
using FiveCrownsCP.Cards;
using Xamarin.Forms;
using FiveCrownsCP.Data;

namespace FiveCrownsXF
{
    public class CardGraphicsXF : BaseDeckGraphicsXF<FiveCrownsCardInformation, FiveCrownsGraphicsCP>//begin
    {
        public static readonly BindableProperty CardValueProperty = BindableProperty.Create(propertyName: "CardValue", returnType: typeof(EnumCardValueList), declaringType: typeof(CardGraphicsXF), defaultValue: EnumCardValueList.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: CardValuePropertyChanged);
        public EnumCardValueList CardValue
        {
            get
            {
                return (EnumCardValueList)GetValue(CardValueProperty);
            }
            set
            {
                base.SetValue(CardValueProperty, value);
            }
        }
        private static void CardValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.CardValue = (EnumCardValueList)newValue;
        }

        public static readonly BindableProperty SuitProperty = BindableProperty.Create(propertyName: "Suit", returnType: typeof(EnumSuitList), declaringType: typeof(CardGraphicsXF), defaultValue: EnumSuitList.None, defaultBindingMode: BindingMode.TwoWay, propertyChanged: SuitPropertyChanged);
        public EnumSuitList Suit
        {
            get
            {
                return (EnumSuitList)GetValue(SuitProperty);
            }
            set
            {
                SetValue(SuitProperty, value);
            }
        }
        private static void SuitPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisItem = (CardGraphicsXF)bindable;
            thisItem.MainObject!.Suit = (EnumSuitList)newValue;
        }
        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
        protected override void DoCardBindings()
        {
            SetBinding(CardValueProperty, new Binding(nameof(FiveCrownsCardInformation.CardValue)));
            SetBinding(SuitProperty, new Binding(nameof(FiveCrownsCardInformation.Suit))); // i think
        }
    }
}
