using BaseSolitaireClassesCP.Cards;
using BasicGameFramework.Attributes;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.ViewModelInterfaces;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace TriangleSolitaireCP
{
    [SingletonGame]
    public class TriangleSolitaireGameClass : RegularDeckOfCardsGameClass<SolitaireCard>
    {
        private EnumIncreaseList Incs
        {
            set => SaveRoot.Incs = value;
            get => SaveRoot.Incs;
        }

        public TriangleSolitaireSaveInfo SaveRoot;

        private readonly ISaveSinglePlayerClass _thisState;

        internal readonly TriangleSolitaireViewModel ThisMod;

        internal bool GameGoing;

        public TriangleSolitaireGameClass(ISoloCardGameVM<SolitaireCard> thisMod) : base(thisMod)
        {
            _thisState = thisMod.MainContainer!.Resolve<ISaveSinglePlayerClass>();
            ThisMod = (TriangleSolitaireViewModel)thisMod;
            SaveRoot = new TriangleSolitaireSaveInfo();
        }
        public override Task NewGameAsync()
        {
            GameGoing = true;
            return base.NewGameAsync();
        }
        public override async Task<bool> CanOpenSavedSinglePlayerGameAsync()
        {
            return await _thisState.CanOpenSavedSinglePlayerGameAsync();
        }
        private bool _isBusy;

        public override async Task OpenSavedGameAsync()
        {
            DeckList.OrderedObjects(); //i think
            SaveRoot = await _thisState.RetrieveSinglePlayerGameAsync<TriangleSolitaireSaveInfo>();
            if (SaveRoot.DeckList.Count > 0)
            {
                var newList = SaveRoot.DeckList.GetNewObjectListFromDeckList(DeckList);
                ThisMod.DeckPile!.OriginalList(newList);
            }
            ThisMod.DeckPile!.Visible = true;
            //anything else that is needed to open the saved game will be here.
            ThisMod.Pile1!.SavedDiscardPiles(SaveRoot.PileData!);
            ThisMod.Triangle1!.LoadSavedTriangles(SaveRoot.TriangleData!);
        }

        public async Task SaveStateAsync()
        {
            if (_isBusy)
                return;
            _isBusy = true;
            SaveRoot.DeckList = ThisMod.DeckPile!.GetCardIntegers();
            SaveRoot.TriangleData = ThisMod.Triangle1!.GetSavedTriangles();
            SaveRoot.PileData = ThisMod.Pile1!.GetSavedPile();
            await _thisState.SaveSimpleSinglePlayerGameAsync(SaveRoot); //i think
            _isBusy = false;
        }
        internal async Task DeleteGameAsync()
        {
            await _thisState.DeleteSinglePlayerGameAsync();
        }
        public void DrawCard()
        {
            ThisMod.Pile1!.AddCard(ThisMod.DeckPile!.DrawCard());
            Incs = EnumIncreaseList.None;
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
            ThisMod.Pile1!.ClearCards();
            ThisMod.NewGameVisible = false; //you have to go through the deck at least one.
            Incs = EnumIncreaseList.None;
            var newList = ThisMod.DeckPile!.DrawSeveralCards(15);
            ThisMod.Triangle1!.ClearCards(newList);
            DrawCard();
        }
        public bool IsValidMove(SolitaireCard controlCard, SolitaireCard pileCard, out EnumIncreaseList tempi)
        {
            tempi = EnumIncreaseList.None;
            if (controlCard.Color == pileCard.Color)
                return false; //has to be opposite colors.
            if (controlCard.Value == pileCard.Value)
                return false; //can't be the same number either.
            bool ispileAce = false;
            if (pileCard.Value == EnumCardValueList.LowAce || pileCard.Value == EnumCardValueList.HighAce)
            {
                ispileAce = true;
                tempi = Incs;
                if (Incs == EnumIncreaseList.Increase)
                    return controlCard.Value == EnumCardValueList.King;
                if (Incs == EnumIncreaseList.Decrease)
                    return controlCard.Value == EnumCardValueList.Two;
            }
            if (controlCard.Value == EnumCardValueList.King && ispileAce)
            {
                Incs = EnumIncreaseList.Increase;
                return true;
            }
            if (controlCard.Value == EnumCardValueList.Two && ispileAce)
            {
                Incs = EnumIncreaseList.Decrease;
                return true;
            }
            bool isControlAce = controlCard.Value == EnumCardValueList.LowAce || controlCard.Value == EnumCardValueList.HighAce;
            if (isControlAce)
            {
                if (pileCard.Value == EnumCardValueList.King)
                {
                    Incs = EnumIncreaseList.Decrease;
                    return true;
                }
                if (pileCard.Value == EnumCardValueList.Two)
                {
                    Incs = EnumIncreaseList.Increase;
                    return true;
                }
                Incs = EnumIncreaseList.None;
                return false;
            }
            if (controlCard.Value + 1 == pileCard.Value)
            {
                Incs = EnumIncreaseList.Increase;
                return true;
            }
            Incs = EnumIncreaseList.Decrease;
            return controlCard.Value - 1 == pileCard.Value;
        }
        public async Task MakeMoveAsync(int whichOne, EnumIncreaseList tempi)
        {
            Incs = tempi;
            var thisCard = ThisMod.Triangle1!.CardList.Single(items => items.Deck == whichOne);
            var pileCard = ThisMod.Pile1!.GetCardInfo();
            ThisMod.Triangle1.MakeInvisible(whichOne);
            var newCard = new SolitaireCard();
            newCard.Populate(thisCard.Deck);
            newCard.Visible = true;
            ThisMod.Pile1.AddCard(newCard);
            if (ThisMod.Triangle1.HowManyCardsLeft == 0)
                await ShowWinAsync();
        }
    }
}