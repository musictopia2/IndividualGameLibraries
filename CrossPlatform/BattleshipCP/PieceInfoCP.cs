using CommonBasicStandardLibraries.MVVMHelpers;
namespace BattleshipCP
{
    public class PieceInfoCP : ObservableObject
    {
        private bool _DidHit;
        public bool DidHit
        {
            get
            {
                return _DidHit;
            }
            set
            {
                if (SetProperty(ref _DidHit, value) == true)
                {
                }
            }
        }
        private string _Location = "";
        public string Location
        {
            get
            {
                return _Location;
            }

            set
            {
                if (SetProperty(ref _Location, value) == true)
                {
                }
            }
        }
        private int _Index;
        public int Index
        {
            get
            {
                return _Index;
            }

            set
            {
                if (SetProperty(ref _Index, value) == true)
                {
                }
            }
        }
    }
}