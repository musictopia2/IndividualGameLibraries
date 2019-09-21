using BasicGameFramework.GameBoardCollections;
using BasicGameFramework.MiscProcesses;
using CommonBasicStandardLibraries.MVVMHelpers;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace ConnectFourCP
{
    public class SpaceInfoCP : ObservableObject, IBasicSpace
    {
        public Vector Vector { get; set; }
        private int _Player;
        public int Player
        {
            get { return _Player; }
            set
            {
                if (SetProperty(ref _Player, value))
                {
                    //can decide what to do when property changes
                    OnPropertyChanged(nameof(HasImage));
                }
            }
        }
        private string _Color = cs.Transparent; //now we can specify default.
        public string Color
        {
            get { return _Color; }
            set
            {
                if (SetProperty(ref _Color, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public bool HasImage => Player > 0;
        public void ClearSpace()
        {
            Player = 0;
            Color = cs.Transparent;
        }
        public bool IsFilled()
        {
            return HasImage;
        }
    }
}