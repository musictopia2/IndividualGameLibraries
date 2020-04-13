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
using VegasSolitaireCP.Logic;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using VegasSolitaireCP.Data;
using BasicGameFrameworkLibrary.SolitaireClasses.MainClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.PileInterfaces;

namespace VegasSolitaireCP.ViewModels
{
    [InstanceGame]
    public class VegasSolitaireMainViewModel : SolitaireMainViewModel<VegasSolitaireSaveInfo>
    {
        private decimal _money = 500; //start out with 500.
        private readonly MoneyModel _money1;

        public decimal Money
        {
            get { return _money; }
            set
            {
                if (SetProperty(ref _money, value))
                {
                    //can decide what to do when property changes
                    _money1.Money = value;
                }

            }
        }
        public VegasSolitaireMainViewModel(IEventAggregator aggregator,
            CommandContainer command, 
            IGamePackageResolver resolver,
            MoneyModel money
            )
            : base(aggregator, command, resolver)
        {
            _money1 = money;
            Money = money.Money;
        }

        protected override SolitaireGameClass<VegasSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
        {
            VegasSolitaireMainGameClass game;
            game = resolver.ReplaceObject<VegasSolitaireMainGameClass>();
            game.AddMoney = (() => Money += 5);
            game.ResetMoney = (() => Money -=52 );
            return game;
        }
    }
}
