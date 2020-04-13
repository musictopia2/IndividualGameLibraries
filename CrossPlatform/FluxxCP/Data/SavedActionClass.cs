using CommonBasicStandardLibraries.CollectionClasses;
//i think this is the most common things i like to do
namespace FluxxCP.Data
{
    public class SavedActionClass
    {
        public int SelectedIndex { get; set; }
        public CustomBasicList<int> TempHandList { get; set; } = new CustomBasicList<int>(); // this is the list of cards remaining
        public CustomBasicList<PreviousCard> PreviousList { get; set; } = new CustomBasicList<PreviousCard>();
        public CustomBasicList<int> TempDiscardList { get; set; } = new CustomBasicList<int>();
    }

}