using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplePilesObservable;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using FlinchCP.Cards;
using System.Reflection;
using System.Threading.Tasks;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.

namespace FlinchCP.Piles
{
    public class PublicPilesViewModel : SimpleControlObservable
    {
        public CustomBasicCollection<BasicPileInfo<FlinchCardInformation>> PileList = new CustomBasicCollection<BasicPileInfo<FlinchCardInformation>>();
        public int NextNumberNeeded(int index)
        {
            var thisPile = PileList[index];
            if (thisPile.ObjectList.Count == 0)
                return 1;
            if (thisPile.ObjectList.Count > 15)
                throw new BasicBlankException("Should have cleared the piles because the numbers goes up to 15");
            return thisPile.ObjectList.Count + 1;
        }
        public void CreateNewPile(FlinchCardInformation thisCard) // because this appears to be the only game that has it this way. (?)
        {
            BasicPileInfo<FlinchCardInformation> ThisPile = new BasicPileInfo<FlinchCardInformation>();
            ThisPile.ObjectList.Add(thisCard);
            PileList.Add(ThisPile); // the bindings should do what it needs to (because of observable) well see
        }
        public void UnselectAllPiles()
        {
            foreach (var thisPile in PileList)
                thisPile.IsSelected = false;// i think its this simple (?)
        }
        public async Task<string> GetSavedPilesAsync()
        {
            return await js.SerializeObjectAsync(PileList);
        }
        public async Task LoadSavedPilesAsync(string thisStr)
        {
            PileList = await js.DeserializeObjectAsync<CustomBasicCollection<BasicPileInfo<FlinchCardInformation>>>(thisStr);
        }
        public void ClearBoard()
        {
            PileList.Clear(); // i think its as simple as clearing the pilelist (?)
        }
        public bool NeedToRemovePile(int pile)
        {
            if (PileList[pile].ObjectList.Count == 15)
                return true;
            if (PileList[pile].ObjectList.Count > 15)
                throw new BasicBlankException("Should have already cleared the pile");
            return false;
        }
        public DeckObservableDict<FlinchCardInformation> EmptyPileList(int whichOne)
        {
            var thisPile = PileList[whichOne];
            if (thisPile.ObjectList.Count != 15)
                throw new BasicBlankException($"Must have 15 cards to empty a pile; not {thisPile.ObjectList.Count}");
            var tempList = thisPile.ObjectList.ToObservableDeckDict();
            PileList.RemoveSpecificItem(thisPile); //you have to actually remove the pile on this game.
            if (tempList.Count != 15)
                throw new BasicBlankException($"Must have 15 cards in the list, not {tempList.Count}");
            return tempList;
        }
        public void AddCardToPile(int pile, FlinchCardInformation thisCard)
        {
            var thisPile = PileList[pile];
            thisPile.ObjectList.Add(thisCard); // i think its this simple
        }
        public int MaxPiles()
        {
            return PileList.Count;
        }
        public event PileClickedEventHandler? PileClickedAsync;

        public delegate Task PileClickedEventHandler(int index);

        public ControlCommand PileCommand { get; set; }


        private async Task PrivatePileAsync(BasicPileInfo<FlinchCardInformation> card)
        {
            if (PileClickedAsync == null)
                return;
            await PileClickedAsync.Invoke(PileList.IndexOf(card));
        }

        //public ControlCommand<BasicPileInfo<FlinchCardInformation>> PileCommand { get; set; }
        //public Command PileCommand { get; set; }

        public PublicPilesViewModel(CommandContainer command) : base(command)
        {
            MethodInfo method = this.GetPrivateMethod(nameof(PrivatePileAsync));
            PileCommand = new ControlCommand(this, method, command);

            //Visible = true;
        }
        protected override void EnableChange()
        {
            PileCommand.ReportCanExecuteChange();
        }
        protected override void PrivateEnableAlways() { }

        //protected override void VisibleChange() { }
    }
}