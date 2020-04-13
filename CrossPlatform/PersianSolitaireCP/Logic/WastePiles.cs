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
using BasicGameFrameworkLibrary.SolitaireClasses.PileObservable;
using BasicGameFrameworkLibrary.CommandClasses;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using PersianSolitaireCP.Data;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SolitaireClasses.MiscClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.DataClasses;
using BasicGameFrameworkLibrary.DIContainers;
//i think this is the most common things i like to do
namespace PersianSolitaireCP.Logic
{
    public class WastePiles : WastePilesCP
    {
        private int DealNumber
        {
            get
            {
                return _model.DealNumber;
            }
            set
            {
                _model.DealNumber = value;
            }
        }

        private readonly ScoreModel _model;

        public WastePiles(CommandContainer command, 
            IEventAggregator aggregator,
            ScoreModel model
            ) : base(command, aggregator)
        {

            _model = model;
            //_model = (ScoreModel) temps;
            //_model.TestItem = 10;
            //temps.Score = 5;
        }
        public override void ClearBoard(IDeckDict<SolitaireCard> thisCol)
        {
            base.ClearBoard(thisCol);
            //below is custom code.
            int y = 0;
            8.Times(x =>
            {
                Piles.PileList.ForEach(thisPile =>
                {
                    var thisCard = thisCol[y];
                    thisPile.CardList.Add(thisCard);
                    y++;
                });
            });
        }
        public override bool CanAddSingleCard(int whichOne, SolitaireCard thisCard)
        {
            return false;
        }

        public override bool CanMoveCards(int whichOne, out int lastOne)
        {
            if (PreviousSelected == -1)
                throw new BasicBlankException("Cannot find out whether we can move the cards because none was selected");
            lastOne = -1; //until i figure out what else to do.
            var givList = Piles.ListGivenCards(PreviousSelected);

            TempList = givList.ListValidCardsAlternateColors();
            var thisPile = Piles.PileList[whichOne];
            SolitaireCard oldCard;
            if (thisPile.CardList.Count == 0)
            {
                lastOne = TempList.Count - 1;
                return true;
            }
            oldCard = Piles.GetLastCard(whichOne);
            if (oldCard.Value == EnumCardValueList.LowAce)
                return false;
            return TempList.CanMoveCardsAlternateColors(oldCard, ref lastOne);
        }

        public override bool CanMoveToAnotherPile(int whichOne)
        {
            return false;
        }
        public void Redeal()
        {
            TempList = new DeckRegularDict<SolitaireCard>();
            if (DealNumber == 3)
                throw new BasicBlankException("There are only 3 deals allowed.  Therefore, there is a problem");
            var thisCol = new DeckRegularDict<SolitaireCard>();
            DealNumber++;
            Piles.PileList.ForEach(thisPile =>
            {
                thisPile.CardList.ForEach(thisCard => thisCol.Add(thisCard));
            });
            Piles.ClearBoard();
            thisCol.ShuffleList();
            int y = 0;
            8.Times(x =>
            {
                foreach (var thisPile in Piles.PileList)
                {
                    if (y == thisCol.Count)
                        break;
                    var tempCard = thisCol[y];
                    thisPile.CardList.Add(tempCard);
                    y++;
                }
            });
            PreviousSelected = -1;
        }
    }
}
