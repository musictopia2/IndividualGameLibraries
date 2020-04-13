using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using TileRummyCP.Data;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace TileRummyCP.Logic
{
    public class TileSet : SetInfo<EnumColorType, EnumColorType, TileInfo, SavedSet>
    {
        private bool _isNew;
        private EnumWhatSets _setType;
        private readonly RummyProcesses<EnumColorType, EnumColorType, TileInfo> _rummys;

        //private readonly TileRummyMainGameClass _mainGame;
        //can't require main game.  or overflow errors.

        public TileSet(CommandContainer command, RummyProcesses<EnumColorType, EnumColorType, TileInfo> rummys) : base(command)
        {
            _rummys = rummys;
            //_mainGame = thisMod.MainContainer!.Resolve<TileRummyMainGameClass>();
        }
        public override void LoadSet(SavedSet payLoad)
        {
            HandList.ReplaceRange(payLoad.TileList);
            _isNew = payLoad.IsNew;
            _setType = payLoad.SetType;
        }
        public override SavedSet SavedSet()
        {
            SavedSet output = new SavedSet();
            output.TileList = HandList.ToRegularDeckDict();
            output.IsNew = _isNew;
            output.SetType = _setType;
            return output;
        }
        protected override bool CanClickMainBoard()
        {
            return false;
        }
        public override bool CanClickSingleObject()
        {
            return true;
        }
        public override void EndTurn()
        {
            _isNew = false;
            HandList.ForEach(thisTile => thisTile.WhatDraw = EnumDrawType.IsNone);
            base.EndTurn();
        }
        private DeckRegularDict<TileInfo> GetWildList(IDeckDict<TileInfo> tempList) => tempList.Where(items => items.IsJoker).ToRegularDeckDict();
        public int PositionToPlay(TileInfo thisTile, int position)
        {
            if (_setType == EnumWhatSets.Kinds)
                return position;
            if (thisTile.IsJoker == true)
                return position;
            int newPos;
            if (position == 1)
                newPos = 2;
            else
                newPos = 1;
            TileInfo newTile;
            if (newPos == 1)
                newTile = HandList.First();
            else
                newTile = HandList.Last();
            if (newTile.Number == 13)
                return 1;
            if (newTile.Number == 1)
                return 2;
            if (((newTile.Number + 1) == thisTile.Number) & (newPos == 2))
                return newPos;
            if (newTile.Number == 1)
                return 2;
            if (((newTile.Number - 1) == thisTile.Number) & (newPos == 1))
                return newPos;
            return position;
        }
        public void CreateSet(IDeckDict<TileInfo> thisCol, EnumWhatSets whatType)
        {
            _setType = whatType;
            _isNew = true;
            TileRummySaveInfo saveRoot = cons!.Resolve<TileRummySaveInfo>();
            if (thisCol.Count == 0)
                throw new BasicBlankException("There must be at least one item to create a new set");
            foreach (var tempTile in thisCol)
                saveRoot.TilesFromField.RemoveSpecificItem(tempTile.Deck);// if not there, ignore
            if (_setType == EnumWhatSets.Kinds)
            {
                HandList.ReplaceRange(thisCol);
                return;
            }
            var wildCol = GetWildList(thisCol);
            int VeryFirst;
            VeryFirst = thisCol.First().Number;
            int veryLast;
            veryLast = thisCol.Last().Number;
            int firstNum;
            int lastNum;
            firstNum = VeryFirst;
            lastNum = veryLast;
            int x;
            int y;
            int WildNum = default;
            y = 1;
            var loopTo = thisCol.Count;
            for (x = 2; x <= loopTo; x++)
            {
                y += 1;
                firstNum += 1;
                var thisTile = thisCol[y - 1];
                if (thisTile.Number != firstNum)
                {
                    WildNum += 1;
                    thisTile = wildCol[WildNum - 1];
                    thisTile.Number = firstNum;
                    if (thisTile.Number == 14)
                        thisTile.Number = VeryFirst - 1;
                    y -= 1;
                }
            }
            var Temps = (from items in thisCol
                         orderby items.Number
                         select items).ToList();
            HandList.ReplaceRange(Temps);
        }
        public void AddTile(TileInfo thisTile, int position)
        {
            TileRummySaveInfo saveRoot = cons!.Resolve<TileRummySaveInfo>();
            thisTile.Drew = true; // this should be okay so others can tell something was added to it.
            thisTile.IsSelected = false;
            saveRoot.TilesFromField.RemoveSpecificItem(thisTile.Deck);
            if (((int)_setType == (int)EnumWhatSets.Runs) & (thisTile.IsJoker == true))
            {
                TileInfo newTile;
                if (position == 1)
                {
                    newTile = HandList.First();
                    thisTile.Number = newTile.Number - 1;
                    HandList.InsertBeginning(thisTile);
                }
                else
                {
                    newTile = HandList.Last();
                    thisTile.Number = newTile.Number + 1;
                    HandList.Add(thisTile);
                }
            }
            else
            {
                HandList.Add(thisTile);
            }

            if ((int)_setType == (int)EnumWhatSets.Runs)
            {
                var TempList = (from Items in HandList
                                orderby Items.Number
                                select Items).ToList();
                HandList.ReplaceRange(TempList);
            }
        }
        //if i can't ask for it to begin with, do here for rummys.
        public bool IsAcceptableSet()
        {
            if (HandList.Count < 3)
                return false;
            var thisList = HandList.ToRegularDeckDict();
            if (thisList.Count(items => items.IsJoker) > 1)
                return false; //each set can only have one joker
            var wildList = GetWildList(thisList);
            if (wildList.Count == 1)
            {
                if (_isNew)
                {
                    if (thisList.Count(items => items.IsJoker == false && items.WhatDraw == EnumDrawType.FromHand) < 2)
                        return false; //because needs 2 from hand.
                }
            }
            var newRummy = HandList.ToRegularDeckDict();
            if (_setType == EnumWhatSets.Runs)
                return _rummys.IsNewRummy(newRummy, newRummy.Count, RummyProcesses<EnumColorType, EnumColorType, TileInfo>.EnumRummyType.Runs);
            return _rummys.IsNewRummy(newRummy, newRummy.Count, RummyProcesses<EnumColorType, EnumColorType, TileInfo>.EnumRummyType.Sets);
        }
    }
}
