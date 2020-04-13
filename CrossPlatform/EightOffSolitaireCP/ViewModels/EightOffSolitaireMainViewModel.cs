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
using EightOffSolitaireCP.Logic;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using EightOffSolitaireCP.Data;
using BasicGameFrameworkLibrary.SolitaireClasses.MainClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.PileInterfaces;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;

namespace EightOffSolitaireCP.ViewModels
{
    [InstanceGame]
    public class EightOffSolitaireMainViewModel : SolitaireMainViewModel<EightOffSolitaireSaveInfo>
    {
        [Command(EnumCommandCategory.Plain)]
        public async Task AddToReserveAsync()
        {
            await _mainGame!.AddToReserveAsync();
        }

        public ReservePiles ReservePiles1;
        private EightOffSolitaireMainGameClass? _mainGame;
        public EightOffSolitaireMainViewModel(IEventAggregator aggregator,
            CommandContainer command, 
            IGamePackageResolver resolver
            )
            : base(aggregator, command, resolver)
        {
            GlobalClass.MainModel = this;
            ReservePiles1 = new ReservePiles(command);
            ReservePiles1.Maximum = 8;
            ReservePiles1.AutoSelect = HandObservable<SolitaireCard>.EnumAutoType.SelectOneOnly;
            ReservePiles1.Text = "Reserve Pile";
        }

        protected override SolitaireGameClass<EightOffSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
        {
            _mainGame = resolver.ReplaceObject<EightOffSolitaireMainGameClass>();
            return _mainGame;
        }
        //anything else needed is here.
    }
}
