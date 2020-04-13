using BasicGameFrameworkLibrary.Attributes;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
namespace MahJongSolitaireCP.Data
{
    [SingletonGame]
    public class MahJongSolitaireSaveInfo : ObservableObject, IMappable
    {
        private int _firstSelected;

        public int FirstSelected
        {
            get { return _firstSelected; }
            set
            {
                if (SetProperty(ref _firstSelected, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _tilesGone;

        public int TilesGone
        {
            get { return _tilesGone; }
            set
            {
                if (SetProperty(ref _tilesGone, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public CustomBasicList<BoardInfo> BoardList { get; set; } = new CustomBasicList<BoardInfo>(); //try this.
        public CustomBasicList<BoardInfo> PreviousList { get; set; } = new CustomBasicList<BoardInfo>(); //this is needed to support undo move.
    }
}