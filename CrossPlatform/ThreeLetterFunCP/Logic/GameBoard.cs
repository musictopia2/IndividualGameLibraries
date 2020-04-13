using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DrawableListsViewModels;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ThreeLetterFunCP.Data;
namespace ThreeLetterFunCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class GameBoard : GameBoardObservable<ThreeLetterFunCardData>
    {
        private readonly GlobalHelpers _global;
        private readonly ThreeLetterFunVMData _model;
        private readonly BasicData _basicData;
        private readonly TestOptions _test;
        private readonly IListShuffler<ThreeLetterFunCardData> _deck; //hopefully this works.
        private readonly IAsyncDelayer _delay;

        public GameBoard(CommandContainer container,
            IAsyncDelayer delay,
            GlobalHelpers global,
            ThreeLetterFunVMData model,
            BasicData basicData,
            TestOptions test,
            IListShuffler<ThreeLetterFunCardData> deck
            ) : base(container)
        {
            _delay = delay;
            _global = global;
            _model = model;
            _basicData = basicData;
            _test = test;
            _deck = deck;
            HasFrame = false;
            Visible = true;
            IsEnabled = false;
        }


        #region "Delegates to stop the overflow issues"
        public Func<ThreeLetterFunSaveInfo>? SaveRoot { get; set; }
        public Action? SetSelf { get; set; } //this will ask the main game clas to set player to self.
        public Func<ThreeLetterFunPlayerItem>? SingleInfo { get; set; }

        #endregion

        //public GameBoard(IBasicGameVM thisMod) : base(thisMod)
        //{
        //    _mainGame = thisMod.MainContainer!.Resolve<ThreeLetterFunMainGameClass>();
        //    _thisGlobal = thisMod.MainContainer.Resolve<GlobalHelpers>(); //globals has to be created first.
        //    _thisMod = (ThreeLetterFunViewModel)thisMod;
        //    _delay = thisMod.MainContainer.Resolve<IAsyncDelayer>();
        //    HasFrame = false;
        //    Visible = true;
        //    IsEnabled = false; //has to be proven true.  hopefully works.  may require rethinking.
        //}

        public void NewLoadSavedGame()
        {
            SetUpBoard();
            if (SaveRoot!.Invoke().Level != EnumLevel.Easy)
                ObjectList.ReplaceRange(SaveRoot!.Invoke().SavedList);
            else
            {
                SetSelf!.Invoke();

                ObjectList = SingleInfo!.Invoke().MainHandList;
            }
            ObjectList.ForEach(Items => Items.ReloadSaved());
        }

        private void SetUpBoard()
        {
            if (SaveRoot!.Invoke().CardsToBeginWith == 36)
            {
                Rows = 4;
                Columns = 9;
            }
            else
            {
                Rows = 1;
                Columns = SaveRoot!.Invoke().CardsToBeginWith;
            }
        }

        protected override Task ClickProcessAsync(ThreeLetterFunCardData thisObject)
        {
            if (thisObject.ClickLocation == EnumClickPosition.None)
                throw new BasicBlankException("Must know what the position it clicked on in order to move on");
            _global.PauseContinueTimer();
            var firstPosition = thisObject.GetLetterPosition(thisObject.ClickLocation);
            if (firstPosition == 0 && thisObject.ClickLocation == EnumClickPosition.Right)
                throw new BasicBlankException("The first position cannot be 0 if the click location is to the right");
            var tempTile = new TilePosition();
            var thisTile = _model.TileBoard1!.GetTile(true);
            if (thisTile == null)
            {
                _global.PauseContinueTimer();
                return Task.CompletedTask; //because you selected nothing.  if you did, rethink.
            }
            if (_basicData.MultiPlayer == true)
            {
                SetSelf!.Invoke();
                SingleInfo!.Invoke().TileList.Clear();
                tempTile.Index = firstPosition;
                SingleInfo!.Invoke().CardUsed = ObjectList.IndexOf(thisObject); //i think.
                tempTile.Deck = thisTile.Deck;
                SingleInfo!.Invoke().TileList.Add(tempTile);

            }
            thisObject.AddLetter(thisTile.Deck, firstPosition);
            var newTile = _model.TileBoard1.GetTile(false);
            thisTile.Visible = false;
            thisTile.IsSelected = false;
            if (SaveRoot!.Invoke().Level != EnumLevel.Easy)
            {
                var others = thisObject.LetterRemaining();
                if (_basicData.MultiPlayer == true)
                {
                    tempTile = new TilePosition();
                    tempTile.Index = others;
                    tempTile.Deck = newTile!.Deck;
                    SingleInfo!.Invoke().TileList.Add(tempTile);
                }
                newTile!.Visible = false;
                newTile.IsSelected = false;
                thisObject.AddLetter(newTile.Deck, others);
            }
            thisObject.HiddenValue++;
            _global.PauseContinueTimer();
            return Task.CompletedTask;
        }
        public void ClearBoard(IDeckDict<ThreeLetterFunCardData> thisList)
        {
            SaveRoot!.Invoke().CardsToBeginWith = thisList.Count;
            SetUpBoard();
            ObjectList.ReplaceRange(thisList);
            ObjectList.MakeAllObjectsKnown();
            _model.PlayerWon = "";
        }
        public void RemoveTiles()
        {
            ObjectList.RemoveTiles();
        }
        public void UnDo()
        {
            ObjectList.RemoveTiles();
            _model.TileBoard1!.Undo();
            _global.PauseContinueTimer(); // i think this is needed too.
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
            if (_basicData.MultiPlayer == false)
                throw new BasicBlankException("Can't show a word for single player games");
            ObjectList.RemoveTiles();
            ThreeLetterFunCardData thisCard;
            if (SaveRoot!.Invoke().Level != EnumLevel.Easy)
                thisCard = ObjectList.GetSpecificItem(deck); //try from here no matter what.  otherwise, will get hosed.
            else if (SingleInfo!.Invoke().PlayerCategory == EnumPlayerCategory.Self)
                thisCard = ObjectList.GetSpecificItem(deck);
            else
                thisCard = _deck.GetSpecificItem(deck);
            if (SingleInfo!.Invoke().TileList.Count == 0)
                throw new BasicBlankException("Must have tiles to play");
            SingleInfo!.Invoke().TileList.ForEach(ThisTile =>
                thisCard.AddLetter(ThisTile.Deck, ThisTile.Index));
            if (thisCard.IsValidWord() == false)
                throw new BasicBlankException("Not a valid word");
            if (SaveRoot!.Invoke().Level == EnumLevel.Easy)
            {
                _model.CurrentCard = thisCard.CloneCard();
                SingleInfo!.Invoke().TileList.ForEach(tile =>
                    _model.CurrentCard.AddLetter(tile.Deck, tile.Index));
            }
            else
                _model.CurrentCard = null;
            if (_test.NoAnimations == false)
                await _delay.DelaySeconds(.2);
            await UIPlatform.ShowMessageAsync($"{SingleInfo!.Invoke().NickName} spelled a word");
            _model.CurrentCard = null;
            SingleInfo!.Invoke().CardsWon++;
            SingleInfo!.Invoke().MostRecent = SaveRoot!.Invoke().UpTo;
            ObjectList.RemoveTiles();
            if (SingleInfo!.Invoke().PlayerCategory == EnumPlayerCategory.Self)
                thisCard.Visible = false; //can't be invisible if you did not get the word.   
        }
    }
}
