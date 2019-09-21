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
//i think this is the most common things i like to do
namespace ClockSolitaireCP
{
    [SingletonGame]
    public class ClockSolitaireGameClass : RegularDeckOfCardsGameClass<SolitaireCard>
    {
        public ClockSolitaireSaveInfo SaveRoot;

        private readonly ISaveSinglePlayerClass _thisState;

        internal readonly ClockSolitaireViewModel ThisMod;

        internal bool GameGoing;

        public ClockSolitaireGameClass(ISoloCardGameVM<SolitaireCard> thisMod) : base(thisMod)
        {
            _thisState = thisMod.MainContainer!.Resolve<ISaveSinglePlayerClass>();
            ThisMod = (ClockSolitaireViewModel)thisMod;
            SaveRoot = new ClockSolitaireSaveInfo();
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
            SaveRoot = await _thisState.RetrieveSinglePlayerGameAsync<ClockSolitaireSaveInfo>();
            if (SaveRoot.DeckList.Count > 0)
            {
                var newList = SaveRoot.DeckList.GetNewObjectListFromDeckList(DeckList);
                ThisMod.DeckPile!.OriginalList(newList);
                ThisMod.DeckPile.Visible = true;
            }
            //anything else that is needed to open the saved game will be here.
            ThisMod.Clock1!.LoadSavedClocks(SaveRoot.SavedClocks);
            SaveRoot.LoadMod(ThisMod);
        }
        private bool _isBusy;
        public async Task SaveStateAsync()
        {
            if (_isBusy)
                return;
            _isBusy = true;
            SaveRoot.DeckList = ThisMod.DeckPile!.GetCardIntegers();
            ThisMod.Clock1!.SaveGame();
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
        public async Task ShowLossAsync()
        {
            await ThisMod.ShowGameMessageAsync("Sorry, you lost");
            ThisMod.NewGameVisible = true;
            GameGoing = false;
            await _thisState.DeleteSinglePlayerGameAsync(); //i think.
        }
        protected override void AfterShuffle()
        {
            ThisMod.NewGameVisible = false;
            //this is what runs after the cards shuffle.
            ThisMod.Clock1!.NewGame(DeckList.ToRegularDeckDict());
        }
    }
}
