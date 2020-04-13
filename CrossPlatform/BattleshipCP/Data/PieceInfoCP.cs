using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
namespace BattleshipCP.Data
{
    public class PieceInfoCP : ObservableObject
    {
        private bool _didHit;
        public bool DidHit
        {
            get
            {
                return _didHit;
            }
            set
            {
                if (SetProperty(ref _didHit, value) == true)
                {
                }
            }
        }
        private string _location = "";
        public string Location
        {
            get
            {
                return _location;
            }

            set
            {
                if (SetProperty(ref _location, value) == true)
                {
                }
            }
        }
        private int _index;
        public int Index
        {
            get
            {
                return _index;
            }

            set
            {
                if (SetProperty(ref _index, value) == true)
                {
                }
            }
        }
    }
}
