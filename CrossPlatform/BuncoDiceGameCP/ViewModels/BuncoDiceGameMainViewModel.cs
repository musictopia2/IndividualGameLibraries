using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using BuncoDiceGameCP.Data;
using BuncoDiceGameCP.EventModels;
using BuncoDiceGameCP.Logic;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BuncoDiceGameCP.ViewModels
{
    [InstanceGame]
    public class BuncoDiceGameMainViewModel : Screen,
        IBasicEnableProcess,
        IBlankGameVM,
        IAggregatorContainer,
        IHandleAsync<ChoseNewRoundEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly IGamePackageResolver _resolver;
        private readonly ISaveSinglePlayerClass _state;
        private readonly GlobalClass _global;
        private readonly BuncoDiceGameMainGameClass _mainGame;
        public DiceCup<SimpleDice>? ThisCup;

        #region Properties
        private bool _canEndTurn;
        public bool CanEndTurn
        {
            get
            {
                return _canEndTurn;
            }

            set
            {
                if (SetProperty(ref _canEndTurn, value) == true)
                {
                }
            }
        }

        private bool _alreadyReceivedBunco;
        public bool AlreadyReceivedBunco
        {
            get
            {
                return _alreadyReceivedBunco;
            }

            set
            {
                if (SetProperty(ref _alreadyReceivedBunco, value) == true)
                {
                }
            }
        }

        #endregion


        public BuncoDiceGameMainViewModel(IEventAggregator aggregator,
            CommandContainer commandContainer,
            IGamePackageResolver resolver,
            ISaveSinglePlayerClass state,
            GlobalClass global
            )
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            CommandContainer = commandContainer;
            _resolver = resolver;
            _state = state;
            _global = global;
            CommandContainer.ManuelFinish = true;
            CommandContainer.IsExecuting = true; //not sure.
            _mainGame = resolver.ReplaceObject<BuncoDiceGameMainGameClass>(); //hopefully this works.  means you have to really rethink.
        }

        public CommandContainer CommandContainer { get; set; }
        //looks like i did as standard event instead of interfaces.  because overflows happened on multiplayer.
        IEventAggregator IAggregatorContainer.Aggregator => _aggregator;

        public bool CanEnableBasics()
        {
            return _global.IsActive; //because maybe you can't enable it.
        }
        protected override async Task ActivateAsync()
        {
            await base.ActivateAsync();
            _global.IsActive = true;
            await _mainGame.StartGameAsync(GetLoadedCup);
        }
        private BuncoDiceGameSaveInfo? _saveroot;
        private DiceCup<SimpleDice> GetLoadedCup(BuncoDiceGameSaveInfo saveRoot, bool autoResume)
        {
            _saveroot = saveRoot; //hopefully this simple (?)
            if (ThisCup != null)
            {
                return ThisCup;
            }
            ThisCup = new DiceCup<SimpleDice>(saveRoot.DiceList, _resolver, CommandContainer);
            ThisCup.HowManyDice = 3;
            ThisCup.ShowDiceListAlways = true;
            if (autoResume)
            {
                ThisCup.ClearDice();
            }
            else
            {
                ThisCup.CanShowDice = true;
            }
            return ThisCup;
        }

        Task IHandleAsync<ChoseNewRoundEventModel>.HandleAsync(ChoseNewRoundEventModel message)
        {
            return _mainGame.ProcessNewRoundAsync(); //hopefully this simple (?)
        }

        public bool CanRoll => !CanEndTurn;
        [Command(EnumCommandCategory.Game)]
        public async Task RollAsync()
        {
            await _mainGame.RollDiceAsync();
            int score = _mainGame.ScoreRoll();
            if (score == 0)
            {
                CanEndTurn = true; //could be iffy.
                return;
            }
            _mainGame.UpdateScores(score);
            _saveroot!.HasRolled = true;
            await _state.SaveSimpleSinglePlayerGameAsync(_saveroot); //needs to save.  this could have been a serious bug.
        }
        public bool CanBunco()
        {
            if (CanEndTurn)
            {
                return false;
            }
            if (AlreadyReceivedBunco)
            {
                return false;
            }
            return _saveroot!.HasRolled;
        }
        [Command(EnumCommandCategory.Game)]
        public async Task BuncoAsync()
        {
            CommandContainer.ManuelFinish = false;
            if (_mainGame.DidHaveBunco() == false)
            {
                await UIPlatform.ShowMessageAsync("Sorry, there is no bunco here");
                return;
            }
            _mainGame.UpdateScores(16);
            _mainGame.ReceivedBunco();
            _saveroot!.ThisStats.YourPoints = _mainGame.CurrentPlayer!.Points;
            _saveroot.ThisStats.Buncos = _mainGame.CurrentPlayer.Buncos;
            if (_mainGame.CurrentPlayer.Table == 1)
            {
                await _mainGame.FinishRoundAsync(); // because a bunco has been received and you are hosting.  if you are not hosting, then round does not end right away.
                return;
            }
            AlreadyReceivedBunco = true;
        }
        public bool CanHuman21()
        {
            //return false;
            if (CanEndTurn == false)
            {
                return false;
            }
            if (_saveroot!.HasRolled == false)
            {
                return false;
            }
            return _mainGame.CurrentPlayer!.Table == 1;
        }
        [Command(EnumCommandCategory.Game)]
        public async Task Human21Async()
        {
            CommandContainer.ManuelFinish = false;
            if (_mainGame!.CurrentPlayer!.Points < 21)
            {
                await UIPlatform.ShowMessageAsync("Sorry, you do not have 21 points.  Therefore, yuo cannot end the round");
                return;
            }
            if (_mainGame.CurrentPlayer.Table > 1)
            {
                await UIPlatform.ShowMessageAsync("Sorry, you cannot end this round because you are not hosting this round.");
                return;
            }
            await _mainGame.FinishRoundAsync();
        }
        [Command(EnumCommandCategory.Game)]
        public async Task EndTurnAsync()
        {
            CanEndTurn = false;
            AlreadyReceivedBunco = false;
            await _mainGame.EndTurnAsync();
        }
    }
}