using BasicGameFramework.RegularDeckOfCards;
namespace ChinazoCP
{
    public class ChinazoCard : RegularRummyCard
    {
        protected override void PopulateAceValue()
        {
            Value = EnumCardValueList.HighAce; //this time, don't use interface.
        }
        public int UsedAs { get; set; } //this is needed so it can figure out what the card was used as.  only when playing as a run i think does it matter.
    }
}