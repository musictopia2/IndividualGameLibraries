using BasicGameFramework.Attributes;
using BasicGameFramework.CombinationHelpers;
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace SinisterSixCP
{
    [SingletonGame]
    public class SinisterSixMainGameClass : DiceGameClass<EightSidedDice, SinisterSixPlayerItem, SinisterSixSaveInfo>, IMiscDataNM, IAdditionalRollProcess
    {
        public SinisterSixMainGameClass(IGamePackageResolver container) : base(container) { }
        private SinisterSixViewModel? _thisMod;
        private bool WasAuto;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<SinisterSixViewModel>();
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            AfterRestoreDice(); //i think
            SaveRoot!.LoadMod(_thisMod!);
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            LoadUpDice();
            IsLoaded = true; //i think needs to be here.
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            SetUpDice();
            SaveRoot!.ImmediatelyStartTurn = true;
            SaveRoot!.LoadMod(_thisMod!);
            SaveRoot.MaxRolls = 3;
            PlayerList!.ForEach(items => items.Score = 0);
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //leave warning for now.
            {
                case "removeselecteddice":
                    await RemoveSelectedDiceAsync();
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            PrepStartTurn(); //anything else is below.
            _thisMod!.ThisCup!.HowManyDice = 6;
            WasAuto = false;
            _thisMod.ThisCup.HideDice();
            _thisMod.ThisCup.CanShowDice = false;
            ProtectedStartTurn();
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        protected override async Task ProtectedAfterRollingAsync()
        {
            if (SaveRoot!.RollNumber > SaveRoot.MaxRolls)
            {
                if (ContainsSix() == false)
                {
                    WasAuto = true;
                    await EndTurnAsync();
                    return;
                }
            }
            await ContinueTurnAsync();
        }
        Task IAdditionalRollProcess.BeforeRollingAsync()
        {
            return Task.CompletedTask; //we have nothing else todo.  some times we do.  did not want to have to creae another interface just for that part.
        }
        async Task<bool> IAdditionalRollProcess.CanRollAsync()
        {
            if (SaveRoot!.RollNumber > 1)
            {
                if (ContainsSix() == true)
                {
                    await _thisMod!.ShowGameMessageAsync("Must remove any dice that equal 6");
                    return false;
                }
            }
            return true;
        }
        public async Task RemoveSelectedDiceAsync()
        {
            if (SingleInfo!.CanSendMessage(ThisData!) == true)
                await ThisNet!.SendAllAsync("removeselecteddice");
            _thisMod!.ThisCup!.RemoveSelectedDice();
            if (SaveRoot!.RollNumber > SaveRoot.MaxRolls && ContainsSix() == false)
            {
                WasAuto = true;
                await EndTurnAsync();
                return;
            }
            await ContinueTurnAsync();
        }
        private bool ContainsSix()
        {
            var thisList = _thisMod!.ThisCup!.DiceList.ToCustomBasicList();
            thisList.KeepConditionalItems(Items => Items.Value == 6);
            var temps = thisList.GetAllPossibleCombinations();
            return temps.Any(Items => Items.Sum(News => News.Value) == 6);
        }
        private async Task GameOverAsync()
        {
            SingleInfo = PlayerList.OrderBy(items => items.Score).Take(1).Single();
            await ShowWinAsync();
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo!.Score = _thisMod!.ThisCup!.DiceList.Sum(Items => Items.Value);
            if (WasAuto == true && ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(1); //if auto is done, needs to see what happened.
            if (WhoTurn == WhoStarts)
                SaveRoot!.MaxRolls = SaveRoot.RollNumber - 1;
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync(); //i think
            if (WhoTurn == WhoStarts)
            {
                await GameOverAsync();
                return;
            }
            await StartNewTurnAsync();
        }
    }
}