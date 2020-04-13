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
using PersianSolitaireCP.Logic;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using PersianSolitaireCP.Data;
using BasicGameFrameworkLibrary.SolitaireClasses.MainClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.PileInterfaces;
using BasicGameFrameworkLibrary.SolitaireClasses.DataClasses;

namespace PersianSolitaireCP.ViewModels
{
    [InstanceGame]
    public class PersianSolitaireMainViewModel : SolitaireMainViewModel<PersianSolitaireSaveInfo>
    {

        private int _dealNumber;

        public int DealNumber
        {
            get { return _dealNumber; }
            set
            {
                if (SetProperty(ref _dealNumber, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private readonly WastePiles _tempWaste;
        private readonly ISolitaireData _thisData;
        public PersianSolitaireMainViewModel(IEventAggregator aggregator,
            CommandContainer command, 
            IGamePackageResolver resolver,
            ISolitaireData thisData
            )
            : base(aggregator, command, resolver)
        {
            _tempWaste = (WastePiles)WastePiles1;
            _thisData = thisData;
        }
        public bool CanNewDeal => _thisData.Deals != DealNumber;
        [Command(EnumCommandCategory.Plain)]
        public void NewDeal()
        {
            _tempWaste.Redeal();
        }
        protected override SolitaireGameClass<PersianSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
        {
            return resolver.ReplaceObject<PersianSolitaireMainGameClass>();
        }
        //anything else needed is here.
    }
}
