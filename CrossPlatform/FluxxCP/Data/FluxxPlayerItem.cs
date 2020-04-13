using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using FluxxCP.Cards;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using FluxxCP.Containers;
//i think this is the most common things i like to do
namespace FluxxCP.Data
{
    public class FluxxPlayerItem : PlayerSingleHand<FluxxCardInformation>
    { //anything needed is here
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
        private int _numberOfKeepers;
        public int NumberOfKeepers
        {
            get
            {
                return _numberOfKeepers;
            }

            set
            {
                if (SetProperty(ref _numberOfKeepers, value) == true)
                {
                }
            }
        }
        private bool _bread;
        public bool Bread
        {
            get
            {
                return _bread;
            }

            set
            {
                if (SetProperty(ref _bread, value) == true)
                {
                }
            }
        }
        private bool _chocolate;
        public bool Chocolate
        {
            get
            {
                return _chocolate;
            }

            set
            {
                if (SetProperty(ref _chocolate, value) == true)
                {
                }
            }
        }
        private bool _cookies;
        public bool Cookies
        {
            get
            {
                return _cookies;
            }

            set
            {
                if (SetProperty(ref _cookies, value) == true)
                {
                }
            }
        }
        private bool _death;
        public bool Death
        {
            get
            {
                return _death;
            }

            set
            {
                if (SetProperty(ref _death, value) == true)
                {
                }
            }
        }
        private bool _dreams;
        public bool Dreams
        {
            get
            {
                return _dreams;
            }

            set
            {
                if (SetProperty(ref _dreams, value) == true)
                {
                }
            }
        }
        private bool _love;
        public bool Love
        {
            get
            {
                return _love;
            }

            set
            {
                if (SetProperty(ref _love, value) == true)
                {
                }
            }
        }
        private bool _milk;
        public bool Milk
        {
            get
            {
                return _milk;
            }

            set
            {
                if (SetProperty(ref _milk, value) == true)
                {
                }
            }
        }
        private bool _money;
        public bool Money
        {
            get
            {
                return _money;
            }

            set
            {
                if (SetProperty(ref _money, value) == true)
                {
                }
            }
        }
        private bool _peace;
        public bool Peace
        {
            get
            {
                return _peace;
            }

            set
            {
                if (SetProperty(ref _peace, value) == true)
                {
                }
            }
        }
        private bool _sleep;
        public bool Sleep
        {
            get
            {
                return _sleep;
            }

            set
            {
                if (SetProperty(ref _sleep, value) == true)
                {
                }
            }
        }
        private bool _television;
        public bool Television
        {
            get
            {
                return _television;
            }

            set
            {
                if (SetProperty(ref _television, value) == true)
                {
                }
            }
        }
        private bool _theBrain;
        public bool TheBrain
        {
            get
            {
                return _theBrain;
            }

            set
            {
                if (SetProperty(ref _theBrain, value) == true)
                {
                }
            }
        }
        private bool _theMoon;
        public bool TheMoon
        {
            get
            {
                return _theMoon;
            }

            set
            {
                if (SetProperty(ref _theMoon, value) == true)
                {
                }
            }
        }
        private bool _theRocket;
        public bool TheRocket
        {
            get
            {
                return _theRocket;
            }

            set
            {
                if (SetProperty(ref _theRocket, value) == true)
                {
                }
            }
        }
        private bool _theSun;
        public bool TheSun
        {
            get
            {
                return _theSun;
            }

            set
            {
                if (SetProperty(ref _theSun, value) == true)
                {
                }
            }
        }
        private bool _theToaster;
        public bool TheToaster
        {
            get
            {
                return _theToaster;
            }

            set
            {
                if (SetProperty(ref _theToaster, value) == true)
                {
                }
            }
        }
        private bool _time;
        public bool Time
        {
            get
            {
                return _time;
            }

            set
            {
                if (SetProperty(ref _time, value) == true)
                {
                }
            }
        }
        private bool _war;
        public bool War
        {
            get
            {
                return _war;
            }

            set
            {
                if (SetProperty(ref _war, value) == true)
                {
                }
            }
        }
        private FluxxGameContainer? _gameContainer;
        public bool ObeyedRulesWhenNotYourTurn()
        {
            if (_gameContainer == null)
                _gameContainer = Resolve<FluxxGameContainer>(); //this means i can't unit test it.  if necessary, rethink.
            var tempList = _gameContainer.GetLimitList();
            if (tempList.Count > 2)
                throw new BasicBlankException("Can only have 2 types of limits when its not your turn");
            bool output = true; //has to be proven false.
            int count;
            tempList.ForEach(thisRule =>
            {
                count = thisRule.HowMany + _gameContainer.IncreaseAmount();
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
