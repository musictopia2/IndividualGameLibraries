using BasicGameFramework.Attributes;
using BasicGameFramework.MiscProcesses;
using CommonBasicStandardLibraries.MVVMHelpers;
namespace XPuzzleCP
{
    [SingletonGame]
    public class XPuzzleSaveInfo : ObservableObject // needs to be observable for bindings
    {

        private Vector _PreviousOpen; //has to make it a vector now.

        public Vector PreviousOpen
        {
            get { return _PreviousOpen; }
            set
            {
                if (SetProperty(ref _PreviousOpen, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public XPuzzleCollection SpaceList = new XPuzzleCollection();
    }
}
