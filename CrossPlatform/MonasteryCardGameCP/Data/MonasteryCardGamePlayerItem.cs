using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
namespace MonasteryCardGameCP.Data
{
    public class MonasteryCardGamePlayerItem : PlayerRummyHand<MonasteryCardInfo>
    { //anything needed is here
        public CustomBasicList<int> IndexList { get; set; } = new CustomBasicList<int>();


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
        private bool _finishedCurrentMission;
        public bool FinishedCurrentMission
        {
            get { return _finishedCurrentMission; }
            set
            {
                if (SetProperty(ref _finishedCurrentMission, value))
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
        private bool _mission1Completed;
        public bool Mission1Completed
        {
            get
            {
                return _mission1Completed;
            }

            set
            {
                if (SetProperty(ref _mission1Completed, value) == true)
                    UpdateValue(0, value);
            }
        }
        private bool _mission2Completed;
        public bool Mission2Completed
        {
            get
            {
                return _mission2Completed;
            }

            set
            {
                if (SetProperty(ref _mission2Completed, value) == true)
                    UpdateValue(1, value);
            }
        }
        private bool _mission3Completed;
        public bool Mission3Completed
        {
            get
            {
                return _mission3Completed;
            }

            set
            {
                if (SetProperty(ref _mission3Completed, value) == true)
                    UpdateValue(2, value);
            }
        }
        private bool _mission4Completed;
        public bool Mission4Completed
        {
            get
            {
                return _mission4Completed;
            }

            set
            {
                if (SetProperty(ref _mission4Completed, value) == true)
                    UpdateValue(3, value);
            }
        }

        private bool _mission5Completed;
        public bool Mission5Completed
        {
            get
            {
                return _mission5Completed;
            }

            set
            {
                if (SetProperty(ref _mission5Completed, value) == true)
                    UpdateValue(4, value);
            }
        }

        private bool _mission6Completed;
        public bool Mission6Completed
        {
            get
            {
                return _mission6Completed;
            }

            set
            {
                if (SetProperty(ref _mission6Completed, value) == true)
                    UpdateValue(5, value);
            }
        }
        private bool _mission7Completed;
        public bool Mission7Completed
        {
            get
            {
                return _mission7Completed;
            }

            set
            {
                if (SetProperty(ref _mission7Completed, value) == true)
                    UpdateValue(6, value);
            }
        }
        private bool _mission8Completed;
        public bool Mission8Completed
        {
            get
            {
                return _mission8Completed;
            }

            set
            {
                if (SetProperty(ref _mission8Completed, value) == true)
                    // code to run
                    UpdateValue(7, value);
            }
        }
        private bool _mission9Completed;
        public bool Mission9Completed
        {
            get
            {
                return _mission9Completed;
            }

            set
            {
                if (SetProperty(ref _mission9Completed, value) == true)
                    // code to run
                    UpdateValue(8, value);
            }
        }
    }
}
