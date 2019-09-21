using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Extensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using ps = BasicGameFramework.BasicDrawables.MiscClasses;
namespace ThreeLetterFunCP
{
    [SingletonGame]
    public class StarterClass : IFirstOptions
    {
        readonly ThreeLetterFunMainGameClass _mainGame;
        public FirstOptionViewModel? FirstOption1;
        public CardsPlayerViewModel? Cards1;
        public AdvancedOptionsViewModel? Advanced1;
        readonly IAsyncDelayer _delay;
        readonly ThreeLetterFunViewModel _thisMod;
        GlobalHelpers? _thisGlobal;
        public StarterClass(ThreeLetterFunMainGameClass mainGame, IAsyncDelayer delay, ThreeLetterFunViewModel thisMod)
        {
            _mainGame = mainGame;
            _delay = delay;
            _thisMod = thisMod;
        }
        public void TestLoad()
        {
            LoadUp(); //so i can test some things first.
        }
        public void Init()
        {
            if (_mainGame.ThisData!.MultiPlayer == false)
                throw new BasicBlankException("Single player don't have advanced settings");
            if (FirstOption1 != null)
                return; //because already done.
            LoadUp();
        }
        private void LoadUp()
        {
            FirstOption1 = _mainGame.MainContainer.Resolve<FirstOptionViewModel>();
            Cards1 = _mainGame.MainContainer.Resolve<CardsPlayerViewModel>();
            Advanced1 = _mainGame.MainContainer.Resolve<AdvancedOptionsViewModel>();
        }
        public void StartUp()
        {
            if (_mainGame.ThisData!.MultiPlayer == false)
                throw new BasicBlankException("Single player don't have advanced settings");
            FirstOption1!.Visible = true;
            Cards1!.Visible = false;
            Advanced1!.Visible = false;
        }
        async Task IFirstOptions.BeginningOptionSelectedAsync(EnumFirstOption firstOption)
        {
            if (_mainGame.ThisData!.MultiPlayer == false)
                throw new BasicBlankException("Single player don't have advanced settings");
            await _mainGame.ThisNet!.SendAllAsync("firstoption", firstOption);
            await ProcessBeginningOptionsAsync(firstOption, true);
        }
        internal async Task ProcessBeginningOptionsAsync(string data)
        {
            EnumFirstOption thisOption = await js.DeserializeObjectAsync<EnumFirstOption>(data);
            await ProcessBeginningOptionsAsync(thisOption, false);
        }
        private async Task ProcessBeginningOptionsAsync(EnumFirstOption chosen, bool isHost)
        {
            FirstOption1!.Option1.SelectSpecificItem((int)chosen); //hopefully that works.
            if (_mainGame.ThisTest!.NoAnimations == false)
                await _delay.DelayMilli(500);
            FirstOption1.Visible = false;
            if (chosen == EnumFirstOption.Beginner)
            {
                Cards1!.Visible = true;
                _mainGame.SaveRoot!.Level = EnumLevel.Easy;
                if (isHost == false)
                    _mainGame.ThisCheck!.IsEnabled = true;
                return; //hopefully works. if not, rethink.
            }
            Advanced1!.Visible = true;
            if (isHost == false)
                _mainGame.ThisCheck!.IsEnabled = true;
        }

