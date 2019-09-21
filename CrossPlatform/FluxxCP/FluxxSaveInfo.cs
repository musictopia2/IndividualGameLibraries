using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace FluxxCP
{
    [SingletonGame]
    public class FluxxSaveInfo : BasicSavedCardClass<FluxxPlayerItem, FluxxCardInformation> //hopefully using fluxcardinformation is okay.  otherwise, very iffy.
    {
        public DeckObservableDict<GoalCard> GoalList { get; set; } = new DeckObservableDict<GoalCard>();
        public DeckObservableDict<RuleCard> RuleList { get; set; } = new DeckObservableDict<RuleCard>();
        public CustomBasicList<int> DelayedPlayList { get; set; } = new CustomBasicList<int>();
        public bool DoAnalyze { get; set; }
        public CustomBasicList<int> QueList { get; set; } = new CustomBasicList<int>();
        public SavedActionClass SavedActionData { get; set; } = new SavedActionClass(); //forgot this part.
        public int CurrentAction { get; set; }
        private int _PlaysLeft;
        public int PlaysLeft
        {
            get { return _PlaysLeft; }
            set
            {
                if (SetProperty(ref _PlaysLeft, value))
                {
                    if (_thisMod != null)
                        _thisMod.PlaysLeft = value;
                }
            }
        }
        private int _HandLimit;
        public int HandLimit
        {
            get { return _HandLimit; }
            set
            {
                if (SetProperty(ref _HandLimit, value))
                {
                    if (_thisMod != null)
                        _thisMod.HandLimit = value;
                }
            }
        }
        private int _KeeperLimit;
        public int KeeperLimit
        {
            get { return _KeeperLimit; }
            set
            {
                if (SetProperty(ref _KeeperLimit, value))
                {
                    if (_thisMod != null)
                        _thisMod.KeeperLimit = value;
                }
            }
        }
        private int _PlayLimit;
        public int PlayLimit
        {
            get { return _PlayLimit; }
            set
            {
                if (SetProperty(ref _PlayLimit, value))
                {
                    if (_thisMod != null)
                        _thisMod.PlayLimit = value;
                }
            }
        }
        private bool _AnotherTurn;
        public bool AnotherTurn
        {
            get { return _AnotherTurn; }
            set
            {
                if (SetProperty(ref _AnotherTurn, value))
                {
                    if (_thisMod != null)
                        _thisMod.AnotherTurn = value;
                }
            }
        }
        private int _DrawBonus;
        public int DrawBonus
        {
            get { return _DrawBonus; }
            set
            {
                if (SetProperty(ref _DrawBonus, value))
                {
                    if (_thisMod != null)
                        _thisMod.DrawBonus = value;
                }
            }
        }
        private int _PlayBonus;
        public int PlayBonus
        {
            get { return _PlayBonus; }
            set
            {
                if (SetProperty(ref _PlayBonus, value))
                {
                    if (_thisMod != null)
                        _thisMod.PlayBonus = value;
                }
            }
        }
        private int _CardsDrawn;
        public int CardsDrawn
        {
            get { return _CardsDrawn; }
            set
            {
                if (SetProperty(ref _CardsDrawn, value))
                {
                    if (_thisMod != null)
                        _thisMod.CardsDrawn = value;
                }
            }
        }
        private int _CardsPlayed;
        public int CardsPlayed
        {
            get { return _CardsPlayed; }
            set
            {
                if (SetProperty(ref _CardsPlayed, value))
                {
                    if (_thisMod != null)
                        _thisMod.CardsPlayed = value;
                }
            }
        }
        private int _DrawRules;
        public int DrawRules
        {
            get { return _DrawRules; }
            set
            {
                if (SetProperty(ref _DrawRules, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null)
                        _thisMod.DrawRules = value;
                }
            }
        }
        private int _PreviousBonus;
        public int PreviousBonus
        {
            get { return _PreviousBonus; }
            set
            {
                if (SetProperty(ref _PreviousBonus, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null)
                        _thisMod.PreviousBonus = value;
                }
            }
        }
        private FluxxViewModel? _thisMod;
        internal void LoadMod(FluxxViewModel thisMod)
        {
            thisMod.PlaysLeft = PlaysLeft;
            thisMod.HandLimit = HandLimit;
            thisMod.KeeperLimit = KeeperLimit;
            thisMod.PlayLimit = PlayLimit;
            thisMod.AnotherTurn = AnotherTurn;
            thisMod.DrawBonus = DrawBonus;
            thisMod.PlayBonus = PlayBonus;
            thisMod.CardsDrawn = CardsDrawn;
            thisMod.CardsPlayed = CardsPlayed;
            thisMod.DrawRules = DrawRules;
            thisMod.PreviousBonus = PreviousBonus;
            _thisMod = thisMod;
        }
    }
}