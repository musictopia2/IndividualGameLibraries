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
using BaseSolitaireClassesCP.MainClasses;
using BasicGameFramework.DIContainers;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using BasicGameFramework.DrawableListsViewModels;
using BaseSolitaireClassesCP.Cards;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFramework.CommandClasses;
//i think this is the most common things i like to do
namespace EagleWingsSolitaireCP
{
    public class EagleWingsSolitaireViewModel : SolitaireMainViewModel<EagleWingsSolitaireSaveInfo>
    {
        public EagleWingsSolitaireViewModel(ISimpleUI tempUI, IGamePackageResolver tempC) : base(tempUI, tempC) { }
        private int _StartingNumber;

        public int StartingNumber
        {
            get { return _StartingNumber; }
            set
            {
                if (SetProperty(ref _StartingNumber, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        protected override void CommandExecutingChanged()
        {
            StartingNumber = MainPiles1!.StartNumber();

        }
        public DeckViewModel<SolitaireCard>? Heel1;
        public override void Init()
        {
            base.Init();
            EventAggregator thisE = MainContainer!.Resolve<EventAggregator>();
            CustomHeelClass thisC = new CustomHeelClass(this);
            Heel1 = new DeckViewModel<SolitaireCard>(thisE, thisC);
            Heel1.Visible = true;
            Heel1.SendEnableProcesses(this, () => Heel1.CardsLeft() == 1);
        }
    }
    public class CustomHeelClass : IDeckClick
    {
        private readonly EagleWingsSolitaireViewModel _thisMod;
        private readonly EagleWingsSolitaireGameClass _mainGame;

        public CustomHeelClass(EagleWingsSolitaireViewModel thisMod)
        {
            _thisMod = thisMod;
            _mainGame = _thisMod.MainContainer!.Resolve<EagleWingsSolitaireGameClass>();
        }

        public PlainCommand? NewGameCommand { get => _thisMod.NewGameCommand; set => _thisMod.NewGameCommand = value; }
        public IGamePackageResolver? MainContainer { get => _thisMod.MainContainer; set => _thisMod.MainContainer = value; }
        public bool NewGameVisible { get => _thisMod.NewGameVisible; set => _thisMod.NewGameVisible = value; }
        public CommandContainer? CommandContainer { get => _thisMod.CommandContainer; set => _thisMod.CommandContainer = value; }

        public async Task DeckClicked()
        {
            _thisMod.Heel1!.IsSelected = true;
            await _mainGame.HeelToMainAsync();
        }

        public Task HandleErrorAsync(Exception ex)
        {
            return ((IDeckClick)_thisMod).HandleErrorAsync(ex);
        }

        public void Init()
        {
            ((IDeckClick)_thisMod).Init();
        }

        public Task ShowGameMessageAsync(string Message)
        {
            return ((IDeckClick)_thisMod).ShowGameMessageAsync(Message);
        }
    }

}