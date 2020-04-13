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
using RaglanSolitaireCP.Logic;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using RaglanSolitaireCP.Data;
using BasicGameFrameworkLibrary.SolitaireClasses.MainClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.PileInterfaces;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;

namespace RaglanSolitaireCP.ViewModels
{
    [InstanceGame]
    public class RaglanSolitaireMainViewModel : SolitaireMainViewModel<RaglanSolitaireSaveInfo>
    {
        public RaglanSolitaireMainViewModel(IEventAggregator aggregator,
            CommandContainer command, 
            IGamePackageResolver resolver
            )
            : base(aggregator, command, resolver)
        {
            Stock1 = new HandObservable<SolitaireCard>(command);
            Stock1.Maximum = 6;
            Stock1.Text = "Stock";
            GlobalClass.Stock = Stock1;
        }
        public HandObservable<SolitaireCard>? Stock1;
        protected override SolitaireGameClass<RaglanSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
        {
            return resolver.ReplaceObject<RaglanSolitaireMainGameClass>();
        }
        //anything else needed is here.
    }
}
