using BasicGameFrameworkLibrary.GameBoardCollections;
using BasicGameFrameworkLibrary.MiscProcesses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
namespace TicTacToeCP.Data
{
    public class SpaceInfoCP : ObservableObject, IBasicSpace
    {
        public Vector Vector { get; set; }
        private EnumSpaceType _status;
        public EnumSpaceType Status
        {
            get { return _status; }
            set
            {
                if (SetProperty(ref _status, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public void ClearSpace()
        {
            Status = EnumSpaceType.Blank;
        }
        public bool IsFilled()
        {
            return Status != EnumSpaceType.Blank;
        }
    }
}
