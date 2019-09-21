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
using BaseSolitaireClassesCP.PileInterfaces;
using BaseSolitaireClassesCP.Cards;
using BasicGameFramework.BasicDrawables.Dictionary;
using BaseSolitaireClassesCP.ClockClasses;
using BasicGameFramework.RegularDeckOfCards;

namespace GrandfathersClockCP
{
    public class CustomMain : ClockViewModel, IMain
    {
        private readonly GrandfathersClockViewModel _thisMod;
        public CustomMain(IClockVM thisMod) : base(thisMod)
        {
            _thisMod = (GrandfathersClockViewModel)thisMod;
            ShowCenter = false;
        }

        public int CardsNeededToBegin { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsRound { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }


        public event MainPileClickedEventHandler? PileSelectedAsync;



        public void SetSavedScore(int previousScore)
        {
            _thisMod.Score = previousScore;
        }


        public void AddCard(int pile, SolitaireCard thisCard)
        {
            _thisMod.Score++;
            AddCardToPile(pile, thisCard);
        }
        public new bool HasCard(int pile) => true;
        public void FirstLoad(bool needToMatch, bool showNextNeeded)
        {
            LoadBoard();
        }
        public int HowManyPiles()
        {
            return 12;
        }

        public bool CanAddCard(int pile, SolitaireCard thiscard)
        {
            var oldCard = GetLastCard(pile);
            if (thiscard.Suit != oldCard.Suit)
                return false;
            if (oldCard.Value + 1 == thiscard.Value)
                return true;
            return oldCard.Value == EnumCardValueList.King && thiscard.Value == EnumCardValueList.LowAce;
        }


        public int StartNumber()
        {
            return 0;
        }

        public void ClearBoard(IDeckDict<SolitaireCard> thisList)
        {
            if (thisList.Count != 12)
                throw new BasicBlankException("Must have 12 cards");
            ClearBoard();
            _thisMod.Score = thisList.Count;
            ClockList!.ForEach(thisClock =>
            {
                thisClock.CardList.ReplaceAllWithGivenItem(thisList.First());
                thisClock.IsEnabled = true;
                thisList.RemoveFirstItem();
            });
        }
        protected override async Task OnClockClickedAsync(int index)
        {
            if (PileSelectedAsync == null)
                return;
            await PileSelectedAsync.Invoke(index);

        }
        public async Task LoadGameAsync(string data)
        {
            CustomBasicList<ClockInfo> temps = await js.DeserializeObjectAsync<CustomBasicList<ClockInfo>>(data);
            LoadSavedClocks(temps);
        }

        public async Task<string> GetSavedPilesAsync()
        {
            CustomBasicList<ClockInfo> output = GetSavedClocks();
            return await js.SerializeObjectAsync(output);
        }

        public void AddCards(int Pile, IDeckDict<SolitaireCard> list)
        {
            throw new BasicBlankException("Can't Add Cards");
        }

        public void AddCards(IDeckDict<SolitaireCard> list)
        {
            throw new BasicBlankException("Can't Add Cards");
        }

    }
}
