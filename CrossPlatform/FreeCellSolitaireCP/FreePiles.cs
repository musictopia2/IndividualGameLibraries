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
using BasicGameFramework.MultiplePilesViewModels;
using BaseSolitaireClassesCP.Cards;
using BasicGameFramework.ViewModelInterfaces;
//i think this is the most common things i like to do
namespace FreeCellSolitaireCP
{
    public class FreePiles : BasicMultiplePilesCP<SolitaireCard>
    {

        private int _previousSelected = -1;

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
                UnselectPile(_previousSelected);
                return;
            }
            _previousSelected = index;
        }

        public SolitaireCard GetCard()
        {
            if (_previousSelected == -1)
                throw new BasicBlankException("There is no card selected");
            return GetLastCard(_previousSelected);
        }
        public int OneSelected => _previousSelected;

        public void SelectUnselectCard(int whichOne)
        {
            if (_previousSelected > -1 && _previousSelected != whichOne)
                throw new BasicBlankException($"Cannot select one because {_previousSelected} was already selected");
            if (HasCard(whichOne) == false)
                throw new BasicBlankException("There is no card to select");
            if (whichOne == _previousSelected)
                _previousSelected = -1;
            else
                _previousSelected = whichOne;
            SelectUnselectSinglePile(whichOne);
        }
        public void RemoveCard()
        {
            if (_previousSelected == -1)
                throw new BasicBlankException("There was no card selected to even remove");
            RemoveCardFromPile(_previousSelected);
            UnselectPile(_previousSelected);
            _previousSelected = -1;
        }
        //use addcardtopile instead of addcard.
        public override void ClearBoard()
        {
            _previousSelected = -1;
            base.ClearBoard();
        }

        public FreePiles(IBasicGameVM ThisMod) : base(ThisMod)
        {
            Columns = 4;
            Rows = 1;
            Visible = true; //just in case.
            Style = EnumStyleList.SingleCard;
            HasText = false;
            HasFrame = true;
            LoadBoard();
        }
    }
}