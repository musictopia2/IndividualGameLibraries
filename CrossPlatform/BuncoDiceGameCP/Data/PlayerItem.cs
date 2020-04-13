using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
namespace BuncoDiceGameCP.Data
{
    public class PlayerItem : SimplePlayer
    {
        private int _team;

        public int Team
        {
            get { return _team; }
            set
            {
                if (SetProperty(ref _team, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _buncos;

        public int Buncos
        {
            get { return _buncos; }
            set
            {
                if (SetProperty(ref _buncos, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _wins;

        public int Wins
        {
            get { return _wins; }
            set
            {
                if (SetProperty(ref _wins, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _losses;

        public int Losses
        {
            get { return _losses; }
            set
            {
                if (SetProperty(ref _losses, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _previousMate;

        public int PreviousMate
        {
            get { return _previousMate; }
            set
            {
                if (SetProperty(ref _previousMate, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _points;

        public int Points
        {
            get { return _points; }
            set
            {
                if (SetProperty(ref _points, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private bool _wonPrevious;

        public bool WonPrevious
        {
            get { return _wonPrevious; }
            set
            {
                if (SetProperty(ref _wonPrevious, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _table;

        public int Table
        {
            get { return _table; }
            set
            {
                if (SetProperty(ref _table, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private bool _winDetermined;

        public bool WinDetermined
        {
            get { return _winDetermined; }
            set
            {
                if (SetProperty(ref _winDetermined, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private bool _acceptable;

        public bool Acceptable
        {
            get { return _acceptable; }
            set
            {
                if (SetProperty(ref _acceptable, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _playerNum;

        public int PlayerNum
        {
            get { return _playerNum; }
            set
            {
                if (SetProperty(ref _playerNum, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _tempTable;

        public int TempTable
        {
            get { return _tempTable; }
            set
            {
                if (SetProperty(ref _tempTable, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

    }
}