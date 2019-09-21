using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using System;
namespace FluxxCP
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
        private int _Index;
        public int Index
        {
            get { return _Index; }
            set
            {
                if (SetProperty(ref _Index, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        protected void PopulateDescription()
        {
            Description = GlobalClass.DescriptionList[Deck - 1];
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