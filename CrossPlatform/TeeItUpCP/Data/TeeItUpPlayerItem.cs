using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using Newtonsoft.Json;
using TeeItUpCP.Cards;
using TeeItUpCP.Logic;
namespace TeeItUpCP.Data
{
    public class TeeItUpPlayerItem : PlayerSingleHand<TeeItUpCardInformation>
    { //anything needed is here
        [JsonIgnore]
        public TeeItUpPlayerBoardCP? PlayerBoard;

        public bool FinishedChoosing { get; set; }
        private bool _wentOut;

        public bool WentOut
        {
            get { return _wentOut; }
            set
            {
                if (SetProperty(ref _wentOut, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _previousScore;

        public int PreviousScore
        {
            get { return _previousScore; }
            set
            {
                if (SetProperty(ref _previousScore, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _totalScore;

        public int TotalScore
        {
            get { return _totalScore; }
            set
            {
                if (SetProperty(ref _totalScore, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public void LoadPlayerBoard(TeeItUpGameContainer gameContainer)
        {
            PlayerBoard = new TeeItUpPlayerBoardCP(gameContainer);
            PlayerBoard.LoadBoard(this);
        }
    }
}
