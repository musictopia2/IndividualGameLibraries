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
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
//i think this is the most common things i like to do
namespace SnakesAndLaddersCP.Data
{
    [SingletonGame]
    public class SnakesAndLaddersVMData : ObservableObject, IViewModelData, ICup<SimpleDice>, IBasicEnableProcess
    {
		private string _normalTurn = "";
		[VM]
		public string NormalTurn
		{
			get { return _normalTurn; }
			set
			{
				if (SetProperty(ref _normalTurn, value))
				{
					//can decide what to do when property changes
				}

			}
		}

		private string _status = "";
		private readonly CommandContainer _command;
		private readonly IGamePackageResolver _resolver;

		[VM] //use this tag to transfer to the actual view model.  this is being done to avoid overflow errors.
		public string Status
		{
			get { return _status; }
			set
			{
				if (SetProperty(ref _status, value))
				{
					//can decide what to do when property changes
				}

			}
		}

		//do cup here.  hopefully that works.
		public DiceCup<SimpleDice>? Cup { get; set; }

		public SnakesAndLaddersVMData(CommandContainer command, IGamePackageResolver resolver)
		{
			_command = command;
			_resolver = resolver;
		}

		public void LoadCup(SnakesAndLaddersSaveInfo saveRoot, bool autoResume)
		{
			//decided to let this new one handle the loading of the cup.
			Cup = new DiceCup<SimpleDice>(saveRoot.DiceList, _resolver, _command);
			Cup.SendEnableProcesses(this, () =>
			{
				return false; //because you can't click the dice.
			});
			Cup.HowManyDice = 1;
			if (autoResume == true && saveRoot.HasRolled == true)
			{
				Cup.CanShowDice = true;
				Cup.Visible = true;
			} //hopefully no need to raise event for clicking dice.
		}

		bool IBasicEnableProcess.CanEnableBasics()
		{
			return false; //hopefully i don't regret this.
		}
		//any other ui related properties will be here.
		//can copy/paste for the actual view model.

	}
}
