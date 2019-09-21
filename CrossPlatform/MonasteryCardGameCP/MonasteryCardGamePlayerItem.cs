using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System.Linq;
namespace MonasteryCardGameCP
{
    public class MonasteryCardGamePlayerItem : PlayerSingleHand<MonasteryCardInfo>, IHandle<UpdateCountEventModel>
    { //anything needed is here
        public CustomBasicList<int> IndexList { get; set; } = new CustomBasicList<int>();

        private DeckRegularDict<MonasteryCardInfo> _AdditionalCards = new DeckRegularDict<MonasteryCardInfo>();

        public DeckRegularDict<MonasteryCardInfo> AdditionalCards
        {
            get { return _AdditionalCards; }
            set
            {
                if (SetProperty(ref _AdditionalCards, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _tempCards;
        protected override int GetTotalObjectCount => base.GetTotalObjectCount + _tempCards;
        public void Handle(UpdateCountEventModel message)
        {
            _tempCards = message.ObjectCount;
            ObjectCount = GetTotalObjectCount;
        }
        public void UpdateIndexes()
        {
            IndexList = Enumerable.Range(0, 9).ToCustomBasicList(); //i think this should now also show all not completed so for new game, still worsk.
            Mission1Completed = false;
            Mission2Completed = false;
            Mission3Completed = false;
            Mission4Completed = false;
            Mission5Completed = false;
            Mission6Completed = false;
            Mission7Completed = false;
            Mission8Completed = false;
            Mission9Completed = false;
            FinishedCurrentMission = false;
        }
        private bool _FinishedCurrentMission;
        public bool FinishedCurrentMission
        {
            get { return _FinishedCurrentMission; }
            set
            {
                if (SetProperty(ref _FinishedCurrentMission, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private void UpdateValue(int index, bool completed)
        {
            if (completed == false)
                return;
            if (IndexList.Contains(index))
                IndexList.RemoveSpecificItem(index);
        }
        public void CompleteMissionIndex(int index) //0 based
        {
            FinishedCurrentMission = true; // this will happen as well.
            switch (index)
            {
                case 0:
                    {
                        Mission1Completed = true;
                        break;
                    }

                case 1:
                    {
                        Mission2Completed = true;
                        break;
                    }

                case 2:
                    {
                        Mission3Completed = true;
                        break;
                    }

                case 3:
                    {
                        Mission4Completed = true;
                        break;
                    }

                case 4:
                    {
                        Mission5Completed = true;
                        break;
                    }

                case 5:
                    {
                        Mission6Completed = true;
                        break;
                    }

                case 6:
                    {
                        Mission7Completed = true;
                        break;
                    }

                case 7:
                    {
                        Mission8Completed = true;
                        break;
                    }

                case 8:
                    {
                        Mission9Completed = true;
                        break;
                    }

                default:
                    {
                        throw new BasicBlankException("Not Supported");
                    }
            }
        }
        private bool _Mission1Completed;
        public bool Mission1Completed
        {
            get
            {
                return _Mission1Completed;
            }

            set
            {
                if (SetProperty(ref _Mission1Completed, value) == true)
                    UpdateValue(0, value);
            }
        }
        private bool _Mission2Completed;
        public bool Mission2Completed
        {
            get
            {
                return _Mission2Completed;
            }

            set
            {
                if (SetProperty(ref _Mission2Completed, value) == true)
                    UpdateValue(1, value);
            }
        }
        private bool _Mission3Completed;
        public bool Mission3Completed
        {
            get
            {
                return _Mission3Completed;
            }

            set
            {
                if (SetProperty(ref _Mission3Completed, value) == true)
                    UpdateValue(2, value);
            }
        }
        private bool _Mission4Completed;
        public bool Mission4Completed
        {
            get
            {
                return _Mission4Completed;
            }

            set
            {
                if (SetProperty(ref _Mission4Completed, value) == true)
                    UpdateValue(3, value);
            }
        }

        private bool _Mission5Completed;
        public bool Mission5Completed
        {
            get
            {
                return _Mission5Completed;
            }

            set
            {
                if (SetProperty(ref _Mission5Completed, value) == true)
                    UpdateValue(4, value);
            }
        }

        private bool _Mission6Completed;
        public bool Mission6Completed
        {
            get
            {
                return _Mission6Completed;
            }

            set
            {
                if (SetProperty(ref _Mission6Completed, value) == true)
                    UpdateValue(5, value);
            }
        }
        private bool _Mission7Completed;
        public bool Mission7Completed
        {
            get
            {
                return _Mission7Completed;
            }

            set
            {
                if (SetProperty(ref _Mission7Completed, value) == true)
                    UpdateValue(6, value);
            }
        }
        private bool _Mission8Completed;
        public bool Mission8Completed
        {
            get
            {
                return _Mission8Completed;
            }

            set
            {
                if (SetProperty(ref _Mission8Completed, value) == true)
                    // code to run
                    UpdateValue(7, value);
            }
        }
        private bool _Mission9Completed;
        public bool Mission9Completed
        {
            get
            {
                return _Mission9Completed;
            }

            set
            {
                if (SetProperty(ref _Mission9Completed, value) == true)
                    // code to run
                    UpdateValue(8, value);
            }
        }
    }
}