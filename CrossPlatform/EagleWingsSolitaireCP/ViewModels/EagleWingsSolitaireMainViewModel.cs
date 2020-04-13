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
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Attributes;
using CommonBasicStandardLibraries.Messenging;
using EagleWingsSolitaireCP.Logic;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using EagleWingsSolitaireCP.Data;
using BasicGameFrameworkLibrary.SolitaireClasses.MainClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.PileInterfaces;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;

namespace EagleWingsSolitaireCP.ViewModels
{
    [InstanceGame]
    public class EagleWingsSolitaireMainViewModel : SolitaireMainViewModel<EagleWingsSolitaireSaveInfo>
    {
        public DeckObservablePile<SolitaireCard> Heel1;
        public EagleWingsSolitaireMainViewModel(IEventAggregator aggregator,
            CommandContainer command, 
            IGamePackageResolver resolver
            )
            : base(aggregator, command, resolver)
        {
            GlobalClass.MainModel = this;
            Heel1 = new DeckObservablePile<SolitaireCard>(aggregator, command);
            Heel1.DeckClickedAsync += Heel1_DeckClickedAsync;
            Heel1.SendEnableProcesses(this, () => Heel1.CardsLeft() == 1);
        }
        private EagleWingsSolitaireMainGameClass? _mainGame;
        private async Task Heel1_DeckClickedAsync()
        {
            Heel1.IsSelected = true;
            await _mainGame!.HeelToMainAsync();
        }

        protected override SolitaireGameClass<EagleWingsSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
        {
            _mainGame = resolver.ReplaceObject<EagleWingsSolitaireMainGameClass>();
            return _mainGame;
        }
        private int _startingNumber;

        public int StartingNumber
        {
            get { return _startingNumber; }
            set
            {
                if (SetProperty(ref _startingNumber, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        protected override void CommandExecutingChanged()
        {
            StartingNumber = MainPiles1!.StartNumber();

        }
    }
}
