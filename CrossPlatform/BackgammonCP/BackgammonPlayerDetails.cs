using CommonBasicStandardLibraries.CollectionClasses;
namespace BackgammonCP
{
    public class BackgammonPlayerDetails
    {
        public CustomBasicList<int>? PiecesOnBoard { get; set; } //integers.  1 means beginning.  24 means end
        public int PiecesAtHome { get; set; }
        public int PiecesAtStart { get; set; }
    }
}