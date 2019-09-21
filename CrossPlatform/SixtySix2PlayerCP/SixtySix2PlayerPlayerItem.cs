using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
namespace SixtySix2PlayerCP
{
    public class SixtySix2PlayerPlayerItem : PlayerTrick<EnumSuitList, SixtySix2PlayerCardInformation>
    { //anything needed is here
        private int _ScoreRound;

        public int ScoreRound
        {
            get { return _ScoreRound; }
            set
            {
                if (SetProperty(ref _ScoreRound, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _GamePointsRound;

        public int GamePointsRound
        {
            get { return _GamePointsRound; }
            set
            {
                if (SetProperty(ref _GamePointsRound, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _GamePointsGame;

        public int GamePointsGame
        {
            get { return _GamePointsGame; }
            set
            {
                if (SetProperty(ref _GamePointsGame, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public EnumMarriage FirstMarriage { get; set; }
        public CustomBasicList<EnumMarriage> MarriageList { get; set; } = new CustomBasicList<EnumMarriage>();
    }
}