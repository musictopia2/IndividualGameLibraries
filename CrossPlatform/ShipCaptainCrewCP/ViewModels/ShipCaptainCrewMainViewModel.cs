using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using ShipCaptainCrewCP.Data;
using ShipCaptainCrewCP.Logic;
namespace ShipCaptainCrewCP.ViewModels
{
    [InstanceGame]
    public class ShipCaptainCrewMainViewModel : DiceGamesVM<SimpleDice>
    {
        public ShipCaptainCrewMainViewModel(CommandContainer commandContainer,
            ShipCaptainCrewMainGameClass mainGame,
            ShipCaptainCrewVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            IStandardRollProcesses roller
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver, roller)
        {
        }
        //anything else needed is here.
        protected override bool CanEnableDice()
        {
            return true; //if you can enable dice, change the routine.
        }
        public override bool CanEndTurn()
        {
            return false; //if you can't or has extras, do here.
        }
        public override bool CanRollDice()
        {
            return true;
        }

    }
}