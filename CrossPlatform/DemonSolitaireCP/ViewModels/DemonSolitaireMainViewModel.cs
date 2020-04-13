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
using DemonSolitaireCP.Logic;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using DemonSolitaireCP.Data;
using BasicGameFrameworkLibrary.SolitaireClasses.MainClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.PileInterfaces;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;

namespace DemonSolitaireCP.ViewModels
{
    [InstanceGame]
    public class DemonSolitaireMainViewModel : SolitaireMainViewModel<DemonSolitaireSaveInfo>
    {
        public DemonSolitaireMainViewModel(IEventAggregator aggregator,
            CommandContainer command, 
            IGamePackageResolver resolver
            )
            : base(aggregator, command, resolver)
        {
            GlobalClass.MainModel = this;
            Heel1 = new DeckObservablePile<SolitaireCard>(aggregator, command);
            Heel1.SendEnableProcesses(this, () => false);
            Heel1.DeckStyle = DeckObservablePile<SolitaireCard>.EnumStyleType.AlwaysKnown;
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
        public DeckObservablePile<SolitaireCard> Heel1;

        protected override SolitaireGameClass<DemonSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
        {
            return resolver.ReplaceObject<DemonSolitaireMainGameClass>();
        }
        //anything else needed is here.
    }
}
