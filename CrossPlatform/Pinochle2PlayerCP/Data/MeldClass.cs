using CommonBasicStandardLibraries.CollectionClasses;
namespace Pinochle2PlayerCP.Data
{
    public class MeldClass
    {
        public int Player { get; set; }
        public EnumClassA ClassAValue { get; set; } = EnumClassA.None;
        public EnumClassB ClassBValue { get; set; } = EnumClassB.None;
        public EnumClassC ClassCValue { get; set; } = EnumClassC.None;
        public CustomBasicList<int> CardList { get; set; } = new CustomBasicList<int>(); // this can link with the card
    }
}