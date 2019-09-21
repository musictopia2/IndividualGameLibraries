using BasicGameFramework.GameBoardCollections;
using BasicGameFramework.MiscProcesses;
using CommonBasicStandardLibraries.MVVMHelpers;
namespace TicTacToeCP
{
    public enum EnumSpaceType
    {
        Blank, O, X
    }
    public class SpaceInfoCP : ObservableObject, IBasicSpace
    {
        public Vector Vector { get; set; }
        private EnumSpaceType _Status;
        public EnumSpaceType Status
        {
            get { return _Status; }
            set
            {
                if (SetProperty(ref _Status, value))
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