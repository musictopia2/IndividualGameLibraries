using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MiscProcesses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using SolitaireBoardGameCP.Logic;
//i think this is the most common things i like to do
namespace SolitaireBoardGameCP.Data
{
    [SingletonGame]
    public class SolitaireBoardGameSaveInfo : ObservableObject, IMappable
    {
        private Vector _previousPiece;

        public Vector PreviousPiece
        {
            get { return _previousPiece; }
            set
            {
                if (SetProperty(ref _previousPiece, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public SolitaireBoardGameCollection SpaceList = new SolitaireBoardGameCollection();
    }
}