using BasicGameFramework.GameBoardCollections;
using BasicGameFramework.MiscProcesses;
using CommonBasicStandardLibraries.MVVMHelpers;
namespace BingoCP
{
    public class SpaceInfoCP : ObservableObject, IBasicSpace
    {
        private bool _IsEnabled = true; //if false, then headers.

        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                if (SetProperty(ref _IsEnabled, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        public Vector Vector { get; set; }
        private bool _AlreadyMarked;

        public bool AlreadyMarked
        {
            get { return _AlreadyMarked; }
            set
            {
                if (SetProperty(ref _AlreadyMarked, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private string _Text = "";

        public string Text
        {
            get { return _Text; }
            set
            {
                if (SetProperty(ref _Text, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        public void ClearSpace()
        {
            AlreadyMarked = false;
        }

        public bool IsFilled()
        {
            return AlreadyMarked;
        }
    }
}