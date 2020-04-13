using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.Exceptions;
using RackoCP.Cards;
using System.Collections.Specialized;
namespace RackoCP.Data
{
    public class RackoPlayerItem : PlayerSingleHand<RackoCardInformation>
    { //anything needed is here
        private int _scoreRound;
        public int ScoreRound
        {
            get
            {
                return _scoreRound;
            }

            set
            {
                if (SetProperty(ref _scoreRound, value) == true)
                {
                }
            }
        }
        private int _totalScore;
        public int TotalScore
        {
            get
            {
                return _totalScore;
            }

            set
            {
                if (SetProperty(ref _totalScore, value) == true)
                {
                }
            }
        }
        private bool _canShowValues; // this is used for binding for the visible whether the ui can show or not
        public bool CanShowValues
        {
            get
            {
                return _canShowValues;
            }

            set
            {
                if (SetProperty(ref _canShowValues, value) == true)
                {
                }
            }
        }
        private int _value1;
        public int Value1
        {
            get
            {
                return _value1;
            }

            set
            {
                if (SetProperty(ref _value1, value) == true)
                {
                }
            }
        }
        private int _value2;
        public int Value2
        {
            get
            {
                return _value2;
            }

            set
            {
                if (SetProperty(ref _value2, value) == true)
                {
                }
            }
        }
        private int _value3;
        public int Value3
        {
            get
            {
                return _value3;
            }

            set
            {
                if (SetProperty(ref _value3, value) == true)
                {
                }
            }
        }
        private int _value4;
        public int Value4
        {
            get
            {
                return _value4;
            }

            set
            {
                if (SetProperty(ref _value4, value) == true)
                {
                }
            }
        }
        private int _value5;
        public int Value5
        {
            get
            {
                return _value5;
            }

            set
            {
                if (SetProperty(ref _value5, value) == true)
                {
                }
            }
        }
        private int _value6;
        public int Value6
        {
            get
            {
                return _value6;
            }

            set
            {
                if (SetProperty(ref _value6, value) == true)
                {
                }
            }
        }
        private int _value7;
        public int Value7
        {
            get
            {
                return _value7;
            }

            set
            {
                if (SetProperty(ref _value7, value) == true)
                {
                }
            }
        }
        private int _value8;
        public int Value8
        {
            get
            {
                return _value8;
            }

            set
            {
                if (SetProperty(ref _value8, value) == true)
                {
                }
            }
        }
        private int _value9;
        public int Value9
        {
            get
            {
                return _value9;
            }

            set
            {
                if (SetProperty(ref _value9, value) == true)
                {
                }
            }
        }
        private int _value10;

        

        public int Value10
        {
            get
            {
                return _value10;
            }

            set
            {
                if (SetProperty(ref _value10, value) == true)
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
