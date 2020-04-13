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
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
//i think this is the most common things i like to do
namespace ThreeLetterFunCP.Data
{
    public class ThreeLetterFunPlayerItem : PlayerSingleHand<ThreeLetterFunCardData>
    { //anything needed is here
        private int _cardsWon;

        public int CardsWon
        {
            get { return _cardsWon; }
            set
            {
                if (SetProperty(ref _cardsWon, value))
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
