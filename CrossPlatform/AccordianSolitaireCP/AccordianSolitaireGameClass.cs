using BasicGameFramework.Attributes;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.ViewModelInterfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace AccordianSolitaireCP
{
    [SingletonGame]
    public class AccordianSolitaireGameClass : RegularDeckOfCardsGameClass<AccordianSolitaireCardInfo>
    {
        public AccordianSolitaireSaveInfo SaveRoot;

        private readonly ISaveSinglePlayerClass _thisState;

        internal readonly AccordianSolitaireViewModel ThisMod;

        internal bool GameGoing;

        public AccordianSolitaireGameClass(ISoloCardGameVM<AccordianSolitaireCardInfo> thisMod) : base(thisMod)
        {
            _thisState = thisMod.MainContainer!.Resolve<ISaveSinglePlayerClass>();
            ThisMod = (AccordianSolitaireViewModel)thisMod;
            SaveRoot = new AccordianSolitaireSaveInfo();
            SaveRoot.LoadMod(ThisMod);
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
            SaveRoot = await _thisState.RetrieveSinglePlayerGameAsync<AccordianSolitaireSaveInfo>();
            if (SaveRoot.DeckList.Count > 0)
            {
                var newList = SaveRoot.DeckList.GetNewObjectListFromDeckList(DeckList);
                ThisMod.DeckPile!.OriginalList(newList);
                ThisMod.DeckPile.Visible = true;
            }
            //anything else that is needed to open the saved game will be here.
            SaveRoot.LoadMod(ThisMod);
            ThisMod.GameBoard1!.ReloadSavedGame();
        }
        private bool _isBusy;
        public async Task SaveStateAsync()
        {
            if (_isBusy)
                return;
            _isBusy = true;
            SaveRoot.DeckList = ThisMod.DeckPile!.GetCardIntegers();
            ThisMod.GameBoard1!.SaveGame();
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

        protected override void AfterShuffle()
        {
            //this is what runs after the cards shuffle.
            SaveRoot.Score = 1;
            var thisList = DeckList.ToRegularDeckDict();
            thisList.MakeAllObjectsKnown();
            ThisMod.GameBoard1!.NewGame(thisList);
        }
    }
}