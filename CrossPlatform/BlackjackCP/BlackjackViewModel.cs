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
using BasicGameFramework.ViewModelInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.DIContainers;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using BasicGameFramework.CommandClasses; //its common to have command classes.
//i think this is the most common things i like to do
namespace BlackjackCP
{
    public class BlackjackViewModel : SimpleGameVM, ISoloCardGameVM<BlackjackCardInfo>
    {

        public PlayerStack? ComputerStack; //decided to make it more clear now.
        public PlayerStack? HumanStack;




        public enum EnumAceChoice
        {
            Low = 1,
            High = 2
        }

        private bool _NeedsAceChoice;
        public bool NeedsAceChoice
        {
            get
            {
                return _NeedsAceChoice;
            }

            set
            {
                if (SetProperty(ref _NeedsAceChoice, value) == true)
                {
                    Reprocess();
                }
            }
        }
        private bool _SelectedYet;

        public bool SelectedYet
        {
            get { return _SelectedYet; }
            set
            {
                if (SetProperty(ref _SelectedYet, value))
                {
                    Reprocess();
                }

            }
        }
        private void Reprocess()
        {
            if (NeedsAceChoice == false && SelectedYet == true)
                CanHitOrStay = true;
            else
                CanHitOrStay = false;
        }

        private bool _CanHitOrStay;
        public bool CanHitOrStay
        {
            get
            {
                return _CanHitOrStay;
            }

            set
            {
                if (SetProperty(ref _CanHitOrStay, value) == true)
                {
                }
            }
        }


        private int _HumanPoints;
        public int HumanPoints
        {
            get
            {
                return _HumanPoints;
            }

            set
            {
                if (SetProperty(ref _HumanPoints, value) == true)
                {
                }
            }
        }

        private int _ComputerPoints;
        public int ComputerPoints
        {
            get
            {
                return _ComputerPoints;
            }

            set
            {
                if (SetProperty(ref _ComputerPoints, value) == true)
                {
                }
            }
        }

        private int _Draws;
        public int Draws
        {
            get
            {
                return _Draws;
            }

            set
            {
                if (SetProperty(ref _Draws, value) == true)
                {
                }
            }
        }

        private int _Wins;
        public int Wins
        {
            get
            {
                return _Wins;
            }

            set
            {
                if (SetProperty(ref _Wins, value) == true)
                {
                }
            }
        }

        private int _Losses;
        public int Losses
        {
            get
            {
                return _Losses;
            }

            set
            {
                if (SetProperty(ref _Losses, value) == true)
                {
                }
            }
        }


        public BlackjackViewModel(ISimpleUI TempUI, IGamePackageResolver TempC) : base(TempUI, TempC)
        {
        }

        public DeckViewModel<BlackjackCardInfo>? DeckPile { get; set; }

        public async Task DeckClicked()
        {
            //if we click on deck, will do code for this.
            await Task.CompletedTask;
        }
        private BlackjackGameClass? _mainGame;

        public BasicGameCommand<EnumAceChoice>? AceCommand { get; set; }
        public BasicGameCommand? HitCommand { get; set; }
        public BasicGameCommand? StayCommand { get; set; }

        public override void Init()
        {
            _mainGame = MainContainer!.Resolve<BlackjackGameClass>();
            DeckPile = MainContainer.Resolve<DeckViewModel<BlackjackCardInfo>>(); //i think.
            CommandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged;
            DeckPile.NeverAutoDisable = true; //if it needs to autoreshuffle. do this.  if you don't want to allow reshuffling set to false.
            DeckPile.SendEnableProcesses(this, () =>
            {
                if (_mainGame.GameGoing == false)
                    return false;
                return true; //if other logic is needed for deck, put here.

            });

            HumanStack = new PlayerStack(this);
            HumanStack.CardSelectedAsync += HumanStack_CardSelectedAsync;
            ComputerStack = new PlayerStack(this);
            HumanStack.ProcessLabel(false);
            ComputerStack.ProcessLabel(true);
            ComputerStack.AlwaysDisabled = true;
            HumanStack.SendFunction(() => NeedsAceChoice == false && SelectedYet == false);
            HumanStack.Visible = true;
            ComputerStack.Visible = true;
            AceCommand = new BasicGameCommand<EnumAceChoice>(this, async Items =>
            {
                await _mainGame.HumanAceAsync(Items);
            }, Items =>
            {
                //if (NewGameVisible == true)
                //    return false;
                return NeedsAceChoice;
            }, this, CommandContainer);
            HitCommand = new BasicGameCommand(this, async Items =>
            {
                await _mainGame.HumanHitAsync();
            }, Items =>
            {
                //if (NewGameVisible == true)
                //    return false;
                return CanHitOrStay;
            }, this, CommandContainer);
            StayCommand = new BasicGameCommand(this, async Items =>
            await _mainGame.HumanStayAsync(), Items =>
            {
                //if (NewGameVisible == true)
                //    return false;
                return CanHitOrStay;
            }, this, CommandContainer);


        }
        private async Task HumanStack_CardSelectedAsync(bool HasChoice)
        {
            await _mainGame!.HumanSelectAsync(HasChoice);
        }
        private async void CommandContainer_ExecutingChanged()
        {
            if (CommandContainer!.IsExecuting)
                return;
            if (_mainGame!.GameGoing)
                await _mainGame.SaveStateAsync();
        }

        public override async Task StartNewGameAsync()
        {
            HumanPoints = 0;
            ComputerPoints = 0;
            SelectedYet = false;
            NewGameVisible = false;
            await _mainGame!.NewGameAsync();
        }
        public override bool CanEnableBasics() //since a person can do new game but still do other things.
        {
            return true;
        }
    }
}