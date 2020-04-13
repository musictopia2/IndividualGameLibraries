using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using ClockSolitaireCP.Data;
using ClockSolitaireCP.ViewModels;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ClockSolitaireCP.Logic
{
    [SingletonGame]
    public class ClockSolitaireMainGameClass : RegularDeckOfCardsGameClass<SolitaireCard>, IAggregatorContainer
    {
        private readonly ISaveSinglePlayerClass _thisState;
        internal ClockSolitaireSaveInfo SaveRoot { get; set; }
        internal bool GameGoing { get; set; }
        public ClockSolitaireMainGameClass(ISaveSinglePlayerClass thisState,
            IEventAggregator aggregator,
            IGamePackageResolver container
            )
        {
            _thisState = thisState;
            Aggregator = aggregator;
            SaveRoot = container.ReplaceObject<ClockSolitaireSaveInfo>(); //can't create new one.  because if doing that, then anything that needs it won't have it.
            SaveRoot.LoadMod(aggregator);
        }
        private ClockBoard? _clock1;
        public async Task NewGameAsync(ClockSolitaireMainViewModel model)
        {
            GameGoing = true;
            _clock1 = model.Clock1;
            await base.NewGameAsync(model.DeckPile);
        }
        public override Task NewGameAsync(DeckObservablePile<SolitaireCard> deck)
        {
            throw new BasicBlankException("Must new new function");
        }
        public IEventAggregator Aggregator { get; }

        public override async Task<bool> CanOpenSavedSinglePlayerGameAsync()
        {
            return await _thisState.CanOpenSavedSinglePlayerGameAsync();
        }

        public override async Task OpenSavedGameAsync()
        {
            DeckList.OrderedObjects(); //i think
            SaveRoot = await _thisState.RetrieveSinglePlayerGameAsync<ClockSolitaireSaveInfo>();
            if (SaveRoot.DeckList.Count > 0)
            {
                var newList = SaveRoot.DeckList.GetNewObjectListFromDeckList(DeckList);
                DeckPile!.OriginalList(newList);
                //not sure if we need this or not (?)
                //DeckPile.Visible = true;
            }
            _clock1!.LoadSavedClocks(SaveRoot.SavedClocks);
            SaveRoot.LoadMod(Aggregator);
        }
        private bool _isBusy;
        public async Task SaveStateAsync()
        {
            if (_isBusy)
                return;
            _isBusy = true;
            SaveRoot.DeckList = DeckPile!.GetCardIntegers();
            _clock1!.SaveGame();
            await _thisState.SaveSimpleSinglePlayerGameAsync(SaveRoot); //i think
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
        public async Task ShowLossAsync()
        {
            await UIPlatform.ShowMessageAsync("Sorry, you lost");
            GameGoing = false;
            await this.SendGameOverAsync();
        }
        protected override void AfterShuffle()
        {
            //this is what runs after the cards shuffle.
            _clock1!.NewGame(DeckList.ToRegularDeckDict());
        }
    }
}