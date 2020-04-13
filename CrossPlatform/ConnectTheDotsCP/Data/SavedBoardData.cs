using CommonBasicStandardLibraries.CollectionClasses;
namespace ConnectTheDotsCP.Data
{
    public class SavedBoardData
    {
        public CustomBasicList<bool> LineList { get; set; } = new CustomBasicList<bool>(); // this represents the lines
        public CustomBasicList<bool> DotList { get; set; } = new CustomBasicList<bool>(); // to represent whether the dot was selected or not.
        public int PreviousColumn { get; set; }
        public int PreviousRow { get; set; }
        public int PreviousLine { get; set; }
        public CustomBasicList<int> SquarePlayerList { get; set; } = new CustomBasicList<int>();
    }
}