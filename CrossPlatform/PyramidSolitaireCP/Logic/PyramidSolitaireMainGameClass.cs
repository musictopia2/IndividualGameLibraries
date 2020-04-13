using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using PyramidSolitaireCP.Data;
using PyramidSolitaireCP.EventModels;
using PyramidSolitaireCP.ViewModels;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace PyramidSolitaireCP.Logic
{
    [SingletonGame]
    public class PyramidSolitaireMainGameClass : RegularDeckOfCardsGameClass<SolitaireCard>, IAggregatorContainer
    {
        private readonly ISaveSinglePlayerClass _thisState;
        internal PyramidSolitaireSaveInfo SaveRoot { get; set; }
        internal bool GameGoing { get; set; }
        public PyramidSolitaireMainGameClass(ISaveSinglePlayerClass thisState,
            IEventAggregator aggregator,
            IGamePackageResolver container
            )
        {
            _thisState = thisState;
            Aggregator = aggregator;
            SaveRoot = container.ReplaceObject<PyramidSolitaireSaveInfo>(); //can't create new one.  because if doing that, then anything that needs it won't have it.
            SaveRoot.Load(Aggregator);
        }
        private PyramidSolitaireMainViewModel? _model;
        public Task NewGameAsync(PyramidSolitaireMainViewModel model)
        {
            GameGoing = true;
            _model = model;
            return base.NewGameAsync(model.DeckPile);
        }
        public override Task NewGameAsync(DeckObservablePile<SolitaireCard> deck)
        {
            throw new BasicBlankException("Wrong");
        }
        public IEventAggregator Aggregator { get; }

        public override async Task<bool> CanOpenSavedSinglePlayerGameAsync()
        {
            return await _thisState.CanOpenSavedSinglePlayerGameAsync();
        }

        public override Task OpenSavedGameAsync()
        {

            throw new BasicBlankException("Unable to open save game.  Rethink");

            //DeckList.OrderedObjects(); //i think
            //SaveRoot = await _thisState.RetrieveSinglePlayerGameAsync<PyramidSolitaireSaveInfo>();
            //if (SaveRoot.DeckList.Count > 0)
            //{
            //    var newList = SaveRoot.DeckList.GetNewObjectListFromDeckList(DeckList);
            //    DeckPile!.OriginalList(newList);
            //    //not sure if we need this or not (?)
            //    //DeckPile.Visible = true;
            //}
            //anything else that is needed to open the saved game will be here.

        }
        private bool _isBusy;
        public async Task SaveStateAsync()
        {
            if (_isBusy)
                return;
            _isBusy = true;
            SaveRoot.DeckList = DeckPile!.GetCardIntegers();
            await _thisState.SaveSimpleSinglePlayerGameAsync(SaveRoot); //i think
            _isBusy = false;
            //should be okay afterall (?)
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
            _model!.CurrentPile!.ClearCards();
            SaveRoot.Score = 0;
            _model.Discard!.ClearCards();
            var newList = _model.DeckPile!.DrawSeveralCards(28);
            _model.PlayList1!.ObjectList.Clear();
            _model.GameBoard1!.ClearCards(newList);
        }
        public async Task DrawCardAsync()
        {
            if (HasPlayedCard())
            {
                await UIPlatform.ShowMessageAsync("Sorry, you must choose to either play the cards or put them back before drawing another card");
                return;
            }
            bool wasEnd = DeckPile!.IsEndOfDeck();
            if (wasEnd)
            {
                var thisCol = _model!.Discard!.DiscardList();
                if (_model.Discard.PileEmpty() == false)
                    thisCol.Add(_model.Discard.GetCardInfo());
                if (_model.CurrentPile!.PileEmpty() == false)
                    thisCol.Add(_model.CurrentPile.GetCardInfo());
                _model.CurrentPile.ClearCards();
                _model.Discard.ClearCards();
                _model.DeckPile.OriginalList(thisCol);
            }
            var thisCard = _model!.DeckPile.DrawCard();
            if (_model.CurrentPile!.PileEmpty())
                _model.CurrentPile.AddCard(thisCard);
            else
            {
                var newCard = _model.CurrentPile.GetCardInfo();
                _model.Discard!.AddCard(newCard);
                _model.CurrentPile.AddCard(thisCard);
            }
            SaveRoot.RecentCard = _model.CurrentPile.GetCardInfo();
            if (wasEnd)
            {
                await Aggregator.PublishAsync(new PossibleNewGameEventModel());
            }
        }
        public bool HasPlayedCard() => _model!.PlayList1!.HasChosenCards();
        public bool IsValidMove()
        {
            var thisCol = _model!.PlayList1!.ObjectList;
            int maxs = thisCol.Sum(items => (int)items.Value);
            return maxs == 13;
        }
        public void PutBack()
        {
            var thisCol = _model!.PlayList1!.ObjectList;
            thisCol.ForEach(thisCard =>
            {
                if (_model.GameBoard1!.CanPutBack(thisCard.Deck) == false)
                {
                    if (thisCard.Deck == SaveRoot.RecentCard.Deck)
                        _model.CurrentPile!.AddCard(thisCard);
                    else
                        _model.Discard!.AddCard(thisCard);
                }
            });
            _model.GameBoard1!.PutBackAll();
            _model.PlayList1.RemoveCards();
        }
        public bool CanAddToPlay() => !_model!.PlayList1!.AlreadyHasTwoCards();
        public async Task PlayCardsAsync()
        {
            var thisCol = _model!.PlayList1!.ObjectList;
            SaveRoot.Score += thisCol.Count;
            if (_model.CurrentPile!.PileEmpty() == false)
                SaveRoot.RecentCard = _model.CurrentPile.GetCardInfo();
            _model.PlayList1.RemoveCards();
            _model.GameBoard1!.MakePermanant();
            if (SaveRoot.Score == 52)
            {
                await ShowWinAsync();
                return;
            }
        }


    }
}
