using BasicGameFrameworkLibrary.GameBoardCollections;
using BasicGameFrameworkLibrary.MiscProcesses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;

namespace SequenceDiceCP.Data
{
    public class SpaceInfoCP : ObservableObject, IBasicSpace
    {
        public Vector Vector { get; set; }
        private int _player;

        public int Player
        {
            get { return _player; }
            set
            {
                if (SetProperty(ref _player, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private string _color = cs.Transparent; //now we can specify default.

        public string Color
        {
            get { return _color; }
            set
            {
                if (SetProperty(ref _color, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        private bool _wasRecent;
        public bool WasRecent
        {
            get
            {
                return _wasRecent;
            }

            set
            {
                if (SetProperty(ref _wasRecent, value) == true)
                {
                }
            }
        }



        private int _number;
        public int Number
        {
            get
            {
                return _number;
            }

            set
            {
                if (SetProperty(ref _number, value) == true)
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
            {
                throw new BasicBlankException("Number cannot be 0.  Rethink");
            }
        }

        public bool IsFilled()
        {
            return Player > 0; //if player is put in, then its filled this time.
        }
    }
}