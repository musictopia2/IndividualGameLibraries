using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using Newtonsoft.Json;
namespace TeeItUpCP
{
    public class TeeItUpPlayerItem : PlayerSingleHand<TeeItUpCardInformation>
    { //anything needed is here
        [JsonIgnore]
        public TeeItUpPlayerBoardCP? PlayerBoard;

        public bool FinishedChoosing { get; set; }
        private bool _WentOut;

        public bool WentOut
        {
            get { return _WentOut; }
            set
            {
                if (SetProperty(ref _WentOut, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _PreviousScore;

        public int PreviousScore
        {
            get { return _PreviousScore; }
            set
            {
                if (SetProperty(ref _PreviousScore, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _TotalScore;

        public int TotalScore
        {
            get { return _TotalScore; }
            set
            {
                if (SetProperty(ref _TotalScore, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public void LoadPlayerBoard(TeeItUpViewModel thisMod)
        {
            PlayerBoard = new TeeItUpPlayerBoardCP(thisMod);
            PlayerBoard.LoadBoard(this);
        }
    }
}