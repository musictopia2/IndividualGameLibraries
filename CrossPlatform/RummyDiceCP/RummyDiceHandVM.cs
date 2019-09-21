using BasicGameFramework.CommandClasses;
using BasicGameFramework.MiscViewModels;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace RummyDiceCP
{
    public class RummyDiceHandVM : SimpleControlViewModel
    {
        public ControlCommand BoardCommand { get; set; }
        public ControlCommand<RummyDiceInfo> DiceCommand { get; set; } //for this one, will select/unselect one.
        public CustomBasicCollection<RummyDiceInfo> HandList = new CustomBasicCollection<RummyDiceInfo>();
        public int Index { get; set; } //index is needed so it puts to correct one.
        readonly RummyDiceMainGameClass _mainGame;
        public RummyDiceHandVM(IBasicGameVM thisMod) : base(thisMod)
        {
            _mainGame = thisMod.MainContainer!.Resolve<RummyDiceMainGameClass>();
            Visible = true; //needs to be true this time.
            BoardCommand = new ControlCommand(this, async items =>
            {
                if (_mainGame.ThisData!.MultiPlayer == true)
                    await _mainGame.ThisNet!.SendAllAsync("setchosen", Index);
                await _mainGame.SetProcessAsync(Index);
            }, thisMod, thisMod.CommandContainer!);
            DiceCommand = new ControlCommand<RummyDiceInfo>(this, async thisDice =>
            {
                int diceN = HandList.IndexOf(thisDice);
                if (_mainGame.ThisData!.MultiPlayer == true)
                {
                    SendSet thisSet = new SendSet();
                    thisSet.WhichSet = Index;
                    thisSet.Dice = diceN;
                    await _mainGame.ThisNet!.SendAllAsync("diceset", thisSet);
                }
                await SelectUnselectDiceAsync(diceN);
            }, thisMod, thisMod.CommandContainer!);
        }
        public async Task SelectUnselectDiceAsync(int index)
        {
            HandList[index].IsSelected = !HandList[index].IsSelected;
            await _mainGame.ContinueTurnAsync();
        }
        protected override void EnableChange()
        {
            BoardCommand.ReportCanExecuteChange();
            DiceCommand.ReportCanExecuteChange(); //i think you have to do manually since its a controlcommand.
        }
        protected override void PrivateEnableAlways() { }

        public void PopulateTiles(ICustomBasicList<RummyDiceInfo> thisList)
        {
            HandList.ReplaceRange(thisList);
            HandList.Sort(); //i think
        }
        public void TransferTiles(ICustomBasicList<RummyDiceInfo> thisList)
        {
            HandList.AddRange(thisList);
            HandList.ForEach(Items => Items.IsSelected = false); //make sure its all unselected.
            HandList.Sort();
        }
        public ICustomBasicList<RummyDiceInfo> GetSelectedDiceAndRemove() //i think returning an interface is acceptable.
        {
            return HandList.RemoveAllAndObtain(Items => Items.IsSelected == true);
        }
        public void EndTurn()
        {
            HandList.Clear();
        }
        protected override void VisibleChange() { }
    }
}