using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace HeapSolitaireCP
{
    [SingletonGame]
    public class HeapSolitaireGameClass : RegularDeckOfCardsGameClass<HeapSolitaireCardInfo>
    {
        public HeapSolitaireSaveInfo SaveRoot;
        private readonly ISaveSinglePlayerClass _thisState;
        internal readonly HeapSolitaireViewModel ThisMod;
        internal bool GameGoing;
        private readonly RandomGenerator _rs;
        public HeapSolitaireGameClass(ISoloCardGameVM<HeapSolitaireCardInfo> thisMod) : base(thisMod)
        {
            _thisState = thisMod.MainContainer!.Resolve<ISaveSinglePlayerClass>();
            ThisMod = (HeapSolitaireViewModel)thisMod;
            SaveRoot = new HeapSolitaireSaveInfo();
            SaveRoot.LoadMod(ThisMod);
            _rs = ThisMod.MainContainer!.Resolve<RandomGenerator>();
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
        public override async Task OpenSavedGameAsync()
        {
            DeckList.OrderedObjects(); //i think
            SaveRoot = await _thisState.RetrieveSinglePlayerGameAsync<HeapSolitaireSaveInfo>();
            if (SaveRoot.DeckList.Count > 0)
            {
                var newList = SaveRoot.DeckList.GetNewObjectListFromDeckList(DeckList);
                ThisMod.DeckPile!.OriginalList(newList);
                ThisMod.DeckPile.Visible = true;
            }
            ThisMod.Main1!.PileList!.ReplaceRange(SaveRoot.MainPiles);
            ThisMod.Main1.RefreshInfo();
            SaveRoot.LoadMod(ThisMod);
            ThisMod.Waste1!.PileList!.ReplaceRange(SaveRoot.WasteData);
        }
        private bool _isBusy;
        public async Task SaveStateAsync()
        {
            if (_isBusy)
                return;
            _isBusy = true;
            SaveRoot.DeckList = ThisMod.DeckPile!.GetCardIntegers();
            SaveRoot.MainPiles = ThisMod.Main1!.PileList.ToCustomBasicList();
            SaveRoot.WasteData = ThisMod.Waste1!.PileList.ToCustomBasicList();
            await _thisState.SaveSimpleSinglePlayerGameAsync(SaveRoot); //i think
            _isBusy = false;
        }
        public async Task ShowWinAsync()
        {
            await ThisMod.ShowGameMessageAsync("Congratulations, you won");
            ThisMod.NewGameVisible = true;
            GameGoing = false;
            await _thisState.DeleteSinglePlayerGameAsync(); //i think.
        }
        private DeckRegularDict<HeapSolitaireCardInfo> _cardList = new DeckRegularDict<HeapSolitaireCardInfo>();
        private DeckRegularDict<HeapSolitaireCardInfo> GetFirstList()
        {
            DeckRegularDict<HeapSolitaireCardInfo> output = new DeckRegularDict<HeapSolitaireCardInfo>();
            EnumColorList newColor = (EnumColorList)_rs.GetRandomNumber(2);
            int newNumber = 7;
            do
            {
                if (output.Count < 13)
                {
                    if (newColor == EnumColorList.Black)
                        newColor = EnumColorList.Red;
                    else
                        newColor = EnumColorList.Black;
                    var finalCard = _cardList.First(items => (int)items.Value == newNumber && items.Color == newColor);
                    output.Add(finalCard);
                    _cardList.RemoveSpecificItem(finalCard);
                    if (output.Count == 13)
                        return output;
                    newNumber++;
                    if (newNumber > 13)
                        newNumber = 1;
                }
            } while (true);
        }
        protected override void AfterShuffle()
        {
            _cardList = DeckList.ToRegularDeckDict();
            _cardList.ForEach(thisCard => thisCard.IsUnknown = false);
            var firstCol = GetFirstList();
            SaveRoot.Score = 13;
            ThisMod.Main1!.ClearBoard(firstCol);
            ThisMod.Waste1!.ClearBoard(_cardList); //hopefully no problem this time (?)
        }
        private bool IsValidMove(int whichOne, out HeapSolitaireCardInfo thisCard)
        {
            var prevCard = ThisMod.Main1!.GetLastCard(whichOne);
            var newCard = ThisMod.Waste1!.GetCard();
            thisCard = newCard;
            if (whichOne == 12 && prevCard.Color == newCard.Color)
                return false;
            else if (prevCard.Color != newCard.Color && whichOne != 12)
                return false;
            if (prevCard.Value == EnumCardValueList.King && newCard.Value == EnumCardValueList.LowAce)
                return true;
            if (prevCard.Value == EnumCardValueList.King)
                return false;
            return prevCard.Value + 1 == newCard.Value;
        }
        public async Task SelectMainAsync(int whichOne)
        {
            if (ThisMod.Waste1!.DidSelectCard == false)
            {
                await ThisMod.ShowGameMessageAsync("Sorry, you must select a card to put to the main one");
                return;
            }
            if (IsValidMove(whichOne, out HeapSolitaireCardInfo thisCard) == false)
            {
                await ThisMod.ShowGameMessageAsync("Illegal Move");
                return;
            }
            ThisMod.Main1!.AddCardToPile(whichOne, thisCard);
            ThisMod.Waste1.RemoveCardFromPile(SaveRoot.PreviousSelected);

            SaveRoot.PreviousSelected = -1;
            if (SaveRoot.Score == 104)
                await ShowWinAsync();
        }
    }
}
