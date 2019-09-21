using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Dominos;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LottoDominosCP
{
    public class LottoDominosViewModel : BasicMultiplayerVM<LottoDominosPlayerItem, LottoDominosMainGameClass>
        , IControlsVisible

    {
        internal DominosBasicShuffler<SimpleDominoInfo>? DominosList;
        public LottoDominosViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }

        private int _NumberToChoose = -1;
        public int NumberToChoose
        {
            get { return _NumberToChoose; }
            set
            {
                if (SetProperty(ref _NumberToChoose, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        internal void ReloadLists()
        {
            Number1!.UnselectAll();
            NumberToChoose = -1;
            Number1.LoadNumberList(MainGame!.GetNumberList());
        }
        public NumberPicker? Number1;
        public async Task DominoClickedAsync(SimpleDominoInfo thisDomino)
        {
            await MainGame!.MakeMoveAsync(thisDomino.Deck);
        }
        public BasicGameCommand? ChooseNumberCommand { get; set; }
        protected override void EndInit()
        {
            DominosList = new DominosBasicShuffler<SimpleDominoInfo>(); //this time, maybe don't have to register it.
            Number1 = new NumberPicker(this);
            Number1.ChangedNumberValueAsync += Number1_ChangedNumberValueAsync;
            MainGame!.GameBoard1.SendEnableProcesses(this, () =>
            {
                return MainGame.SaveRoot!.GameStatus == EnumStatus.NormalPlay;
            });
            Number1.SendEnableProcesses(this, () => MainGame.SaveRoot!.GameStatus == EnumStatus.ChooseNumber);
            Number1.Visible = false; //start out with false this time.
            ChooseNumberCommand = new BasicGameCommand(this, async Items =>
            {
                await MainGame.ProcessNumberAsync(NumberToChoose);
            }, items =>
            {
                if (MainGame.SaveRoot!.GameStatus != EnumStatus.ChooseNumber)
                    return false;
                return NumberToChoose > -1;
            }, this, CommandContainer!);
        }
        public bool NumberVisible
        {
            get
            {
                if (MainGame!.SaveRoot!.GameStatus == EnumStatus.ChooseNumber)
                {
                    Number1!.Visible = true;
                    return true;
                }
                if (Number1 != null)
                    Number1.Visible = false;
                return false;
            }
        }
        private Task Number1_ChangedNumberValueAsync(int chosen)
        {
            NumberToChoose = chosen;
            return Task.CompletedTask;
        }
        public void MakeControlsVisible()
        {
            OnPropertyChanged(nameof(NumberVisible));
            if (MainGame!.SaveRoot!.GameStatus == EnumStatus.NormalPlay)
                MainGame.GameBoard1.Visible = true;
            else if (MainGame.GameBoard1 != null)
                MainGame.GameBoard1.Visible = false;
        }
    }
}