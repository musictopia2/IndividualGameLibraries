using SkipboCP.Cards;

namespace SkipboCP.Data
{
    public class ComputerData
    {
        // this is the data the computer needs to make a decision
        // since in this game, the computer may play more than one card
        public EnumCardType WhichType { get; set; }
        public int Pile { get; set; } // this is the pile it will play
        public SkipboCardInformation? ThisCard { get; set; }
        public int Discard { get; set; } // this is if the computer will play from discard pile
    }
}