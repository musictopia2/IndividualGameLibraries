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
using BasicGameFramework.MainViewModels;
using BasicGameFramework.DIContainers;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.NetworkingClasses.Extensions;

namespace SorryCP
{
    public class SorryViewModel : SimpleBoardGameVM<EnumColorChoice, PawnPiecesCP<EnumColorChoice>, SorryPlayerItem, SorryMainGameClass, int>
    {
        internal GameBoardProcesses? GameBoard1;
        private string _CardDetails = "";
        public string CardDetails
        {
            get { return _CardDetails; }
            set
            {
                if (SetProperty(ref _CardDetails, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _TempSpace; //not sure if we still need (?)
        public int TempSpace
        {
            get { return _TempSpace; }
            set
            {
                if (SetProperty(ref _TempSpace, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public override bool CanEndTurn()
        {
            bool rets = base.CanEndTurn();
            if (rets == false)
                return false;
            return !GameBoard1!.HasRequiredMove;
        }
        private bool CanClickSpace()
        {
            return MainGame!.SaveRoot!.DidDraw;
        }
        public SorryViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        public BasicGameCommand<int>? SpaceCommand { get; set; }
        public BasicGameCommand<EnumColorChoice>? HomeCommand { get; set; }
        public BasicGameCommand? DrawCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            SpaceCommand = new BasicGameCommand<int>(this, async space =>
            {
                if (GameBoard1!.IsValidMove(space) == false)
                    return;
                if (ThisData!.MultiPlayer)
                    await ThisNet!.SendMoveAsync(space);
                await MainGame!.MakeMoveAsync(space);
            }, items =>
            {
                if (MainGame!.DidChooseColors == false)
                    return false;
                return CanClickSpace();
            }, this, CommandContainer!);
            HomeCommand = new BasicGameCommand<EnumColorChoice>(this, async color =>
            {
                if (GameBoard1!.CanGoHome(color) == false)
                    return;
                if (ThisData!.MultiPlayer)
                    await ThisNet!.SendMoveAsync(100);
                await MainGame!.MakeMoveAsync(100); //100 means going home.
            }, items =>
            {
                if (MainGame!.DidChooseColors == false)
                    return false;
                return CanClickSpace();
            }, this, CommandContainer!);
            DrawCommand = new BasicGameCommand(this, async items =>
            {
                if (ThisData!.MultiPlayer)
                    await ThisNet!.SendDrawAsync();
                await MainGame!.DrawCardAsync(); //forgot this.
            }, items =>
            {
                if (MainGame!.DidChooseColors == false)
                    return false;
                return !MainGame.SaveRoot!.DidDraw;
            }, this, CommandContainer!);
            GameBoard1 = MainContainer!.Resolve<GameBoardProcesses>();
        }
    }
}