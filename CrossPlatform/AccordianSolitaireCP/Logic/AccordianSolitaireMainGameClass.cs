using AccordianSolitaireCP.Data;
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace AccordianSolitaireCP.Logic
{
    [SingletonGame]
    public class AccordianSolitaireMainGameClass : RegularDeckOfCardsGameClass<AccordianSolitaireCardInfo>, IAggregatorContainer
    {
        private readonly ISaveSinglePlayerClass _thisState;
        internal AccordianSolitaireSaveInfo _saveRoot;
        internal bool GameGoing { get; set; }
        public AccordianSolitaireMainGameClass(ISaveSinglePlayerClass thisState,
            IEventAggregator aggregator,
            IGamePackageResolver container
            )
        {
            _thisState = thisState;
            Aggregator = aggregator;
            _saveRoot = container.ReplaceObject<AccordianSolitaireSaveInfo>(); //can't create new one.  because if doing that, then anything that needs it won't have it.
            _saveRoot.LoadMod(Aggregator);
        }
        private GameBoard? _board;
        public Task NewGameAsync(DeckObservablePile<AccordianSolitaireCardInfo> deck, GameBoard board)
        {
            GameGoing = true;
            _board = board;
            return base.NewGameAsync(deck);
        }

        public override Task NewGameAsync(DeckObservablePile<AccordianSolitaireCardInfo> deck)
        {
            throw new BasicBlankException("Needs to use new method for newgame to send in the gameboard");
        }
        public IEventAggregator Aggregator { get; }

        public override async Task<bool> CanOpenSavedSinglePlayerGameAsync()
        {
            return await _thisState.CanOpenSavedSinglePlayerGameAsync();
        }

        public override async Task OpenSavedGameAsync()
        {
            DeckList.OrderedObjects(); //i think
            _saveRoot = await _thisState.RetrieveSinglePlayerGameAsync<AccordianSolitaireSaveInfo>();
            if (_saveRoot.DeckList.Count > 0)
            {
                var newList = _saveRoot.DeckList.GetNewObjectListFromDeckList(DeckList);
                DeckPile!.OriginalList(newList);
                //not sure if we need this or not (?)
                //DeckPile.Visible = true;
            }
            //anything else that is needed to open the saved game will be here.
            _saveRoot.LoadMod(Aggregator);
            _board!.ReloadSavedGame(_saveRoot);
        }
        private bool _isBusy;
        public async Task SaveStateAsync()
        {
            if (_isBusy)
                return;
            _isBusy = true;
            _saveRoot.DeckList = DeckPile!.GetCardIntegers();
            _board!.SaveGame();
            await _thisState.SaveSimpleSinglePlayerGameAsync(_saveRoot); //i think
            _isBusy = false;
        }

        public async Task ShowWinAsync()
        {
            await UIPlatform.ShowMessageAsync("Congratulations, you won");
            GameGoing = false;
            await this.SendGameOverAsync();
            //ThisMod.NewGameVisible = true;
            //GameGoing = false;
            //await _thisState.DeleteSinglePlayerGameAsync(); //i think.
        }

        protected override void AfterShuffle()
        {
            //this is what runs after the cards shuffle.
            _saveRoot.Score = 1;
            var thisList = DeckList.ToRegularDeckDict();
            thisList.MakeAllObjectsKnown();
            _board!.NewGame(thisList, _saveRoot);
        }



    }
}
