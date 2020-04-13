using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.MiscProcesses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using RummyDiceCP.Data;
using RummyDiceCP.Logic;
using System.Reflection;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace RummyDiceCP.ViewModels
{
    public class RummyDiceHandVM : SimpleControlObservable
    {
        public ControlCommand BoardCommand { get; set; }

        public ControlCommand DiceCommand { get; set; }

        public CustomBasicCollection<RummyDiceInfo> HandList = new CustomBasicCollection<RummyDiceInfo>();
        public int Index { get; set; } //index is needed so it puts to correct one.
        private readonly RummyDiceMainGameClass _mainGame;

        private async Task PrivateSetChosenAsync()
        {
            if (_mainGame.BasicData!.MultiPlayer == true)
                await _mainGame.Network!.SendAllAsync("setchosen", Index);
            await _mainGame.SetProcessAsync(Index);
        }
        private async Task DiceChosenAsync(RummyDiceInfo dice)
        {
            int x = HandList.IndexOf(dice);
            if (_mainGame.BasicData!.MultiPlayer == true)
            {
                SendSet thisSet = new SendSet();
                thisSet.WhichSet = Index;
                thisSet.Dice = x;
                await _mainGame.Network!.SendAllAsync("diceset", thisSet);
            }
            await SelectUnselectDiceAsync(x);
        }
        public RummyDiceHandVM(CommandContainer command, RummyDiceMainGameClass mainGame) : base(command)
        {
            //Visible = true; //needs to be true this time.
            _mainGame = mainGame;

            MethodInfo method = this.GetPrivateMethod(nameof(PrivateSetChosenAsync));
            BoardCommand = new ControlCommand(this, method, command);
            method = this.GetPrivateMethod(nameof(DiceChosenAsync));
            DiceCommand = new ControlCommand(this, method, command);

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
    }
}
