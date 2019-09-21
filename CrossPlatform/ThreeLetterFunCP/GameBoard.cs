using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ThreeLetterFunCP
{
    [SingletonGame]
    public class GameBoard : GameBoardViewModel<ThreeLetterFunCardData>
    {
        private readonly ThreeLetterFunMainGameClass _mainGame;
        private readonly GlobalHelpers _thisGlobal;
        private readonly ThreeLetterFunViewModel _thisMod;
        private readonly IAsyncDelayer _delay;
        public void NewLoadSavedGame()
        {
            SetUpBoard();
            if (_mainGame.SaveRoot!.Level != EnumLevel.Easy)
                ObjectList.ReplaceRange(_mainGame.SaveRoot.SavedList);
            else
            {
                _mainGame.SingleInfo = _mainGame.PlayerList!.GetSelf(); //i think.
                ObjectList = _mainGame.SingleInfo.MainHandList;
            }
            ObjectList.ForEach(Items => Items.ReloadSaved());
        }

        private void SetUpBoard()
        {
            if (_mainGame.SaveRoot!.CardsToBeginWith == 36)
            {
                Rows = 4;
                Columns = 9;
            }
            else
            {
                Rows = 1;
                Columns = _mainGame.SaveRoot.CardsToBeginWith;
            }
        }
        public GameBoard(IBasicGameVM thisMod) : base(thisMod)
        {
            _mainGame = thisMod.MainContainer!.Resolve<ThreeLetterFunMainGameClass>();
            _thisGlobal = thisMod.MainContainer.Resolve<GlobalHelpers>(); //globals has to be created first.
            _thisMod = (ThreeLetterFunViewModel)thisMod;
            _delay = thisMod.MainContainer.Resolve<IAsyncDelayer>();
            HasFrame = false;
            Visible = true;
            IsEnabled = false; //has to be proven true.  hopefully works.  may require rethinking.
        }
        protected override Task ClickProcessAsync(ThreeLetterFunCardData thisObject)
        {
            if (thisObject.ClickLocation == EnumClickPosition.None)
                throw new BasicBlankException("Must know what the position it clicked on in order to move on");
            _thisGlobal.PauseContinueTimer();
            var firstPosition = thisObject.GetLetterPosition(thisObject.ClickLocation);
            if (firstPosition == 0 && thisObject.ClickLocation == EnumClickPosition.Right)
                throw new BasicBlankException("The first position cannot be 0 if the click location is to the right");
            var tempTile = new TilePosition();
            var thisTile = _thisMod.TileBoard1!.GetTile(true);
            if (thisTile == null)
            {
                _thisGlobal.PauseContinueTimer();
                return Task.CompletedTask; //because you selected nothing.  if you did, rethink.
            }
            if (_mainGame.ThisData!.MultiPlayer == true)
            {
                _mainGame.SingleInfo = _mainGame.PlayerList!.GetSelf();
                _mainGame.SingleInfo.TileList.Clear();
                tempTile.Index = firstPosition;
                _mainGame.SingleInfo.CardUsed = ObjectList.IndexOf(thisObject); //i think.
                tempTile.Deck = thisTile.Deck;
                _mainGame.SingleInfo.TileList.Add(tempTile);

            }
            thisObject.AddLetter(thisTile.Deck, firstPosition);
            var newTile = _thisMod.TileBoard1.GetTile(false);
            thisTile.Visible = false;
            thisTile.IsSelected = false;
            if (_mainGame.SaveRoot!.Level != EnumLevel.Easy)
            {
                var others = thisObject.LetterRemaining();
                if (_mainGame.ThisData.MultiPlayer == true)
                {
                    tempTile = new TilePosition();
                    tempTile.Index = others;
                    tempTile.Deck = newTile!.Deck;
                    _mainGame.SingleInfo!.TileList.Add(tempTile);
                }
                newTile!.Visible = false;
                newTile.IsSelected = false;
                thisObject.AddLetter(newTile.Deck, others);
            }
            thisObject.HiddenValue++;
            _thisGlobal.PauseContinueTimer();
            return Task.CompletedTask;
        }
        public void ClearBoard(IDeckDict<ThreeLetterFunCardData> thisList)
        {
            _mainGame.SaveRoot!.CardsToBeginWith = thisList.Count;
            SetUpBoard();
            ObjectList.ReplaceRange(thisList);
            ObjectList.MakeAllObjectsKnown();
            _thisMod.PlayerWon = "";
        }
        public void RemoveTiles()
        {
            ObjectList.RemoveTiles();
        }
        public void UnDo()
        {
            ObjectList.RemoveTiles();
            _thisMod.TileBoard1!.UnDo();
            _thisGlobal.PauseContinueTimer(); // i think this is needed too.
        }
        public ThreeLetterFunCardData GetCompletedCard()
        {
            return (from items in ObjectList
                    where items.CompletedWord() == true
                    select items).SingleOrDefault();
        }
        public int CardsRemaining()
        {
            return ObjectList.Count(items => items.Visible == true);
        }
        public async Task ShowWordAsync(int deck)
        {
            if (_mainGame.ThisData!.MultiPlayer == false)
                throw new BasicBlankException("Can't show a word for single player games");
            ObjectList.RemoveTiles();
            ThreeLetterFunCardData thisCard;
            if (_mainGame.SaveRoot!.Level != EnumLevel.Easy)
                thisCard = ObjectList.GetSpecificItem(deck); //try from here no matter what.  otherwise, will get hosed.
            else if (_mainGame.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                thisCard = ObjectList.GetSpecificItem(deck);
            else
                thisCard = _mainGame.DeckList!.GetSpecificItem(deck);
            if (_mainGame.SingleInfo!.TileList.Count == 0)
                throw new BasicBlankException("Must have tiles to play");
            _mainGame.SingleInfo.TileList.ForEach(ThisTile =>
                thisCard.AddLetter(ThisTile.Deck, ThisTile.Index));
            if (thisCard.IsValidWord() == false)
                throw new BasicBlankException("Not a valid word");
            if (_mainGame.SaveRoot.Level == EnumLevel.Easy)
            {
                _thisMod.CurrentCard = thisCard.CloneCard();
                _mainGame.SingleInfo.TileList.ForEach(ThisTile =>
                    _thisMod.CurrentCard.AddLetter(ThisTile.Deck, ThisTile.Index));
            }
            else
                _thisMod.CurrentCard = null;
            if (_mainGame.ThisTest!.NoAnimations == false)
                await _delay.DelaySeconds(.2);
            await _thisMod.ShowGameMessageAsync($"{_mainGame.SingleInfo.NickName} spelled a word");
            _thisMod.CurrentCard = null;
            _mainGame.SingleInfo.CardsWon++;
            _mainGame.SingleInfo.MostRecent = _mainGame.SaveRoot.UpTo;
            ObjectList.RemoveTiles();
            if (_mainGame.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                thisCard.Visible = false; //can't be invisible if you did not get the word.   
        }
    }
}