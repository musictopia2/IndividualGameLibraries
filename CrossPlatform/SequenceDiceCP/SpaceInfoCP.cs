using BasicGameFramework.GameBoardCollections;
using BasicGameFramework.MiscProcesses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace SequenceDiceCP
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

        private bool _WasRecent;
        public bool WasRecent
        {
            get
            {
                return _WasRecent;
            }

            set
            {
                if (SetProperty(ref _WasRecent, value) == true)
                {
                }
            }
        }



        private int _Number;
        public int Number
        {
            get
            {
                return _Number;
            }

            set
            {
                if (SetProperty(ref _Number, value) == true)
                {
                }
            }
        }

        public void ClearSpace()
        {
            Player = 0;
            Color = cs.Transparent;
            WasRecent = false;
            if (Number == 0)
                throw new BasicBlankException("Number cannot be 0.  Rethink");
        }

        public bool IsFilled()
        {
            return Player > 0; //if player is put in, then its filled this time.
        }
    }
}