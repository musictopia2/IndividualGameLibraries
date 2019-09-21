using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.FileFunctions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.Misc;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace ThreeLetterFunCP
{
    [SingletonGame]
    internal class GlobalHelpers : IMiscDataNM
    {
        public CustomBasicList<WordInfo>? SavedWords;
        public CustomBasicList<char>? SavedTiles { get; set; }
        public CustomBasicList<SavedCard>? SavedCardList { get; set; }
        private readonly ThreeLetterFunViewModel _thisMod;
        private readonly ThreeLetterFunMainGameClass _mainGame;
        public GameBoard? GameBoard1;
        private StarterClass? _thisStart;
        public GlobalHelpers(ThreeLetterFunViewModel ThisMod, ThreeLetterFunMainGameClass MainGame)
        {
            _thisMod = ThisMod;
            _mainGame = MainGame;
        }
        public void PopulateWords()
        {
            Assembly thisAssembly = Assembly.GetAssembly(this.GetType());
            string cardText = thisAssembly.ResourcesAllTextFromFile("cardlist.json");
            string tileText = thisAssembly.ResourcesAllTextFromFile("tilelist.json");
            SavedCardList = JsonConvert.DeserializeObject<CustomBasicList<SavedCard>>(cardText);
            var firstList = JsonConvert.DeserializeObject<CustomBasicList<SavedTile>>(tileText);
            SavedTiles = PrivateGetTiles(firstList);
            SavedWords = GetSavedWords();
        }
        public void LoadItems()
        {
            if (_mainGame.ThisData!.MultiPlayer == true)
            {
                Stops = new CustomStopWatchCP(); //i think should be here.
                Stops.TimeUp += Stops_TimeUp;
                Stops.MaxTime = 120000; //2 minutes should be enough time to try to find a word.
                _thisStart = _mainGame.MainContainer.Resolve<StarterClass>(); //i think
            }
            GameBoard1 = _mainGame.MainContainer.Resolve<GameBoard>(); //i think.
            _thisMod.TileBoard1 = new TileBoardViewModel(_thisMod);
        }
        private async void Stops_TimeUp()
        {
            await _thisMod.SelfGiveUpAsync(false); //because time is already up.
        }
        private CustomBasicList<WordInfo> GetSavedWords()
        {
            using SpellingDll.SpellingDll spells = new SpellingDll.SpellingDll();
            CustomBasicList<WordInfo> output = new CustomBasicList<WordInfo>();
            var firstList = spells.GetWords(null, 3);
            firstList.ForEach(firstWord =>
            {
                WordInfo newWord = new WordInfo();
                newWord.Word = firstWord.Word;
                newWord.IsEasy = firstWord.Difficulty == SpellingDll.EnumDifficulty.Easy;
                output.Add(newWord);
            });
            return output;
        }
        private CustomBasicList<char> PrivateGetTiles(CustomBasicList<SavedTile> thisList)
        {
            CustomBasicList<char> finList = new CustomBasicList<char>();
            thisList.ForEach(thisTile =>
            {
                int x;
                var loopTo = thisTile.HowMany;
                for (x = 1; x <= loopTo; x++)
                    finList.Add(thisTile.Letter.Single());
            });
            return finList;
        }
        public CustomBasicList<TileInformation> GetTiles()
        {
            var thisList = Enumerable.Range(1, 100).ToCustomBasicList();
            return thisList.Select(items => GetTile(items)).ToCustomBasicList();
        }
        public TileInformation GetTile(int deck)
        {
            if (SavedTiles!.Count == 0)
                throw new BasicBlankException("No saved tiles");
            var tempTile = SavedTiles[deck - 1];
            TileInformation output = new TileInformation();
            output.Deck = deck;
            output.Letter = tempTile;
            return output;
        }
        public CustomStopWatchCP? Stops;
        public void PauseContinueTimer()
        {
            if (_mainGame.ThisData!.MultiPlayer == false)
                return; //because multiplayer has no timer.
            if (Stops!.IsRunning == true)
                Stops.PauseTimer();
            else
                Stops.ContinueTimer();
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "firstoption":
                    await _thisStart!.ProcessBeginningOptionsAsync(content);
                    break;
                case "advancedsettings":
                    await _thisStart!.ProcessAdvancedAsync(content);
                    break;
                case "howmanycards":
                    await _thisStart!.ProcessCardsChosenAsync(int.Parse(content), false);
                    break;
                case "giveup": //no more tilelist now.
                    _mainGame.SingleInfo = _mainGame.PlayerList!.GetSelf();
                    if (_mainGame.SingleInfo.TookTurn == false)
                        throw new BasicBlankException("Did not take turn");
                    _mainGame.SaveRoot!.PlayOrder.WhoTurn = int.Parse(content); //hopefully this works too.
                    await _mainGame.GiveUpAsync();
                    break;
                case "playword":
                    _mainGame.SingleInfo = _mainGame.PlayerList!.GetSelf();
                    if (_mainGame.SingleInfo.TookTurn == false)
                        throw new BasicBlankException("Did not take turn");
                    TempWord thisWord = await js.DeserializeObjectAsync<TempWord>(content);
                    _mainGame.SaveRoot!.PlayOrder.WhoTurn = thisWord.Player;
                    _mainGame.SingleInfo = _mainGame.PlayerList.GetWhoPlayer(); //hopefully this still works.
                    _mainGame.SingleInfo.TimeToGetWord = thisWord.TimeToGetWord;
                    _mainGame.SingleInfo.TileList = thisWord.TileList;
                    await _mainGame.PlayWordAsync(thisWord.CardUsed);
                    break;
                case "whowon":
                    await _mainGame.ClientResultsAsync(int.Parse(content));
                    break;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
    }
}