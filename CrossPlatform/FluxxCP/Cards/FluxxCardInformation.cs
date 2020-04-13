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
using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using SkiaSharp;
using FluxxCP.Data;
using FluxxCP.Containers;
//i think this is the most common things i like to do
namespace FluxxCP.Cards
{
    public class FluxxCardInformation : SimpleDeckObject, IDeckObject, IComparable<FluxxCardInformation>
    {
        public FluxxCardInformation()
        {
            DefaultSize = new SKSize(73, 113);
        }
        protected override void ChangeDeck()
        {
            Index = Deck;
        }
        public virtual void Populate(int chosen)
        {
            throw new BasicBlankException("You need the overrided versions to populate this");
        }
        public void Reset()
        {
            //anything that is needed to reset.
        }
        public virtual string Text()
        {
            throw new BasicBlankException("Needs to override if text is needed");
        }
        public EnumCardType CardType { get; set; }
        public string Description { get; set; } = "";
        public virtual bool IncreaseOne() => false;
        private int _index;
        public int Index
        {
            get { return _index; }
            set
            {
                if (SetProperty(ref _index, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        protected void PopulateDescription()
        {
            Description = FluxxGameContainer.DescriptionList[Deck - 1];
        }
        public bool CanDoCardAgain()
        {
            if (Deck == 1)
                return false;
            return CardType == EnumCardType.Action || CardType == EnumCardType.Rule; //this is default one
        }
        int IComparable<FluxxCardInformation>.CompareTo(FluxxCardInformation other)
        {
            return Deck.CompareTo(other.Deck); //maybe cardcategory does not matter after all.
        }
    }
}
