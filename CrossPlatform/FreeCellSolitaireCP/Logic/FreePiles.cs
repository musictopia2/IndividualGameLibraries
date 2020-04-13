using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.MultiplePilesObservable;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
namespace FreeCellSolitaireCP.Logic
{
    public class FreePiles : BasicMultiplePilesCP<SolitaireCard>
    {
        public int HowManyFreeCells
        {
            get
            {
                int output = 0;
                4.Times(x =>
                {
                    if (HasCard(x - 1) == false)
                        output++;
                });
                return output;
            }

        }
        public void ForceSelected(int index)
        {
            if (index == -1)
            {
                UnselectPile(OneSelected);
                return;
            }
            OneSelected = index;
        }

        public SolitaireCard GetCard()
        {
            if (OneSelected == -1)
                throw new BasicBlankException("There is no card selected");
            return GetLastCard(OneSelected);
        }
        public int OneSelected { get; private set; } = -1;

        public void SelectUnselectCard(int whichOne)
        {
            if (OneSelected > -1 && OneSelected != whichOne)
                throw new BasicBlankException($"Cannot select one because {OneSelected} was already selected");
            if (HasCard(whichOne) == false)
                throw new BasicBlankException("There is no card to select");
            if (whichOne == OneSelected)
                OneSelected = -1;
            else
                OneSelected = whichOne;
            SelectUnselectSinglePile(whichOne);
        }
        public void RemoveCard()
        {
            if (OneSelected == -1)
                throw new BasicBlankException("There was no card selected to even remove");
            RemoveCardFromPile(OneSelected);
            UnselectPile(OneSelected);
            OneSelected = -1;
        }
        //use addcardtopile instead of addcard.
        public override void ClearBoard()
        {
            OneSelected = -1;
            base.ClearBoard();
        }

        public FreePiles(CommandContainer command, IEventAggregator aggregator) : base(command, aggregator)
        {
            Columns = 4;
            Rows = 1;
            Style = EnumStyleList.SingleCard;
            HasText = false;
            HasFrame = true;
            LoadBoard();
        }
    }
}
