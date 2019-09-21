using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace ThreeLetterFunCP
{
    public class ThreeLetterFunPlayerItem : PlayerSingleHand<ThreeLetterFunCardData>
    { //anything needed is here
        private int _CardsWon;

        public int CardsWon
        {
            get { return _CardsWon; }
            set
            {
                if (SetProperty(ref _CardsWon, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public bool TookTurn { get; set; }
        public int TimeToGetWord { get; set; } = -1; //-1 means did not get it.
        public CustomBasicList<TilePosition> TileList { get; set; } = new CustomBasicList<TilePosition>(); //this is the tiles used to get the word.  only send if you actually get a word.  the other players needs to know the positions as well.  before sending; will have for yourself
        public int CardUsed { get; set; }
        public int MostRecent { get; set; }
        public void ClearTurn()
        {
            PrivateClear(false);
        }

        private void PrivateClear(bool tookTurn)
        {
            this.TookTurn = tookTurn;
            TimeToGetWord = -1;
            TileList.Clear();
            CardUsed = 0;
        }
        public void GaveUp()
        {
            PrivateClear(true);
        }
    }
}