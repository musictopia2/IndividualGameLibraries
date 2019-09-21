using BasicGameFramework.Attributes;
using BasicGameFramework.MiscProcesses;
using CommonBasicStandardLibraries.MVVMHelpers;
namespace SolitaireBoardGameCP
{
    [SingletonGame]
    public class SolitaireBoardGameSaveInfo : ObservableObject
    {
        private Vector _PreviousPiece;

        public Vector PreviousPiece
        {
            get { return _PreviousPiece; }
            set
            {
                if (SetProperty(ref _PreviousPiece, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public SolitaireBoardGameCollection SpaceList = new SolitaireBoardGameCollection();
    }
}