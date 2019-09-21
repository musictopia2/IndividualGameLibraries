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
using BasicGameFramework.CommandClasses;
namespace EightOffSolitaireCP
{
    public class EightOffSolitaireViewModel : SolitaireMainViewModel<EightOffSolitaireSaveInfo>
    {
        public EightOffSolitaireViewModel(ISimpleUI tempUI, IGamePackageResolver tempC) : base(tempUI, tempC) { }
        public ReservePiles? ReservePiles1;

        public PlainCommand? ReserveCommand { get; private set; }

        public override void Init()
        {
            base.Init();
            ReservePiles1 = new ReservePiles(this);
            ReservePiles1.Maximum = 8;
            EightOffSolitaireGameClass mainGame = MainContainer!.Resolve<EightOffSolitaireGameClass>();
            ReservePiles1.AutoSelect = HandViewModel<BaseSolitaireClassesCP.Cards.SolitaireCard>.EnumAutoType.SelectOneOnly;
            ReservePiles1.Text = "Reserve Pile";
            ReservePiles1.Visible = true; //double click is not supported for not.
            ReserveCommand = new PlainCommand(async items =>
            {
                await mainGame.AddToReserveAsync();
            }, items => true, this, CommandContainer!);
        }
    }
}