using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BlackjackCP.ViewModels;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BlackjackCP.Data
{
    public class PlayerStack : HandObservable<BlackjackCardInfo>
    {

        //seemed to work well last time so will do again.
        public event CardSelectedEventHandler? CardSelectedAsync;
        public delegate Task CardSelectedEventHandler(bool hasChoice);
        public BlackjackCardInfo? MainCard;
        public BlackjackCardInfo? SecondCard;

        private bool _mVarComputer;
        public void ProcessLabel(bool isComputer)
        {
            _mVarComputer = isComputer;
            if (isComputer == true)
                Text = "Computer Cards";
            else
                Text = "Human Cards";
        }

        protected async override Task ProcessObjectClickedAsync(BlackjackCardInfo thisObject, int index)
        {
            if (CardSelectedAsync == null)
                return; //not sure why the ? part does not work with async.
            if (thisObject.Value == EnumCardValueList.LowAce) //decided to make ace low.
                await CardSelectedAsync?.Invoke(true)!;
            else
                await CardSelectedAsync?.Invoke(false)!;
        }

        public void Reveal(int whichOne)
        {
            if (whichOne == 2 && _mVarComputer == false)
                throw new BasicBlankException("Only the computer can reveal the second one because it should have already been revealed to the human");
            if (whichOne < 1 || whichOne > 2)
                throw new BasicBlankException("Only 1 and 2 can be revealed, not " + whichOne);
            if (whichOne == 1)
                MainCard!.IsUnknown = false;
            else
                SecondCard!.IsUnknown = false; // already done for me because of data binding
        }
        public void AceChose(int whichOne, bool asOne)
        {
            if (whichOne == 1)
            {
                if (asOne == true)
                    MainCard!.Points = 1;
                else
                    MainCard!.Points = 11;
                return;
            }
            if (asOne == true)
                SecondCard!.Points = 1;
            else
                SecondCard!.Points = 11;
        }

        public void ComputerSelectFirst(ref bool hasChoice)
        {
            if (_mVarComputer == false)
                throw new BasicBlankException("Only the computer can select the first one");
            if (MainCard!.Value == EnumCardValueList.LowAce)
            {
                hasChoice = true; // well see
                return;
            }
            hasChoice = false;
        }

        public int CalculateScore(BlackjackMainViewModel model)
        {
            if (model.SelectedYet == false)
            {
                if (SecondCard!.Points == 0)
                    throw new BasicBlankException("Cannot have 0 points.  Find out what happened");
                return SecondCard.Points;
            }
            if (_mVarComputer == false)
            {
                if (HandList.Any(Items => Items.Points == 0))
                    throw new BasicBlankException("A card cannot be worth 0 points.  find out what happened");
            }
            return (HandList.Sum(Items => Items.Points));
        }
        public void HitMe(BlackjackCardInfo thisCard, ref bool hasChoice)
        {
            if (thisCard.Value == EnumCardValueList.LowAce) //decided to make it low now.
            {
                hasChoice = true;
                thisCard.Points = 0;
            }
            else
            {
                hasChoice = false;
                if ((int)thisCard.Value > 9)
                    thisCard.Points = 10;
                else
                    thisCard.Points = (int)thisCard.Value;
            }
            MainCard = thisCard;
            HandList.Add(thisCard);
        }

        public void ClearBoard(IDeckDict<BlackjackCardInfo> thisCol, ref bool hasChoice)
        {
            int x;
            if (thisCol.Count != 2)
                throw new BasicBlankException("Must have 2 cards only for clearing the board");
            BlackjackCardInfo thisCard;
            for (x = 1; x <= 2; x++)
            {
                thisCard = thisCol[x - 1];
                if (x == 2 && _mVarComputer == false)
                {
                }
                else
                    thisCard.IsUnknown = true;
                if (thisCard.Value == EnumCardValueList.LowAce)
                    thisCard.Points = 0;
                else if ((int)thisCard.Value > 9)
                    thisCard.Points = 10;
                else
                    thisCard.Points = (int)thisCard.Value;
                if (x == 1)
                    MainCard = thisCard;
                else
                    SecondCard = thisCard;
            }
            if (SecondCard!.Value == EnumCardValueList.LowAce)
                hasChoice = true;// has to be second one to start with.  the original probably had that bug.
            HandList.ReplaceRange(thisCol); // i think
        }

        public PlayerStack(CommandContainer command) : base(command)
        {
            AutoSelect = EnumAutoType.None;
        }
    }
}
