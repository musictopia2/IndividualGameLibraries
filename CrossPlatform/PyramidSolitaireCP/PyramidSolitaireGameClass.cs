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
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.ViewModelInterfaces;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Extensions;
using BasicGameFramework.Attributes;
using BaseSolitaireClassesCP.Cards;

namespace PyramidSolitaireCP
{
    [SingletonGame]
    public class PyramidSolitaireGameClass : RegularDeckOfCardsGameClass<SolitaireCard>
    {
        public PyramidSolitaireSaveInfo SaveRoot;

        private readonly ISaveSinglePlayerClass _thisState;

        internal readonly PyramidSolitaireViewModel ThisMod;

        internal bool GameGoing;

        public PyramidSolitaireGameClass(ISoloCardGameVM<SolitaireCard> thisMod) : base(thisMod)
        {
            _thisState = thisMod.MainContainer!.Resolve<ISaveSinglePlayerClass>();
            ThisMod = (PyramidSolitaireViewModel)thisMod;
            SaveRoot = new PyramidSolitaireSaveInfo();
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
            SaveRoot = await _thisState.RetrieveSinglePlayerGameAsync<PyramidSolitaireSaveInfo>();
            if (SaveRoot.DeckList.Count > 0)
            {
                var newList = SaveRoot.DeckList.GetNewObjectListFromDeckList(DeckList);
                ThisMod.DeckPile!.OriginalList(newList);
            }
            //anything else that is needed to open the saved game will be here.
            ThisMod.DeckPile!.Visible = true;
            //anything else that is needed to open the saved game will be here.


            ThisMod.Discard!.SavedDiscardPiles(SaveRoot.DiscardPileData!);
            ThisMod.CurrentPile!.SavedDiscardPiles(SaveRoot.CurrentPileData!);
            ThisMod.PlayList1!.LoadSavedGame(SaveRoot.PlayList);
            SaveRoot.LoadMod(ThisMod);
            ThisMod.GameBoard1!.LoadSavedTriangles(SaveRoot.TriangleData!);
            ThisMod.NewGameVisible = SaveRoot.CanEnableGame;
        }
        private bool _isBusy;
        public async Task SaveStateAsync()
        {
            if (_isBusy)
                return;
            _isBusy = true;
            SaveRoot.DeckList = ThisMod.DeckPile!.GetCardIntegers();
            SaveRoot.TriangleData = ThisMod.GameBoard1!.GetSavedTriangles();
            SaveRoot.DiscardPileData = ThisMod.Discard!.GetSavedPile();
            SaveRoot.CurrentPileData = ThisMod.CurrentPile!.GetSavedPile();
            SaveRoot.PlayList = ThisMod.PlayList1!.ObjectList.ToRegularDeckDict();
            await _thisState.SaveSimpleSinglePlayerGameAsync(SaveRoot); //i think
            _isBusy = false;
        }

        public async Task ShowWinAsync()
        {
            await ThisMod.ShowGameMessageAsync("Congratulations, you won");
            ThisMod.NewGameVisible = true;
            SaveRoot.CanEnableGame = true;
            GameGoing = false;
            await _thisState.DeleteSinglePlayerGameAsync(); //i think.
        }

        protected override void AfterShuffle()
        {
            //this is what runs after the cards shuffle.
            ThisMod.CurrentPile!.ClearCards();
            SaveRoot.Score = 0;
            SaveRoot.CanEnableGame = false;
            ThisMod.Discard!.ClearCards();
            ThisMod.NewGameVisible = false; //you have to go through the deck at least one.
            var newList = ThisMod.DeckPile!.DrawSeveralCards(28);
            ThisMod.PlayList1!.ObjectList.Clear();
            ThisMod.GameBoard1!.ClearCards(newList);
        }
        public async Task DrawCardAsync()
        {
            if (HasPlayedCard())
            {
                await ThisMod.ShowGameMessageAsync("Sorry, you must choose to either play the cards or put them back before drawing another card");
                return;
            }
            bool wasEnd = ThisMod.DeckPile!.IsEndOfDeck();
            if (wasEnd)
            {
                var thisCol = ThisMod.Discard!.DiscardList();
                if (ThisMod.Discard.PileEmpty() == false)
                    thisCol.Add(ThisMod.Discard.GetCardInfo());
                if (ThisMod.CurrentPile!.PileEmpty() == false)
                    thisCol.Add(ThisMod.CurrentPile.GetCardInfo());
                ThisMod.CurrentPile.ClearCards();
                ThisMod.Discard.ClearCards();
                ThisMod.DeckPile.OriginalList(thisCol);
            }
            var thisCard = ThisMod.DeckPile.DrawCard();
            if (ThisMod.CurrentPile!.PileEmpty())
                ThisMod.CurrentPile.AddCard(thisCard);
            else
            {
                var newCard = ThisMod.CurrentPile.GetCardInfo();
                ThisMod.Discard!.AddCard(newCard);
                ThisMod.CurrentPile.AddCard(thisCard);
            }
            SaveRoot.RecentCard = ThisMod.CurrentPile.GetCardInfo();
            SaveRoot.CanEnableGame = wasEnd;
        }
        public bool HasPlayedCard() => ThisMod.PlayList1!.HasChosenCards();
        public bool IsValidMove()
        {
            var thisCol = ThisMod.PlayList1!.ObjectList;
            int maxs = thisCol.Sum(items => (int)items.Value);
            return maxs == 13;
        }
        public void PutBack()
        {
            var thisCol = ThisMod.PlayList1!.ObjectList;
            thisCol.ForEach(thisCard =>
            {
                if (ThisMod.GameBoard1!.CanPutBack(thisCard.Deck) == false)
                {
                    if (thisCard.Deck == SaveRoot.RecentCard.Deck)
                        ThisMod.CurrentPile!.AddCard(thisCard);
                    else
                        ThisMod.Discard!.AddCard(thisCard);
                }
            });
            ThisMod.GameBoard1!.PutBackAll();
            ThisMod.PlayList1.RemoveCards();
        }
        public bool CanAddToPlay() => !ThisMod.PlayList1!.AlreadyHasTwoCards();
        public async Task PlayCardsAsync()
        {
            var thisCol = ThisMod.PlayList1!.ObjectList;
            SaveRoot.Score += thisCol.Count;
            if (ThisMod.CurrentPile!.PileEmpty() == false)
                SaveRoot.RecentCard = ThisMod.CurrentPile.GetCardInfo();
            ThisMod.PlayList1.RemoveCards();
            ThisMod.GameBoard1!.MakePermanant();
            if (SaveRoot.Score == 52)
            {
                await ShowWinAsync();
                return;
            }
        }
    }
}
