using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Collections.Specialized;
namespace RackoCP
{
    public class RackoPlayerItem : PlayerSingleHand<RackoCardInformation>
    { //anything needed is here
        private int _ScoreRound;
        public int ScoreRound
        {
            get
            {
                return _ScoreRound;
            }

            set
            {
                if (SetProperty(ref _ScoreRound, value) == true)
                {
                }
            }
        }
        private int _TotalScore;
        public int TotalScore
        {
            get
            {
                return _TotalScore;
            }

            set
            {
                if (SetProperty(ref _TotalScore, value) == true)
                {
                }
            }
        }
        private bool _CanShowValues; // this is used for binding for the visible whether the ui can show or not
        public bool CanShowValues
        {
            get
            {
                return _CanShowValues;
            }

            set
            {
                if (SetProperty(ref _CanShowValues, value) == true)
                {
                }
            }
        }
        private int _Value1;
        public int Value1
        {
            get
            {
                return _Value1;
            }

            set
            {
                if (SetProperty(ref _Value1, value) == true)
                {
                }
            }
        }
        private int _Value2;
        public int Value2
        {
            get
            {
                return _Value2;
            }

            set
            {
                if (SetProperty(ref _Value2, value) == true)
                {
                }
            }
        }
        private int _Value3;
        public int Value3
        {
            get
            {
                return _Value3;
            }

            set
            {
                if (SetProperty(ref _Value3, value) == true)
                {
                }
            }
        }
        private int _Value4;
        public int Value4
        {
            get
            {
                return _Value4;
            }

            set
            {
                if (SetProperty(ref _Value4, value) == true)
                {
                }
            }
        }
        private int _Value5;
        public int Value5
        {
            get
            {
                return _Value5;
            }

            set
            {
                if (SetProperty(ref _Value5, value) == true)
                {
                }
            }
        }
        private int _Value6;
        public int Value6
        {
            get
            {
                return _Value6;
            }

            set
            {
                if (SetProperty(ref _Value6, value) == true)
                {
                }
            }
        }
        private int _Value7;
        public int Value7
        {
            get
            {
                return _Value7;
            }

            set
            {
                if (SetProperty(ref _Value7, value) == true)
                {
                }
            }
        }
        private int _Value8;
        public int Value8
        {
            get
            {
                return _Value8;
            }

            set
            {
                if (SetProperty(ref _Value8, value) == true)
                {
                }
            }
        }
        private int _Value9;
        public int Value9
        {
            get
            {
                return _Value9;
            }

            set
            {
                if (SetProperty(ref _Value9, value) == true)
                {
                }
            }
        }
        private int _Value10;
        public int Value10
        {
            get
            {
                return _Value10;
            }

            set
            {
                if (SetProperty(ref _Value10, value) == true)
                {
                }
            }
        }
        public void UpdateAllValues()
        {
            if (MainHandList.Count != 10)
                throw new BasicBlankException("Must have 10 cards left in order to update values");
            int x;
            for (x = 0; x <= 9; x++)
                UpdateSingleValue(x);
        }
        private void UpdateSingleValue(int index)
        {
            var newValue = MainHandList[index].Value;
            switch (index)
            {
                case 0:
                    {
                        Value1 = newValue;
                        break;
                    }

                case 1:
                    {
                        Value2 = newValue;
                        break;
                    }

                case 2:
                    {
                        Value3 = newValue;
                        break;
                    }

                case 3:
                    {
                        Value4 = newValue;
                        break;
                    }

                case 4:
                    {
                        Value5 = newValue;
                        break;
                    }

                case 5:
                    {
                        Value6 = newValue;
                        break;
                    }

                case 6:
                    {
                        Value7 = newValue;
                        break;
                    }

                case 7:
                    {
                        Value8 = newValue;
                        break;
                    }

                case 8:
                    {
                        Value9 = newValue;
                        break;
                    }

                case 9:
                    {
                        Value10 = newValue;
                        break;
                    }

                default:
                    {
                        throw new BasicBlankException("Not Supported");
                    }
            }
        }
        protected override void OnCollectionChange(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (MainHandList.Count == 10)
                    UpdateAllValues();
                else
                {
                    Value1 = 0;
                    Value2 = 0;
                    Value3 = 0;
                    Value4 = 0;
                    Value5 = 0;
                    Value6 = 0;
                    Value7 = 0;
                    Value8 = 0;
                    Value9 = 0;
                    Value10 = 0;
                }
                return;
            }
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (e.OldItems.Count > 1)
                    throw new BasicBlankException("Not sure when there are more than one to replace");
                var Index = e.OldStartingIndex;
                UpdateSingleValue(Index);
            }
        }
    }
}