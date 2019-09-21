using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Extensions;
using BasicGameFramework.SpecializedGameTypes.StockClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using static FlinchCP.GlobalConstants;
namespace FlinchCP
{
    [SingletonGame]
    public class FlinchComputerAI
    {
        private readonly FlinchMainGameClass _mainGame;
        private struct DiscardPileInfo
        {
            public int Number1; // most recent
            public int Number2; // one before
            public int Pile;
        }
        private struct MoveInfo
        {
            public int StartNum;
            public CustomBasicList<ComputerData> MoveList;
            public int Value;
            public bool GetMore; // if true, then this means after the move, can get more cards
        }
        public struct ComputerDiscardInfo
        {
            public int Deck;
            public int Pile;
        }
        private DiscardPilesVM<FlinchCardInformation>? _tempDiscards;
        private CustomBasicList<DiscardPileInfo>? _tempDiscardList; // i think that's fine (well see)
        private DeckRegularDict<FlinchCardInformation>? _tempHand; // i think
        private CustomBasicList<ComputerData>? _startList; // maybe okay for this one (?)
        public int MaxPiles { get; set; }
        public CustomBasicList<ComputerData> ComputerMoves() // i think
        {
            CustomBasicList<ComputerData> output = new CustomBasicList<ComputerData>();
            _startList = ValidMoveList();
            if (_startList.Count == 0)
                return output;
            int x = default;
            CustomBasicList<MoveInfo> possibleList = new CustomBasicList<MoveInfo>();
            MoveInfo tempMove;
            foreach (var thisMove in _startList)
            {
                x += 1;
                tempMove = ResultsOfMove(_startList, x);
                if (tempMove.MoveList.Count > 0)
                    possibleList.Add(tempMove);
            }
            if (possibleList.Count == 0)
                return output;
            CustomBasicList<MoveInfo> thisCol;
            thisCol = StockList(possibleList);
            if (thisCol.Count > 0)
            {
                return thisCol.OrderByDescending(Items => Items.Value).First().MoveList;
            } // no more attempting to block human
            thisCol = GetMoreList(possibleList);
            if (thisCol.Count > 0)
            {
                return thisCol.OrderByDescending(Items => Items.Value).First().MoveList;
            }
            return possibleList.OrderByDescending(Items => Items.Value).First().MoveList;
        }
        private CustomBasicList<ComputerData> ValidMoveList()
        {
            CustomBasicList<ComputerData> thisList = new CustomBasicList<ComputerData>();
            ComputerData thisComputer;
            int x;
            foreach (var tempCard in _mainGame.SingleInfo!.MainHandList)
            {
                for (x = 1; x <= MaxPiles; x++)
                {
                    if (_mainGame.IsValidMove(x - 1, tempCard.Deck) == true)
                    {
                        thisComputer = new ComputerData();
                        thisComputer.ThisCard = tempCard; // i think needs to be this way.
                        thisComputer.Pile = x - 1;
                        thisComputer.Discard = -1;
                        thisComputer.WhichType = EnumCardType.MyCards;
                        thisList.Add(thisComputer);
                    }
                }
            }
            FlinchCardInformation thisCard;
            int y;
            DiscardPilesVM<FlinchCardInformation> thisDiscard;
            thisDiscard = _mainGame.SingleInfo.DiscardPiles!;
            DeckObservableDict<FlinchCardInformation> cardList;
            for (x = 1; x <= HowManyDiscards; x++)
            {
                cardList = thisDiscard.PileList![x - 1].ObjectList;
                if (cardList.Count > 0)
                {
                    thisCard = thisDiscard.GetLastCard(x - 1);
                    for (y = 1; y <= MaxPiles; y++)
                    {
                        if (_mainGame.IsValidMove(y - 1, thisCard.Deck) == true)
                        {
                            thisComputer = new ComputerData();
                            thisComputer.ThisCard = thisCard;
                            thisComputer.Pile = y - 1;
                            thisComputer.Discard = x - 1; // forgot -1 here too.
                            thisComputer.WhichType = EnumCardType.Discard;
                            thisList.Add(thisComputer);
                        }
                    }
                }
            }
            thisCard = _mainGame.SingleInfo.StockPile!.GetCard(); // i think
            for (x = 1; x <= MaxPiles; x++)
            {
                if (_mainGame.IsValidMove(x - 1, thisCard.Deck) == true)
                {
                    thisComputer = new ComputerData();
                    thisComputer.ThisCard = thisCard;
                    thisComputer.Pile = x - 1;
                    thisComputer.WhichType = EnumCardType.Stock;
                    thisComputer.Discard = -1;
                    thisList.Add(thisComputer);
                }
            }
            return thisList;
        }
        private void CopyDiscards(bool alsoHand)
        {
            DiscardPilesVM<FlinchCardInformation> thisDiscard;
            thisDiscard = _mainGame.SingleInfo!.DiscardPiles!;
            thisDiscard.CopyDiscards(out _tempDiscards);
            if (alsoHand == false)
                return;
            _tempHand = new DeckRegularDict<FlinchCardInformation>();
            foreach (var thisCard in _mainGame.SingleInfo.MainHandList)
                _tempHand.Add(thisCard);
        }
        private CustomBasicList<MoveInfo> StockList(CustomBasicList<MoveInfo> possibleMoves)
        {
            CustomBasicCollection<MoveInfo> output = new CustomBasicCollection<MoveInfo>();
            foreach (var tempItem in possibleMoves)
            {
                foreach (var tempMove in tempItem.MoveList)
                {
                    if ((int)tempMove.WhichType == (int)EnumCardType.Stock)
                        output.Add(tempItem);
                }
            }
            return output;
        }
        private CustomBasicList<MoveInfo> GetMoreList(CustomBasicList<MoveInfo> possibleMoves)
        {
            return possibleMoves.Where(items => items.GetMore == true).ToCustomBasicList();
        }
        private bool AllDiscardEmpty()
        {
            var thisDiscard = _mainGame.SingleInfo!.DiscardPiles;
            int x;
            for (x = 1; x <= HowManyDiscards; x++)
            {
                if (thisDiscard!.PileList![x - 1].ObjectList.Count > 0)
                    return false;
            }
            return true;
        }
        private int EmptyDiscardPile()
        {
            int x;
            var thisDiscard = _mainGame.SingleInfo!.DiscardPiles;
            for (x = 1; x <= HowManyDiscards; x++)
            {
                if (thisDiscard!.PileList![x - 1].ObjectList.Count == 0)
                    return x - 1;
            }
            return -1;
        }
        private void FillDiscardList()
        {
            _tempDiscardList = new CustomBasicList<DiscardPileInfo>();
            DiscardPileInfo thisTemp;
            var thisDiscard = _mainGame.SingleInfo!.DiscardPiles;
            FlinchCardInformation thisCard;
            for (var x = 1; x <= HowManyDiscards; x++)
            {
                thisTemp = new DiscardPileInfo();
                thisTemp.Pile = x - 1; // because 0 based
                if (thisDiscard!.PileList![x - 1].ObjectList.Count == 1)
                {
                    // ThisCard = ThisDiscard.PileList(x - 1).CardList.GetSpecificCardFromDeck(ThisDiscard.FirstDeck(x - 1))
                    thisCard = thisDiscard.PileList[x - 1].ObjectList.Single(); // i think this one.
                    thisTemp.Number1 = thisCard.Number;
                    thisTemp.Number2 = 0;
                }
                else
                {
                    thisCard = thisDiscard.PileList[x - 1].ObjectList.GetSpecificItem(thisDiscard.GetLastCard(x - 1).Deck);
                    thisTemp.Number1 = thisCard.Number;
                    thisCard = thisDiscard.PileList[x - 1].ObjectList.GetSpecificItem(thisDiscard.NextToLastDeck(x - 1));
                    thisTemp.Number2 = thisCard.Number;
                }
                _tempDiscardList.Add(thisTemp);
            }
        }
        private ComputerDiscardInfo FindBestStack()
        {
            ComputerDiscardInfo output = new ComputerDiscardInfo();
            output.Deck = 0;
            output.Pile = 0;
            ComputerDiscardInfo thisDiscard;
            int newNums = default;
            CustomBasicList<ComputerDiscardInfo> newList = new CustomBasicList<ComputerDiscardInfo>(); // i think
            FlinchCardInformation thisCard;
            foreach (var thisTemp in _tempDiscardList!)
            {
                if (thisTemp.Number1 < 16 && thisTemp.Number1 > 1)
                {
                    newNums = thisTemp.Number1 - 1;
                    var Filters = _mainGame.SingleInfo!.MainHandList.Where(Items => Items.Number == newNums).ToRegularDeckDict();
                    if (Filters.Count() > 0)
                    {
                        thisCard = Filters.First();
                        thisDiscard = new ComputerDiscardInfo();
                        thisDiscard.Deck = thisCard.Deck;
                        thisDiscard.Pile = thisTemp.Pile;
                        newList.Add(thisDiscard);
                    }
                }
            }
            if (newList.Count == 0)
                return output;
            return newList.GetRandomItem();
        }
        private ComputerDiscardInfo FindBestStack(CustomBasicList<DiscardPileInfo> stacks)
        {
            ComputerDiscardInfo output = new ComputerDiscardInfo();
            output.Deck = 0;
            output.Pile = 0;
            if (stacks.Count == 0)
                return output;
            CustomBasicList<ComputerDiscardInfo> newList = new CustomBasicList<ComputerDiscardInfo>(); // i think
            int newNums = default;
            ComputerDiscardInfo thisDiscard;
            FlinchCardInformation thisCard;
            foreach (var thisTemp in stacks)
            {
                if (thisTemp.Number2 < 16 && thisTemp.Number2 > 1)
                {
                    newNums = thisTemp.Number1 - 1;
                    var Filters = _mainGame.SingleInfo!.MainHandList.Where(Items => Items.Number == newNums).ToRegularDeckDict();
                    if (Filters.Count() > 0)
                    {
                        thisCard = Filters.First();
                        thisDiscard = new ComputerDiscardInfo();
                        thisDiscard.Deck = thisCard.Deck;
                        thisDiscard.Pile = thisTemp.Pile;
                        newList.Add(thisDiscard);
                    }
                }
            }
            if (newList.Count == 0)
                return output;
            return newList.GetRandomItem();
        }
        private ComputerDiscardInfo FindBestSame()
        {
            return FindBestSame(_tempDiscardList!);
        }
        private ComputerDiscardInfo FindBestSame(ICustomBasicList<DiscardPileInfo> sames)
        {
            ComputerDiscardInfo output = new ComputerDiscardInfo();
            output.Deck = 0;
            output.Pile = 0;
            if (sames.Count == 0)
                return output;
            ComputerDiscardInfo thisDiscard;
            CustomBasicList<ComputerDiscardInfo> newList = new CustomBasicList<ComputerDiscardInfo>(); // i think
            FlinchCardInformation thisCard;
            foreach (var thisTemp in sames)
            {
                if (thisTemp.Number1 < 16)
                {
                    var Filters = _mainGame.SingleInfo!.MainHandList.Where(Items => Items.Number == thisTemp.Number1).ToRegularDeckDict();
                    if (Filters.Count() > 0)
                    {
                        thisCard = Filters.First();
                        thisDiscard = new ComputerDiscardInfo();
                        thisDiscard.Deck = thisCard.Deck;
                        thisDiscard.Pile = thisTemp.Pile;
                        newList.Add(thisDiscard);
                    }
                }
            }
            if (newList.Count == 0)
                return output;
            return newList.GetRandomItem();
        }
        private CustomBasicList<DiscardPileInfo> StackList()
        {
            CustomBasicList<DiscardPileInfo> output = new CustomBasicList<DiscardPileInfo>();
            foreach (var thisTemp in _tempDiscardList!)
            {
                if (thisTemp.Number1 == (thisTemp.Number2 - 1) && thisTemp.Number1 > 0 && thisTemp.Number2 > 0)
                    output.Add(thisTemp);
            }
            return output;
        }
        private CustomBasicList<DiscardPileInfo> NoStackNoSame()
        {
            CustomBasicCollection<DiscardPileInfo> output = new CustomBasicCollection<DiscardPileInfo>();
            foreach (var thisTemp in _tempDiscardList!)
            {
                if (thisTemp.Number1 > 0 && thisTemp.Number2 > 0 && thisTemp.Number1 < 16 && thisTemp.Number2 < 16)
                {
                    if (thisTemp.Number1 != thisTemp.Number2 && (thisTemp.Number1 + 1) != thisTemp.Number2)
                        output.Add(thisTemp);
                }
            }
            return output;
        }
        private CustomBasicList<DiscardPileInfo> SameList()
        {
            CustomBasicList<DiscardPileInfo> output = new CustomBasicList<DiscardPileInfo>();
            foreach (var thisTemp in _tempDiscardList!)
            {
                if (thisTemp.Number1 == thisTemp.Number2 && thisTemp.Number1 > 0 && thisTemp.Number2 > 0)
                    output.Add(thisTemp);
            }
            return output;
        }
        private ComputerDiscardInfo FindDiscardBasedOnOne()
        {
            ComputerDiscardInfo output = new ComputerDiscardInfo();
            output.Deck = 0;
            output.Pile = 0;
            CustomBasicList<DiscardPileInfo> ones;
            ones = ListOnes();
            if (ones.Count == 0)
                return output;
            ComputerDiscardInfo thisDiscard;
            CustomBasicList<ComputerDiscardInfo> newList = new CustomBasicList<ComputerDiscardInfo>(); // i think
            FlinchCardInformation thisCard;
            foreach (var thisTemp in ones)
            {
                var filters = _mainGame.SingleInfo!.MainHandList.Where(Items => Items.Number == thisTemp.Number1).ToRegularDeckDict();
                if (filters.Count() > 0)
                {
                    thisCard = filters.First();
                    thisDiscard = new ComputerDiscardInfo();
                    thisDiscard.Deck = thisCard.Deck;
                    thisDiscard.Pile = thisTemp.Pile;
                    newList.Add(thisDiscard);
                }
            }
            if (newList.Count > 0)
                return newList.GetRandomItem();
            int newNums;
            foreach (var thisTemp in ones)
            {
                newNums = thisTemp.Number1 - 1;
                if (newNums > 0 && newNums < 15)
                {
                    var Filters = _mainGame.SingleInfo!.MainHandList.Where(Items => Items.Number == newNums).ToRegularDeckDict();
                    if (Filters.Count() > 0)
                    {
                        thisCard = Filters.First();
                        thisDiscard = new ComputerDiscardInfo();
                        thisDiscard.Deck = thisCard.Deck;
                        thisDiscard.Pile = thisTemp.Pile;
                        newList.Add(thisDiscard);
                    }
                }
            }
            if (newList.Count > 0)
                return newList.GetRandomItem();
            return output;
        }
        private CustomBasicList<DiscardPileInfo> ListOnes()
        {
            return _tempDiscardList.Where(items => items.Number1 > 0 && items.Number2 == 0).ToCustomBasicList();
        }
        private int BestCardForEmpty()
        {
            if (_mainGame.SingleInfo!.MainHandList.Count == 1)
                return _mainGame.SingleInfo.MainHandList.First().Deck;
            return _mainGame.SingleInfo.MainHandList.GetRandomItem().Deck;
        }
        public FlinchComputerAI(FlinchMainGameClass mainGame)
        {
            _mainGame = mainGame;
        }
        public ComputerDiscardInfo ComputerDiscard()
        {
            ComputerDiscardInfo output = new ComputerDiscardInfo();
            if (AllDiscardEmpty() == true)
            {
                output.Pile = 0; // because 0 based.
                output.Deck = BestCardForEmpty();
                return output;
            }
            int piles;
            int deck;
            piles = EmptyDiscardPile();
            if (piles > -1)
            {
                deck = BestCardForEmpty();
                output.Pile = piles;
                output.Deck = deck;
                return output;
            }
            FillDiscardList();
            if (_tempDiscardList!.Count == 0)
                throw new BasicBlankException("There is nothing in the discard list.  Find out what happened");
            ComputerDiscardInfo finds;
            finds = FindDiscardBasedOnOne();
            if (finds.Deck > 0)
            {
                output.Pile = finds.Pile;
                output.Deck = finds.Deck;
                return output;
            }
            CustomBasicList<DiscardPileInfo> sames;
            sames = SameList();
            finds = FindBestSame(sames);
            if (finds.Deck > 0)
            {
                output.Pile = finds.Pile;
                output.Deck = finds.Deck;
                return output;
            }
            CustomBasicList<DiscardPileInfo> stacks;
            stacks = StackList();
            finds = FindBestStack(stacks);
            if (finds.Deck > 0)
            {
                output.Pile = finds.Pile;
                output.Deck = finds.Deck;
                return output;
            }
            CustomBasicList<DiscardPileInfo> nones;
            nones = NoStackNoSame();
            if (nones.Count > 0)
            {
                DiscardPileInfo temps;
                temps = nones.GetRandomItem();
                output.Pile = temps.Pile;
                deck = BestCardForEmpty();
                output.Deck = deck;
                return output;
            }
            int empties;
            empties = EmptyDiscardPile();
            if (empties > 0)
            {
                output.Pile = empties;
                deck = BestCardForEmpty();
                output.Deck = deck;
                return output;
            }
            finds = FindBestSame();
            if (finds.Deck > 0)
            {
                output.Pile = finds.Pile;
                output.Deck = finds.Deck;
                return output;
            }
            finds = FindBestStack();
            if (finds.Deck > 0)
            {
                output.Pile = finds.Pile;
                output.Deck = finds.Deck;
                return output;
            }
            DiscardPileInfo newFinds;
            newFinds = _tempDiscardList.GetRandomItem();
            output.Pile = newFinds.Pile;
            output.Deck = BestCardForEmpty();
            return output;
        }
        private FlinchCardInformation? FindDiscard(DiscardPilesVM<FlinchCardInformation> thisDiscard, int pile)
        {
            if (thisDiscard.PileList![pile].ObjectList.Count == 0)
                return null;
            return thisDiscard.GetLastCard(pile);
        }

