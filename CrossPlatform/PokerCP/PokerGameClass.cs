using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.ViewModelInterfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace PokerCP
{
    [SingletonGame]
    public class PokerGameClass : RegularDeckOfCardsGameClass<PokerCardInfo>
    {
        private struct PokerData
        {
            public string Message { get; set; }
            public decimal Mults { get; set; }
        }

        public PokerSaveInfo SaveRoot;

        private readonly ISaveSinglePlayerClass _thisState;

        internal readonly PokerViewModel ThisMod;

        internal bool GameGoing;

        private readonly DeckRegularDict<PokerCardInfo> _cardList = new DeckRegularDict<PokerCardInfo>();

        private decimal HowMuch
        {
            get
            {
                return ThisMod.BetAmount; // i think
            }
        }

        internal async Task NewRoundAsync()
        {
            ThisMod.IsRoundOver = false;
            ThisMod.Winnings = 0;
            ThisMod.HandLabel = "Nothing";
            ThisMod.PokerList.Clear();
            ThisMod.BetPlaced = false;
            ThisMod.Round++;
            _cardList.Clear();
            await base.NewGameAsync();
        }

        public PokerGameClass(ISoloCardGameVM<PokerCardInfo> thisMod) : base(thisMod)
        {
            _thisState = thisMod.MainContainer!.Resolve<ISaveSinglePlayerClass>();
            ThisMod = (PokerViewModel)thisMod;
            SaveRoot = new PokerSaveInfo();
        }
        public override async Task NewGameAsync()
        {
            GameGoing = true;
            ThisMod.Round = 0;
            ThisMod.Money = 200;
            await NewRoundAsync();
        }
        public override async Task<bool> CanOpenSavedSinglePlayerGameAsync()
        {
            return await _thisState.CanOpenSavedSinglePlayerGameAsync();
        }

        public override async Task OpenSavedGameAsync()
        {
            DeckList.OrderedObjects(); //i think
            SaveRoot = await _thisState.RetrieveSinglePlayerGameAsync<PokerSaveInfo>();
            if (SaveRoot.DeckList.Count > 0)
            {
                var newList = SaveRoot.DeckList.GetNewObjectListFromDeckList(DeckList);
                ThisMod.DeckPile!.OriginalList(newList);
            }
        }

        public async Task SaveStateAsync()
        {
            SaveRoot.DeckList = ThisMod.DeckPile!.GetCardIntegers();
            await _thisState.SaveSimpleSinglePlayerGameAsync(SaveRoot); //i think
        }

        public async Task ShowWinAsync()
        {
            await ThisMod.ShowGameMessageAsync("Congratulations, you won");
            ThisMod.NewGameVisible = true;
            GameGoing = false;
            await _thisState.DeleteSinglePlayerGameAsync(); //i think.
        }

        protected override void AfterShuffle()
        {
            //this is what runs after the cards shuffle.

        }
        internal void DrawFromDeck()
        {
            int maxs = ThisMod.SpotsToFill;
            bool canEndRound = ThisMod.BetPlaced;
            ThisMod.BetPlaced = true;
            var thisList = ThisMod.DeckPile!.DrawSeveralCards(maxs);
            ThisMod.PopulateNewCards(thisList);
            ProcessScores();
            ThisMod.IsRoundOver = canEndRound;
            if (canEndRound == false)
                return;
            ThisMod.Money += ThisMod.Winnings;
        }
        private void ProcessScores()
        {
            var tempList = ThisMod.GetCardList;
            _cardList.ReplaceRange(tempList);
            var poks = GetPokerInfo();
            ThisMod.HandLabel = poks.Message;
            ThisMod.Winnings = poks.Mults * HowMuch;
        }
        private PokerData GetPokerInfo()
        {
            PokerData output = new PokerData();
            ScoreInfo scores = new ScoreInfo();
            scores.CardList = _cardList.ToRegularDeckDict();
            if (scores.IsRoyalFlush())
            {
                output.Message = "Royal flush";
                output.Mults = 128;
                return output;
            }
            if (scores.IsStraightFlush())
            {
                output.Message = "Straight flush";
                output.Mults = 64;
                return output;
            }
            if (scores.Kinds(4))
            {
                output.Message = "Four of a kind";
                output.Mults = 32;
                return output;
            }
            if (scores.IsFullHouse())
            {
                output.Message = "Full house";
                output.Mults = 16;
                return output;
            }
            if (scores.IsFlush())
            {
                output.Message = "Flush";
                output.Mults = 8;
                return output;
            }
            if (scores.IsStraight())
            {
                output.Message = "Straight";
                output.Mults = 4;
                return output;
            }
            if (scores.Kinds(3))
            {
                output.Message = "Three of a kind";
                output.Mults = 2;
                return output;
            }
            if (scores.MultiPair())
            {
                output.Message = "Two pairs";
                output.Mults = 1;
                return output;
            }
            if (scores.Kinds(2))
            {
                output.Message = "Pair";
                output.Mults = 0;
                return output;
            }
            if (scores.HasAce())
            {
                output.Message = "High card ace";
                output.Mults = -0.5m;
                return output;
            }
            output.Message = "Nothing";
            output.Mults = -1;
            return output;
        }
    }
}