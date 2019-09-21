using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Dominos;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace DiceDominosCP
{
    public class DiceDominosViewModel : DiceGamesVM<SimpleDice, DiceDominosPlayerItem, DiceDominosMainGameClass>
    {
        internal DominosBasicShuffler<SimpleDominoInfo>? DominosList;
        public DiceDominosViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        public async Task DominoClickedAsync(SimpleDominoInfo thisDomino)
        {
            if (MainGame!.GameBoard1!.IsValidMove(thisDomino.Deck) == false)
            {
                await ShowGameMessageAsync("Illegal Move");
                return;
            }
            await MainGame!.MakeMoveAsync(thisDomino.Deck);
        }
        public BasicGameCommand? ChooseNumberCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            DominosList = new DominosBasicShuffler<SimpleDominoInfo>();
        }
        protected override bool CanEnableDice()
        {
            if (MainGame!.SaveRoot!.HasRolled == false || MainGame.SaveRoot.DidHold == true)
                return false;
            return true;
        }
        public override bool CanEndTurn()
        {
            return MainGame!.SaveRoot!.HasRolled;
        }
        protected override bool CanRollDice()
        {
            return !MainGame!.SaveRoot!.HasRolled;
        }
        protected override void FinishCup()
        {
            ThisCup!.HowManyDice = 2; //you specify how many dice here.
            ThisCup.Visible = true; //i think.
            ThisCup.ShowHold = true;
        }
    }
}