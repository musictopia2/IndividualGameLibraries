using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace FluxxCP
{
    public class FluxxPlayerItem : PlayerSingleHand<FluxxCardInformation>
    {
        public DeckObservableDict<KeeperCard> KeeperList { get; set; } = new DeckObservableDict<KeeperCard>();
        public override void HookUpHand()
        {
            base.HookUpHand();
            KeeperList.CollectionChanged += KeeperList_CollectionChanged;
        }
        private void KeeperList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NumberOfKeepers = KeeperList.Count;
            Bread = KeeperList.Any(items => items.Deck == EnumKeeper.Bread);
            Chocolate = KeeperList.Any(items => items.Deck == EnumKeeper.Chocolate);
            Cookies = KeeperList.Any(items => items.Deck == EnumKeeper.Cookies);
            Death = KeeperList.Any(items => items.Deck == EnumKeeper.Death);
            Dreams = KeeperList.Any(items => items.Deck == EnumKeeper.Dreams);
            Love = KeeperList.Any(items => items.Deck == EnumKeeper.Love);
            Milk = KeeperList.Any(items => items.Deck == EnumKeeper.Milk);
            Money = KeeperList.Any(items => items.Deck == EnumKeeper.Money);
            Peace = KeeperList.Any(items => items.Deck == EnumKeeper.Peace);
            Sleep = KeeperList.Any(items => items.Deck == EnumKeeper.Sleep);
            Television = KeeperList.Any(items => items.Deck == EnumKeeper.Television);
            TheBrain = KeeperList.Any(items => items.Deck == EnumKeeper.TheBrain);
            TheMoon = KeeperList.Any(items => items.Deck == EnumKeeper.TheMoon);
            TheRocket = KeeperList.Any(items => items.Deck == EnumKeeper.TheRocket);
            TheSun = KeeperList.Any(items => items.Deck == EnumKeeper.TheSun);
            TheToaster = KeeperList.Any(items => items.Deck == EnumKeeper.TheToaster);
            Time = KeeperList.Any(items => items.Deck == EnumKeeper.Time);
            War = KeeperList.Any(items => items.Deck == EnumKeeper.War);
        }
        private int _NumberOfKeepers;
        public int NumberOfKeepers
        {
            get
            {
                return _NumberOfKeepers;
            }

            set
            {
                if (SetProperty(ref _NumberOfKeepers, value) == true)
                {
                }
            }
        }
        private bool _Bread;
        public bool Bread
        {
            get
            {
                return _Bread;
            }

            set
            {
                if (SetProperty(ref _Bread, value) == true)
                {
                }
            }
        }
        private bool _Chocolate;
        public bool Chocolate
        {
            get
            {
                return _Chocolate;
            }

            set
            {
                if (SetProperty(ref _Chocolate, value) == true)
                {
                }
            }
        }
        private bool _Cookies;
        public bool Cookies
        {
            get
            {
                return _Cookies;
            }

            set
            {
                if (SetProperty(ref _Cookies, value) == true)
                {
                }
            }
        }
        private bool _Death;
        public bool Death
        {
            get
            {
                return _Death;
            }

            set
            {
                if (SetProperty(ref _Death, value) == true)
                {
                }
            }
        }
        private bool _Dreams;
        public bool Dreams
        {
            get
            {
                return _Dreams;
            }

            set
            {
                if (SetProperty(ref _Dreams, value) == true)
                {
                }
            }
        }
        private bool _Love;
        public bool Love
        {
            get
            {
                return _Love;
            }

            set
            {
                if (SetProperty(ref _Love, value) == true)
                {
                }
            }
        }
        private bool _Milk;
        public bool Milk
        {
            get
            {
                return _Milk;
            }

            set
            {
                if (SetProperty(ref _Milk, value) == true)
                {
                }
            }
        }
        private bool _Money;
        public bool Money
        {
            get
            {
                return _Money;
            }

            set
            {
                if (SetProperty(ref _Money, value) == true)
                {
                }
            }
        }
        private bool _Peace;
        public bool Peace
        {
            get
            {
                return _Peace;
            }

            set
            {
                if (SetProperty(ref _Peace, value) == true)
                {
                }
            }
        }
        private bool _Sleep;
        public bool Sleep
        {
            get
            {
                return _Sleep;
            }

            set
            {
                if (SetProperty(ref _Sleep, value) == true)
                {
                }
            }
        }
        private bool _Television;
        public bool Television
        {
            get
            {
                return _Television;
            }

            set
            {
                if (SetProperty(ref _Television, value) == true)
                {
                }
            }
        }
        private bool _TheBrain;
        public bool TheBrain
        {
            get
            {
                return _TheBrain;
            }

            set
            {
                if (SetProperty(ref _TheBrain, value) == true)
                {
                }
            }
        }
        private bool _TheMoon;
        public bool TheMoon
        {
            get
            {
                return _TheMoon;
            }

            set
            {
                if (SetProperty(ref _TheMoon, value) == true)
                {
                }
            }
        }
        private bool _TheRocket;
        public bool TheRocket
        {
            get
            {
                return _TheRocket;
            }

            set
            {
                if (SetProperty(ref _TheRocket, value) == true)
                {
                }
            }
        }
        private bool _TheSun;
        public bool TheSun
        {
            get
            {
                return _TheSun;
            }

            set
            {
                if (SetProperty(ref _TheSun, value) == true)
                {
                }
            }
        }
        private bool _TheToaster;
        public bool TheToaster
        {
            get
            {
                return _TheToaster;
            }

            set
            {
                if (SetProperty(ref _TheToaster, value) == true)
                {
                }
            }
        }
        private bool _Time;
        public bool Time
        {
            get
            {
                return _Time;
            }

            set
            {
                if (SetProperty(ref _Time, value) == true)
                {
                }
            }
        }
        private bool _War;
        public bool War
        {
            get
            {
                return _War;
            }

            set
            {
                if (SetProperty(ref _War, value) == true)
                {
                }
            }
        }
        private GlobalClass? _thisGlobal;
        public bool ObeyedRulesWhenNotYourTurn()
        {
            if (_thisGlobal == null)
                _thisGlobal = Resolve<GlobalClass>(); //this means i can't unit test it.  if necessary, rethink.
            var tempList = _thisGlobal.GetLimitList();
            if (tempList.Count > 2)
                throw new BasicBlankException("Can only have 2 types of limits when its not your turn");
            bool output = true; //has to be proven false.
            int count;
            tempList.ForEach(thisRule =>
            {
                count = thisRule.HowMany + _thisGlobal.IncreaseAmount();
                switch (thisRule.Category)
                {
                    case EnumRuleCategory.Hand:
                        if (MainHandList.Count > count)
                            output = false;
                        break;
                    case EnumRuleCategory.Keeper:
                        if (KeeperList.Count > count)
                            output = false;
                        break;
                    default:
                        throw new BasicBlankException("Can't find out whether its obeyed");
                }
            });
            return output;
        }
    }
}