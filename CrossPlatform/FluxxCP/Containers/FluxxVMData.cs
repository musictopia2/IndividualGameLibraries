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
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.CommandClasses;
using FluxxCP.Cards;
using FluxxCP.UICP;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGameFrameworkLibrary.CommonInterfaces;
//i think this is the most common things i like to do
namespace FluxxCP.Containers
{
    [SingletonGame]
    [AutoReset] //usually needs autoreset
    public class FluxxVMData : ObservableObject, IBasicCardGamesData<FluxxCardInformation>
    {

        public FluxxVMData(IEventAggregator aggregator, CommandContainer command, TestOptions test, IAsyncDelayer delayer)
        {
            Deck1 = new DeckObservablePile<FluxxCardInformation>(aggregator, command);
            Pile1 = new PileObservable<FluxxCardInformation>(aggregator, command);
            PlayerHand1 = new HandObservable<FluxxCardInformation>(command);
            Keeper1 = new HandObservable<KeeperCard>(command);
            Goal1 = new HandObservable<GoalCard>(command);
            Goal1.Text = "Goal Cards";
            Goal1.Maximum = 3;
            Goal1.AutoSelect = HandObservable<GoalCard>.EnumAutoType.SelectOneOnly;
            Keeper1.AutoSelect = HandObservable<KeeperCard>.EnumAutoType.SelectAsMany;
            Keeper1.Text = "Your Keepers";
            CardDetail = new DetailCardObservable();
            _test = test;
            _delayer = delayer;
        }
        //this should have a detail one as well that any can access as well (?)
        public DetailCardObservable CardDetail;
        public HandObservable<KeeperCard> Keeper1;
        public HandObservable<GoalCard> Goal1;
        public DeckObservablePile<FluxxCardInformation> Deck1 { get; set; }
        public PileObservable<FluxxCardInformation> Pile1 { get; set; }
        public HandObservable<FluxxCardInformation> PlayerHand1 { get; set; }
        public PileObservable<FluxxCardInformation>? OtherPile { get; set; }
        private string _normalTurn = "";
        [VM]
        public string NormalTurn
        {
            get { return _normalTurn; }
            set
            {
                if (SetProperty(ref _normalTurn, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        private string _status = "";
        [VM] //use this tag to transfer to the actual view model.  this is being done to avoid overflow errors.
        public string Status
        {
            get { return _status; }
            set
            {
                if (SetProperty(ref _status, value))
                {
                    //can decide what to do when property changes
                }

            }
        }


        private int _playsLeft;
        [VM]
        public int PlaysLeft
        {
            get
            {
                return _playsLeft;
            }

            set
            {
                if (SetProperty(ref _playsLeft, value) == true)
                {
                }
            }
        }
        private int _handLimit;
        [VM]
        public int HandLimit
        {
            get
            {
                return _handLimit;
            }

            set
            {
                if (SetProperty(ref _handLimit, value) == true)
                {
                }
            }
        }
        private int _keeperLimit;
        [VM]
        public int KeeperLimit
        {
            get
            {
                return _keeperLimit;
            }

            set
            {
                if (SetProperty(ref _keeperLimit, value) == true)
                {
                }
            }
        }
        private int _playLimit;
        [VM]
        public int PlayLimit
        {
            get
            {
                return _playLimit;
            }

            set
            {
                if (SetProperty(ref _playLimit, value) == true)
                {
                }
            }
        }
        private bool _anotherTurn; // i think that otherturn is already built in.
        [VM]
        public bool AnotherTurn
        {
            get
            {
                return _anotherTurn;
            }

            set
            {
                if (SetProperty(ref _anotherTurn, value) == true)
                {
                }
            }
        }
        private int _drawBonus;
        [VM]
        public int DrawBonus
        {
            get
            {
                return _drawBonus;
            }

            set
            {
                if (SetProperty(ref _drawBonus, value) == true)
                {
                }
            }
        }
        private int _playBonus;
        [VM]
        public int PlayBonus
        {
            get
            {
                return _playBonus;
            }

            set
            {
                if (SetProperty(ref _playBonus, value) == true)
                {
                }
            }
        }
        private int _cardsDrawn;
        [VM]
        public int CardsDrawn
        {
            get
            {
                return _cardsDrawn;
            }

            set
            {
                if (SetProperty(ref _cardsDrawn, value) == true)
                {
                }
            }
        }
        private int _drawRules;
        [VM]
        public int DrawRules
        {
            get
            {
                return _drawRules;
            }

            set
            {
                if (SetProperty(ref _drawRules, value) == true)
                {
                }
            }
        }
        private int _previousBonus;
        [VM]
        public int PreviousBonus
        {
            get
            {
                return _previousBonus;
            }

            set
            {
                if (SetProperty(ref _previousBonus, value) == true)
                {
                }
            }
        }
        private int _cardsPlayed;
        [VM]
        public int CardsPlayed
        {
            get
            {
                return _cardsPlayed;
            }

            set
            {
                if (SetProperty(ref _cardsPlayed, value) == true)
                {
                }
            }
        }

        private string _otherTurn = "";
        private readonly TestOptions _test;
        private readonly IAsyncDelayer _delayer;

        [VM]
        public string OtherTurn
        {
            get { return _otherTurn; }
            set
            {
                if (SetProperty(ref _otherTurn, value))
                {
                    //can decide what to do when property changes
                }
            }
        }

        public async Task ShowPlayCardAsync(FluxxCardInformation card)
        {
            if (card.Deck != CardDetail!.CurrentCard.Deck)
            {
                CardDetail.ShowCard(card);
                if (_test.NoAnimations == false)
                    await _delayer.DelaySeconds(1);
            }
            CardDetail.ResetCard();
        }

        internal void UnselectAllCards()
        {
            PlayerHand1!.UnselectAllObjects();
            Keeper1!.UnselectAllObjects();
            Goal1!.UnselectAllObjects();
        }

    }
}
