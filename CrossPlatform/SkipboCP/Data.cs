using BasicGameFramework.SpecializedGameTypes.StockClasses;
using BasicGameFramework.ViewModelInterfaces;
namespace SkipboCP
{
    public enum EnumCardType
    {
        Discard = 1,
        Stock = 2,
        MyCards = 3,
        IsNone = 0
    }
    public class ComputerData
    {
        // this is the data the computer needs to make a decision
        // since in this game, the computer may play more than one card
        public EnumCardType WhichType { get; set; }
        public int Pile { get; set; } // this is the pile it will play
        public SkipboCardInformation? ThisCard { get; set; }
        public int Discard { get; set; } // this is if the computer will play from discard pile
    }

    public class SendPlay : SendDiscard
    {
        public int Discard { get; set; }
        public EnumCardType WhichType { get; set; }
    }

    public class SendDiscard
    {
        public int Pile { get; set; }
        public int Deck { get; set; }
    }

    public class StockViewModel : StockPileVM<SkipboCardInformation>
    {
        public StockViewModel(IBasicGameVM thisMod) : base(thisMod) { }

        public override string NextCardInStock()
        {
            if (DidGoOut() == true)
                return "0";
            var thisCard = GetCard();
            return thisCard.Display; // i think
        }
    }
}