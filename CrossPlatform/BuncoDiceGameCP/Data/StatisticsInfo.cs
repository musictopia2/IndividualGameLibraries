using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
namespace BuncoDiceGameCP.Data
{
    public class StatisticsInfo : ObservableObject
    {
        private string _turn = "None";
        public string Turn
        {
            get
            {
                return _turn;
            }

            set
            {
                if (SetProperty(ref _turn, value) == true)
                {
                }
            }
        }

        private int _numberToGet = 0;
        public int NumberToGet
        {
            get
            {
                return _numberToGet;
            }

            set
            {
                if (SetProperty(ref _numberToGet, value) == true)
                {
                }
            }
        }

        private int _set;
        public int Set
        {
            get
            {
                return _set;
            }

            set
            {
                if (SetProperty(ref _set, value) == true)
                {
                }
            }
        }

        private int _yourTeam;
        public int YourTeam
        {
            get
            {
                return _yourTeam;
            }

            set
            {
                if (SetProperty(ref _yourTeam, value) == true)
                {
                }
            }
        }

        private int _yourPoints;
        public int YourPoints
        {
            get
            {
                return _yourPoints;
            }

            set
            {
                if (SetProperty(ref _yourPoints, value) == true)
                {
                }
            }
        }

        private int _opponentScore;
        public int OpponentScore
        {
            get
            {
                return _opponentScore;
            }

            set
            {
                if (SetProperty(ref _opponentScore, value) == true)
                {
                }
            }
        }

        private int _buncos;
        public int Buncos
        {
            get
            {
                return _buncos;
            }

            set
            {
                if (SetProperty(ref _buncos, value) == true)
                {
                }
            }
        }

        private int _wins;
        public int Wins
        {
            get
            {
                return _wins;
            }

            set
            {
                if (SetProperty(ref _wins, value) == true)
                {
                }
            }
        }

        private int _losses;
        public int Losses
        {
            get
            {
                return _losses;
            }

            set
            {
                if (SetProperty(ref _losses, value) == true)
                {
                }
            }
        }

        private int _yourTable;
        public int YourTable
        {
            get
            {
                return _yourTable;
            }

            set
            {
                if (SetProperty(ref _yourTable, value) == true)
                {
                }
            }
        }

        private string _teamMate = "None";
        public string TeamMate
        {
            get
            {
                return _teamMate;
            }

            set
            {
                if (SetProperty(ref _teamMate, value) == true)
                {
                }
            }
        }

        private string _opponent1 = "None";
        public string Opponent1
        {
            get
            {
                return _opponent1;
            }

            set
            {
                if (SetProperty(ref _opponent1, value) == true)
                {
                }
            }
        }

        private string _opponent2 = "None";
        public string Opponent2
        {
            get
            {
                return _opponent2;
            }

            set
            {
                if (SetProperty(ref _opponent2, value) == true)
                {
                }
            }
        }

        private string _status = "Disconnected";
        public string Status
        {
            get
            {
                return _status;
            }

            set
            {
                if (SetProperty(ref _status, value) == true)
                {
                }
            }
        }
    }
}