        async Task IFirstOptions.CardsChosenAsync(int howManyCards)
        {
            if (_mainGame.ThisData!.MultiPlayer == false)
                throw new BasicBlankException("Single player don't have advanced settings");
            await _mainGame.ThisNet!.SendAllAsync("howmanycards", howManyCards);
            await ProcessCardsChosenAsync(howManyCards, true);
        }
        internal async Task ProcessCardsChosenAsync(int howManyCards, bool isHost)
        {
            Cards1!.HowManyCards = howManyCards;
            Cards1.SelectGivenValue();
            if (_mainGame.ThisTest!.NoAnimations == false)
                await _delay.DelayMilli(500);
            Cards1.Visible = false;
            if (isHost == false)
            {
                _mainGame.ThisCheck!.IsEnabled = true; //has to wait for complete game data again.  you saw what was chosen though.
                return;
            }
            await StartShufflingAsync(howManyCards);
        }
        async Task IFirstOptions.ChoseAdvancedOptions(bool isEasy, bool shortGame)
        {
            if (_mainGame.ThisData!.MultiPlayer == false)
                throw new BasicBlankException("Multiplayer don't have advanced settings");
            AdvancedSetting thisA = new AdvancedSetting();
            thisA.IsEasy = isEasy;
            thisA.ShortGame = shortGame;
            await _mainGame.ThisNet!.SendAllAsync("advancedsettings", thisA);
            await ProcessAdvancedAsync(isEasy, shortGame, true);
        }
        internal async Task ProcessAdvancedAsync(string data)
        {
            AdvancedSetting thisA = await js.DeserializeObjectAsync<AdvancedSetting>(data);
            await ProcessAdvancedAsync(thisA.IsEasy, thisA.ShortGame, false);
        }
        async Task ProcessAdvancedAsync(bool isEasy, bool shortGame, bool isHost)
        {
            Advanced1!.EasyWords = isEasy;
            Advanced1.ShortGame = shortGame;
            Advanced1.SelectSpecificOptions();
            if (_mainGame.ThisTest!.NoAnimations == false)
                await _delay.DelayMilli(500);
            Advanced1.Visible = false;
            if (isHost == false)
            {
                _mainGame.ThisCheck!.IsEnabled = true; //has to wait for complete game data again.  you saw what was chosen though.
                return;
            }
            _mainGame.SaveRoot!.ShortGame = shortGame;
            if (isEasy == true)
                _mainGame.SaveRoot.Level = EnumLevel.Moderate;
            else
                _mainGame.SaveRoot.Level = EnumLevel.Hard;
            await StartShufflingAsync();

        }
        internal async Task StartShufflingAsync(int cardsToPassOut = 0)
        {
            if (_thisGlobal == null)
                _thisGlobal = _mainGame.MainContainer.Resolve<GlobalHelpers>();
            _mainGame.SaveRoot!.CanStart = true;
            _thisMod.MainOptionsVisible = true;
            _mainGame.DeckList = _mainGame.MainContainer.Resolve<IListShuffler<ThreeLetterFunCardData>>();
            _mainGame.DeckList.ClearObjects();
            _mainGame.DeckList.ShuffleObjects(); //i think.
            _mainGame.SaveRoot.TileList = _thisGlobal.GetTiles();
            _mainGame.SaveRoot.TileList.ShuffleList();
            _mainGame.SaveRoot.UpTo = 1; //i think
            DeckRegularDict<ThreeLetterFunCardData> cardList = _mainGame.DeckList.Take(36).ToRegularDeckDict();
            if (_mainGame.SaveRoot.TileList.Count != 100)
                throw new BasicBlankException("Must have 100 tiles");
            if (cardList.Count != 36)
                throw new BasicBlankException("Must have 36 cards");
            if (_mainGame.ThisData!.MultiPlayer == true)
            {
                _mainGame.PlayerList!.ForEach(thisPlayer =>
                {
                    thisPlayer.ClearTurn();
                    thisPlayer.CardsWon = 0;
                    thisPlayer.CardUsed = 0;
                    thisPlayer.MostRecent = 0;
                    thisPlayer.TookTurn = false;
                });
            }

            //if we have to pass out cards, will be here.
            DeckRegularDict<ThreeLetterFunCardData> firstList = _mainGame.DeckList.ToRegularDeckDict();
            if (cardsToPassOut > 0)
            {
                DeckRegularDict<ThreeLetterFunCardData> temps = new DeckRegularDict<ThreeLetterFunCardData>();
                ps.CardProcedures.PassOutCards(_mainGame.PlayerList!, firstList, cardsToPassOut, 0, false, ref temps);
            }
            if (_mainGame.SaveRoot.Level != EnumLevel.Easy)
                _thisGlobal.GameBoard1!.ClearBoard(cardList);
            else
                _thisGlobal.GameBoard1!.ClearBoard(_mainGame.SingleInfo!.MainHandList);
            _thisGlobal.GameBoard1.Visible = true; //i think.
            if (_mainGame.ThisData.MultiPlayer == false)
            {
                _thisMod.TileBoard1!.UpdateBoard(); //something else will show continue turn.
                return;
            }
            await _mainGame.PopulateSaveRootAsync(); //so she gets the proper values.
            await _mainGame.ThisNet!.SendAllAsync("restoregame", _mainGame.SaveRoot); //they need to know what to do now.
            _thisMod.TileBoard1!.UpdateBoard();
            INewGame ups = _mainGame.MainContainer.Resolve<INewGame>();
            ups.UpdateBoard();
            await _mainGame.ContinueTurnAsync();
        }
    }
}