using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Linq;
namespace ChineseCheckersCP
{
    public class ChineseCheckersViewModel : SimpleBoardGameVM<EnumColorList, MarblePiecesCP<EnumColorList>, ChineseCheckersPlayerItem, ChineseCheckersMainGameClass, int>
    {
        public ChineseCheckersViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        public BasicGameCommand<int>? SpaceCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            SpaceCommand = new BasicGameCommand<int>(this, async index =>
            {
                if (MainGame!.SaveRoot!.PreviousSpace == index && MainGame.GameBoard1!.WillContinueTurn() == false)
                {
                    MainGame.SaveRoot.Instructions = "Choose a piece to move";
                    if (ThisData!.MultiPlayer == true)
                        await ThisNet!.SendAllAsync("undomove");
                    await MainGame.GameBoard1.UnselectPieceAsync();
                    return;
                }
                else if (MainGame.SaveRoot.PreviousSpace == index)
                    return;
                if (MainGame.GameBoard1!.IsValidMove(index) == false)
                    return;
                if (MainGame.SaveRoot.PreviousSpace == 0)
                {
                    if (MainGame.SingleInfo!.PieceList.Any(Items => Items == index) == false)
                        return;
                    MainGame.SaveRoot.Instructions = "Select where to move to";
                    if (ThisData!.MultiPlayer == true)
                        await ThisNet!.SendAllAsync("pieceselected", index);
                    await MainGame.GameBoard1.HighlightItemAsync(index);
                    return;
                }
                MainGame.SaveRoot.Instructions = "Making Move";
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendMoveAsync(index);
                await MainGame.MakeMoveAsync(index);
            }, items =>
            {
                return MainGame!.DidChooseColors;
            }, this, CommandContainer!);
        }
        public override bool CanEndTurn()
        {
            return MainGame!.GameBoard1!.WillContinueTurn();
        }
    }
}