using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
using SixtySix2PlayerCP.Cards;
namespace SixtySix2PlayerCP.Data
{
    public class SixtySix2PlayerPlayerItem : PlayerTrick<EnumSuitList, SixtySix2PlayerCardInformation>
    { //anything needed is here
        private int _scoreRound;

        public int ScoreRound
        {
            get { return _scoreRound; }
            set
            {
                if (SetProperty(ref _scoreRound, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _gamePointsRound;

        public int GamePointsRound
        {
            get { return _gamePointsRound; }
            set
            {
                if (SetProperty(ref _gamePointsRound, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _gamePointsGame;

        public int GamePointsGame
        {
            get { return _gamePointsGame; }
            set
            {
                if (SetProperty(ref _gamePointsGame, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public EnumMarriage FirstMarriage { get; set; }
        public CustomBasicList<EnumMarriage> MarriageList { get; set; } = new CustomBasicList<EnumMarriage>();
    }
}
