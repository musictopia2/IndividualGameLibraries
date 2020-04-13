using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;
using CommonBasicStandardLibraries.Exceptions;
using Rummy500CP.Data;
using System.Linq;

namespace Rummy500CP.Logic
{
    public class RummySet : SetInfo<EnumSuitList, EnumColorList, RegularRummyCard, SavedSet>
    {
        private bool _useSecond;
        private EnumWhatSets _setType;
        private readonly Rummy500GameContainer _gameContainer;
        public RummySet(Rummy500GameContainer gameContainer) : base(gameContainer.Command)
        {
            CanExpandRuns = true;
            _gameContainer = gameContainer;
        }
        protected override bool CanClickMainBoard()
        {
            return _gameContainer.BasicData!.IsXamarinForms == false; //only desktops can.
        }
        protected override bool IsRun()
        {
            return _setType == EnumWhatSets.runs;
        }
        public override void LoadSet(SavedSet payLoad)
        {
            HandList.ReplaceRange(payLoad.CardList);
            _setType = payLoad.SetType;
            _useSecond = payLoad.UseSecond;
        }
        public override SavedSet SavedSet()
        {
            SavedSet output = new SavedSet();
            output.CardList = HandList.ToRegularDeckDict();
            output.UseSecond = _useSecond;
            output.SetType = _setType;
            return output;
        }
        public void CreateNewSet(IDeckDict<RegularRummyCard> thisCol, EnumWhatSets whatSet, bool useSecond)
        {
            _useSecond = useSecond;
            _setType = whatSet;
            DeckRegularDict<RegularRummyCard> output;
            if (useSecond == true)
                output = thisCol.OrderBy(items => items.SecondNumber).ToRegularDeckDict();
            else
                output = thisCol.OrderBy(items => items.Value).ToRegularDeckDict();
            thisCol.ForEach(thisCard =>
            {
                thisCard.Player = _gameContainer.WhoTurn;
                thisCard.Drew = false;
                thisCard.IsSelected = false;
            });
            HandList.ReplaceRange(output);
        }
        public void AddCard(RegularRummyCard thisCard, int position)
        {
            thisCard.IsSelected = false;
            thisCard.Drew = true; //others has to know what was played.
            thisCard.Player = _gameContainer.WhoTurn;
            if (position == 1)
                HandList.InsertBeginning(thisCard);
            else
                HandList.Add(thisCard);
        }
        public PointInfo GetPointInfo(int player)
        {
            PointInfo output = new PointInfo();
            int x = 0;
            HandList.ForEach(thisCard =>
            {
                x++;
                if (thisCard.Player == player)
                {
                    output.NumberOfCards++;
                    if (x == 1 && _setType == EnumWhatSets.runs && thisCard.SecondNumber == EnumCardValueList.LowAce)
                        output.Points += 5;
                    else if (thisCard.Value == EnumCardValueList.HighAce || thisCard.SecondNumber == EnumCardValueList.LowAce)
                        output.Points += 15;
                    //else if (_setType == EnumWhatSets.kinds && thisCard.Value == EnumCardValueList.LowAce)
                    //    output.Points += 15; //if its in a kind, its 15 period.  this is the best way to handle that.
                    //else if (thisCard.Value == EnumCardValueList.HighAce)
                    //    output.Points += 15;
                    else if (thisCard.Value < EnumCardValueList.Eight)
                        output.Points += 5;

                    else
                        output.Points += 10;
                }
            });
            return output;
        }
        public int CardNeeded(int position)
        {
            RegularRummyCard thisCard;
            if (_setType == EnumWhatSets.kinds || position == 1)
                thisCard = HandList.First();
            else
                thisCard = HandList.Last();
            int x;
            if (_setType == EnumWhatSets.kinds)
            {
                if (HandList.Count == 4)
                    return 0; //because all 4 are taken.
                for (x = 1; x <= 4; x++)
                {
                    if (HandList.Any(y => y.Suit == (EnumSuitList)x) == false)
                        return _gameContainer.DeckList.Single(y => y.Suit == (EnumSuitList)x && y.Value == thisCard.Value).Deck;
                }
                throw new BasicBlankException("Cannot find the card needed for kinds");
            }
            if (position == 1)
            {
                if (thisCard.SecondNumber == EnumCardValueList.LowAce) //since i don't know which is which.  needs to cover both cases.
                    return 0;
                EnumCardValueList LowerValue;
                LowerValue = thisCard.Value - 1;
                return _gameContainer.DeckList.Single(x => x.SecondNumber == LowerValue && x.Suit == thisCard.Suit).Deck;
            }
            if (thisCard.Value == EnumCardValueList.HighAce) //has to lean towards low.  if i do something else, i risk breaking other rummy games.
                return 0;//because nothing higher than high ace.
            EnumCardValueList higherValue = thisCard.Value + 1;
            return _gameContainer.DeckList.Single(Items => Items.Value == higherValue && Items.Suit == thisCard.Suit).Deck; //hopefully this simple.
        }
        public int PositionToPlay(RegularRummyCard thisCard)
        {
            int thisNeed = CardNeeded(2);
            if (thisNeed > 0)
            {
                if (thisCard.Deck == thisNeed)
                    return 2;
            }
            thisNeed = CardNeeded(1);
            if (thisNeed > 0)
            {
                if (thisCard.Deck == thisNeed)
                    return 1;
            }
            return 0;
        }
    }
}
