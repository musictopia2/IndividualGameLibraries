using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
namespace BuncoDiceGameCP
{
    public class PlayerItem : SimplePlayer
    {
        private int _Team;

        public int Team
        {
            get { return _Team; }
            set
            {
                if (SetProperty(ref _Team, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _Buncos;

        public int Buncos
        {
            get { return _Buncos; }
            set
            {
                if (SetProperty(ref _Buncos, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _Wins;

        public int Wins
        {
            get { return _Wins; }
            set
            {
                if (SetProperty(ref _Wins, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _Losses;

        public int Losses
        {
            get { return _Losses; }
            set
            {
                if (SetProperty(ref _Losses, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _PreviousMate;

        public int PreviousMate
        {
            get { return _PreviousMate; }
            set
            {
                if (SetProperty(ref _PreviousMate, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _Points;

        public int Points
        {
            get { return _Points; }
            set
            {
                if (SetProperty(ref _Points, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private bool _WonPrevious;

        public bool WonPrevious
        {
            get { return _WonPrevious; }
            set
            {
                if (SetProperty(ref _WonPrevious, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _Table;

        public int Table
        {
            get { return _Table; }
            set
            {
                if (SetProperty(ref _Table, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private bool _WinDetermined;

        public bool WinDetermined
        {
            get { return _WinDetermined; }
            set
            {
                if (SetProperty(ref _WinDetermined, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private bool _Acceptable;

        public bool Acceptable
        {
            get { return _Acceptable; }
            set
            {
                if (SetProperty(ref _Acceptable, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _PlayerNum;

        public int PlayerNum
        {
            get { return _PlayerNum; }
            set
            {
                if (SetProperty(ref _PlayerNum, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _TempTable;

        public int TempTable
        {
            get { return _TempTable; }
            set
            {
                if (SetProperty(ref _TempTable, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

    }
}