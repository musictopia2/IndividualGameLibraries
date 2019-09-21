using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
namespace ConnectFourCP
{
    public class ConnectFourViewModel : SimpleBoardGameVM<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, ConnectFourPlayerItem, ConnectFourMainGameClass, int>
    {
        public ConnectFourViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        public BasicGameCommand<SpaceInfoCP>? ColumnCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            ColumnCommand = new BasicGameCommand<SpaceInfoCP>(this, async items =>
            {
                await MainGame!.MakeMoveAsync(items.Vector.Column);
            }, items =>
            {
                if (MainOptionsVisible == false)
                    return false;
                if (items.Vector.Column == 0)
                    throw new BasicBlankException("Should not be able to choose 0");
                return !(MainGame!.SaveRoot!.GameBoard.IsFilled(items.Vector.Column));
            }, this, CommandContainer!);
        }
    }
}