        private int DiscardFromStack(int nextNumber, int stockNumber)
        {
            DiscardPilesVM<FlinchCardInformation> thisDiscard;
            thisDiscard = _tempDiscards!;
            FlinchCardInformation thisCard;
            int x;
            int y;
            int counts;
            for (x = 1; x <= HowManyDiscards; x++)
            {
                if (thisDiscard.PileList![x - 1].ObjectList.Count > 0)
                {
                    thisCard = FindDiscard(thisDiscard, x - 1)!;
                    if (thisCard.Number == nextNumber)
                    {
                        if (thisCard.Number == nextNumber)
                        {
                            // now can consider
                            if (thisCard.Number == stockNumber)
                                return x - 1;
                            counts = thisDiscard.PileList[x - 1].ObjectList.Count - 1;
                            for (y = counts; y >= 1; y += -1)
                            {
                                thisCard = FindDiscard(_tempDiscards!, x - 1)!;
                                if (thisCard!.Number == stockNumber)
                                    return x - 1;
                            }
                        }
                    }
                }
            }
            return -1; // because 0 based
        }
        private ComputerData NextMove(int nextNumber, int pile)
        {
            ComputerData output = new ComputerData();
            output.Pile = 0;
            output.WhichType = EnumCardType.IsNone;
            output.Discard = 0;
            string stockNumber;
            stockNumber = _mainGame.SingleInfo!.InStock;
            if (int.Parse(stockNumber) == nextNumber)
            {
                output.Pile = pile;
                output.ThisCard = _mainGame.SingleInfo!.StockPile!.GetCard();
                output.WhichType = EnumCardType.Stock;
                output.Discard = -1;
                return output;
            }
            int discards;
            discards = DiscardFromStack(nextNumber, int.Parse(stockNumber));
            if (discards > 0)
            {
                output.Pile = pile;
                output.WhichType = EnumCardType.Discard;
                output.Discard = discards;
                var ThisCard = FindDiscard(_tempDiscards!, discards);
                output.ThisCard = ThisCard;
                return output;
            }
            foreach (var thisCard in _tempHand!)
            {
                if (thisCard.Number == nextNumber)
                {
                    output.Pile = pile;
                    output.Discard = 0;
                    output.ThisCard = thisCard;
                    output.WhichType = EnumCardType.MyCards;
                    return output;
                }
            }
            int x;
            for (x = 1; x <= HowManyDiscards; x++)
            {
                var thisCard = FindDiscard(_tempDiscards!, x - 1);
                if (thisCard == null == false && thisCard!.Number == nextNumber)
                {
                    output.Pile = pile;
                    output.Discard = x - 1; // because 0 based
                    output.ThisCard = thisCard;
                    output.WhichType = EnumCardType.Discard;
                    return output;
                }
            }
            return output;
        }
        private MoveInfo ResultsOfMove(CustomBasicList<ComputerData> whatList, int whichOne)
        {
            MoveInfo output = new MoveInfo();
            output.MoveList = new CustomBasicList<ComputerData>();
            ComputerData thisMove;
            int numberNext;
            thisMove = whatList[whichOne - 1]; // i think 0 based (well see)
            MoveInfo tempMove;
            if (thisMove.WhichType == EnumCardType.Stock)
            {
                tempMove = new MoveInfo();
                tempMove.MoveList = new CustomBasicList<ComputerData>();
                tempMove.Value = 500; // can't try to block human anymore
                tempMove.StartNum = whichOne;
                tempMove.MoveList = new CustomBasicList<ComputerData>
                {
                    thisMove
                };
                return tempMove;
            }
            numberNext = _mainGame.PublicPiles!.NextNumberNeeded(thisMove.Pile);
            ComputerData newMove;
            output.MoveList.Add(thisMove);
            CopyDiscards(true);
            PopulateMove(ref output, thisMove);
            do
            {
                numberNext += 1;
                if (numberNext == 13)
                    numberNext = 1;
                newMove = NextMove(numberNext, thisMove.Pile);
                if (newMove.WhichType == EnumCardType.IsNone)
                {
                    return output;
                }
                output.MoveList.Add(newMove);
                PopulateMove(ref output, newMove);
                if ((int)newMove.WhichType == (int)EnumCardType.Stock || output.GetMore == true)
                    return output;
            }
            while (true);
        }
        private void PopulateMove(ref MoveInfo thisMove, ComputerData newMove)
        {
            int NewValue;
            NewValue = 0;
            int x;
            if (newMove.WhichType == EnumCardType.Stock)
            {
                NewValue += 400;
                if (thisMove.MoveList.Count > 1)
                {
                    var loopTo = thisMove.MoveList.Count;
                    for (x = 2; x <= loopTo; x++)
                        NewValue -= 20;
                }
            }
            FlinchCardInformation thisCard;
            thisCard = newMove.ThisCard!;
            if (newMove.WhichType == EnumCardType.MyCards)
            {
                _tempHand!.RemoveSpecificItem(thisCard); // i think
                if (_tempHand.Count == 0)
                {
                    thisMove.GetMore = true;
                    NewValue += 60;
                }
            }
            if (newMove.WhichType == EnumCardType.Discard)
            {
                NewValue += 20;
                _tempDiscards!.RemoveCard(newMove.Discard, newMove.ThisCard!.Deck);
                if (_mainGame.SingleInfo!.DiscardPiles!.PileList![newMove.Discard].ObjectList.Count == 0)
                    throw new BasicBlankException("Cannot be 0 for the discards");
            }
            thisMove.Value += NewValue;
        }
    }
}
