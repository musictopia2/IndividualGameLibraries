using BasicGameFramework.Attributes;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
namespace MahJongSolitaireCP
{
    [SingletonGame]
    public class MahJongSolitaireSaveInfo : ObservableObject
    {
        private int _FirstSelected;

        public int FirstSelected
        {
            get { return _FirstSelected; }
            set
            {
                if (SetProperty(ref _FirstSelected, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _TilesGone;

        public int TilesGone
        {
            get { return _TilesGone; }
            set
            {
                if (SetProperty(ref _TilesGone, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        //this no longer has access to the view models.
        //hopefully can get it to work.
        public CustomBasicList<BoardInfo> BoardList { get; set; } = new CustomBasicList<BoardInfo>(); //try this.
        public CustomBasicList<BoardInfo> PreviousList { get; set; } = new CustomBasicList<BoardInfo>(); //this is needed to support undo move.
    }
}