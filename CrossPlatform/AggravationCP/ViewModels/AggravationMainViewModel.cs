using AggravationCP.Data;
using AggravationCP.Logic;
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
//i think this is the most common things i like to do
namespace AggravationCP.ViewModels
{
    [InstanceGame]
    public class AggravationMainViewModel : BoardDiceGameVM
    {
        private readonly AggravationMainGameClass _mainGame; //if we don't need, delete.

        public AggravationMainViewModel(CommandContainer commandContainer,
            AggravationMainGameClass mainGame,
            AggravationVMData model,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            IStandardRollProcesses roller
            )
            : base(commandContainer, mainGame, model, basicData, test, resolver, roller)
        {
            _mainGame = mainGame;
        }
        //anything else needed is here.
        public override bool CanRollDice()
        {
            return _mainGame.SaveRoot.DiceNumber == 0;
        }
        public override async Task RollDiceAsync() //if any changes, do here.
        {
            await base.RollDiceAsync();
        }
    }
}