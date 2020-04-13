using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using FluxxCP.Data;

namespace FluxxCP.Cards
{
    public class KeeperCard : FluxxCardInformation
    {
        public new EnumKeeper Deck
        {
            get
            {
                return (EnumKeeper)base.Deck;
            }
            set
            {
                base.Deck = (int)value;
            }
        }
        public KeeperCard()
        {
            CardType = EnumCardType.Keeper;
        }
        public override string Text()
        {

            return Deck.ToString().TextWithSpaces();
        }
        public override void Populate(int chosen)
        {
            Deck = (EnumKeeper)chosen;
            PopulateDescription();
        }
    }
}
