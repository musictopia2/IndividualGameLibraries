namespace FluxxCP
{
    public partial class FluxxViewModel
    {
        private int _PlaysLeft;
        public int PlaysLeft
        {
            get
            {
                return _PlaysLeft;
            }

            set
            {
                if (SetProperty(ref _PlaysLeft, value) == true)
                {
                }
            }
        }
        private int _HandLimit;
        public int HandLimit
        {
            get
            {
                return _HandLimit;
            }

            set
            {
                if (SetProperty(ref _HandLimit, value) == true)
                {
                }
            }
        }
        private int _KeeperLimit;
        public int KeeperLimit
        {
            get
            {
                return _KeeperLimit;
            }

            set
            {
                if (SetProperty(ref _KeeperLimit, value) == true)
                {
                }
            }
        }
        private int _PlayLimit;
        public int PlayLimit
        {
            get
            {
                return _PlayLimit;
            }

            set
            {
                if (SetProperty(ref _PlayLimit, value) == true)
                {
                }
            }
        }
        private bool _AnotherTurn; // i think that otherturn is already built in.
        public bool AnotherTurn
        {
            get
            {
                return _AnotherTurn;
            }

            set
            {
                if (SetProperty(ref _AnotherTurn, value) == true)
                {
                }
            }
        }
        private int _DrawBonus;
        public int DrawBonus
        {
            get
            {
                return _DrawBonus;
            }

            set
            {
                if (SetProperty(ref _DrawBonus, value) == true)
                {
                }
            }
        }
        private int _PlayBonus;
        public int PlayBonus
        {
            get
            {
                return _PlayBonus;
            }

            set
            {
                if (SetProperty(ref _PlayBonus, value) == true)
                {
                }
            }
        }
        private int _CardsDrawn;
        public int CardsDrawn
        {
            get
            {
                return _CardsDrawn;
            }

            set
            {
                if (SetProperty(ref _CardsDrawn, value) == true)
                {
                }
            }
        }
        private int _DrawRules;
        public int DrawRules
        {
            get
            {
                return _DrawRules;
            }

            set
            {
                if (SetProperty(ref _DrawRules, value) == true)
                {
                }
            }
        }
        private int _PreviousBonus;
        public int PreviousBonus
        {
            get
            {
                return _PreviousBonus;
            }

            set
            {
                if (SetProperty(ref _PreviousBonus, value) == true)
                {
                }
            }
        }
        private int _CardsPlayed;
        public int CardsPlayed
        {
            get
            {
                return _CardsPlayed;
            }

            set
            {
                if (SetProperty(ref _CardsPlayed, value) == true)
                {
                }
            }
        }
    }
}