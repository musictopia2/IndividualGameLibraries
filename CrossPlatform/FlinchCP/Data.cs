using BasicGameFramework.SpecializedGameTypes.StockClasses;
using BasicGameFramework.ViewModelInterfaces;
namespace FlinchCP
{
    public enum EnumCardType
    {
        Discard = 1,
        Stock = 2,
        MyCards = 3,
        IsNone = 0
    }
    public enum EnumStatusList
    {
        DiscardAll = 1, // this means it needs to discard all (this means will not draw cards because need to discard all)
        Normal = 2, // this means normal play
        FirstOne = 3, // this means to either find a 1 or else pass (in this case, will draw the usual 5 cards)
        DiscardOneOnly = 4 // this means that a player has found a one.  all other players has to discard one (no drawing)
    }
    public class ComputerData
    {
        // this is the data the computer needs to make a decision
        // since in this game, the computer may play more than one card
        public EnumCardType WhichType { get; set; }
        public int Pile { get; set; } // this is the pile it will play
        public FlinchCardInformation? ThisCard { get; set; }
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
    public class StockViewModel : StockPileVM<FlinchCardInformation>
    {
        public StockViewModel(IBasicGameVM ThisMod) : base(ThisMod) { }

        public override string NextCardInStock()
        {
            if (DidGoOut() == true)
                return "0";
            var ThisCard = GetCard();
            return ThisCard.Display; // i think
        }
    }
    public static class GlobalConstants
    {
        public const int HowManyDiscards = 5;
    }
}