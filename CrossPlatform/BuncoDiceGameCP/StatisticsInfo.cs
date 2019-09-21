using CommonBasicStandardLibraries.MVVMHelpers;
namespace BuncoDiceGameCP
{
    public class StatisticsInfo : ObservableObject
    {
        private string _Turn = "None";
        public string Turn
        {
            get
            {
                return _Turn;
            }

            set
            {
                if (SetProperty(ref _Turn, value) == true)
                {
                }
            }
        }

        private int _NumberToGet = 0;
        public int NumberToGet
        {
            get
            {
                return _NumberToGet;
            }

            set
            {
                if (SetProperty(ref _NumberToGet, value) == true)
                {
                }
            }
        }

        private int _Set;
        public int Set
        {
            get
            {
                return _Set;
            }

            set
            {
                if (SetProperty(ref _Set, value) == true)
                {
                }
            }
        }

        private int _YourTeam;
        public int YourTeam
        {
            get
            {
                return _YourTeam;
            }

            set
            {
                if (SetProperty(ref _YourTeam, value) == true)
                {
                }
            }
        }

        private int _YourPoints;
        public int YourPoints
        {
            get
            {
                return _YourPoints;
            }

            set
            {
                if (SetProperty(ref _YourPoints, value) == true)
                {
                }
            }
        }

        private int _OpponentScore;
        public int OpponentScore
        {
            get
            {
                return _OpponentScore;
            }

            set
            {
                if (SetProperty(ref _OpponentScore, value) == true)
                {
                }
            }
        }

        private int _Buncos;
        public int Buncos
        {
            get
            {
                return _Buncos;
            }

            set
            {
                if (SetProperty(ref _Buncos, value) == true)
                {
                }
            }
        }

        private int _Wins;
        public int Wins
        {
            get
            {
                return _Wins;
            }

            set
            {
                if (SetProperty(ref _Wins, value) == true)
                {
                }
            }
        }

        private int _Losses;
        public int Losses
        {
            get
            {
                return _Losses;
            }

            set
            {
                if (SetProperty(ref _Losses, value) == true)
                {
                }
            }
        }

        private int _YourTable;
        public int YourTable
        {
            get
            {
                return _YourTable;
            }

            set
            {
                if (SetProperty(ref _YourTable, value) == true)
                {
                }
            }
        }

        private string _TeamMate = "None";
        public string TeamMate
        {
            get
            {
                return _TeamMate;
            }

            set
            {
                if (SetProperty(ref _TeamMate, value) == true)
                {
                }
            }
        }

        private string _Opponent1 = "None";
        public string Opponent1
        {
            get
            {
                return _Opponent1;
            }

            set
            {
                if (SetProperty(ref _Opponent1, value) == true)
                {
                }
            }
        }

        private string _Opponent2 = "None";
        public string Opponent2
        {
            get
            {
                return _Opponent2;
            }

            set
            {
                if (SetProperty(ref _Opponent2, value) == true)
                {
                }
            }
        }

        private string _Status = "Disconnected";
        public string Status
        {
            get
            {
                return _Status;
            }

            set
            {
                if (SetProperty(ref _Status, value) == true)
                {
                }
            }
        }
    }
}