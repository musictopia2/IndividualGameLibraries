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
using FlorentineSolitaireCP.Logic;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using FlorentineSolitaireCP.Data;
using BasicGameFrameworkLibrary.SolitaireClasses.MainClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.PileInterfaces;

namespace FlorentineSolitaireCP.ViewModels
{
    [InstanceGame]
    public class FlorentineSolitaireMainViewModel : SolitaireMainViewModel<FlorentineSolitaireSaveInfo>
    {
        public FlorentineSolitaireMainViewModel(IEventAggregator aggregator,
            CommandContainer command, 
            IGamePackageResolver resolver
            )
            : base(aggregator, command, resolver)
        {
        }

        protected override SolitaireGameClass<FlorentineSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
        {
            return resolver.ReplaceObject<FlorentineSolitaireMainGameClass>();
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
