using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using TileRummyCP.Logic;
namespace TileRummyCP.Data
{
	[SingletonGame]
	public class TileRummyVMData : ObservableObject, IViewModelData, IEnableAlways
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

		//decided to risk having things like pool put here instead of the main view model.
		//same thing for hand, tempsets, etc.
		//to stop the overflows on the main game class.
		public PoolCP Pool1;
		public TempSetsObservable<EnumColorType, EnumColorType, TileInfo> TempSets;
		public MainSets MainSets1;
		public TileHand PlayerHand1;


		//the real view model will do the code behind for this.
		//means delegates here too.
		//the real view model knows about this but this can't reference view model or overflow errors.


		public Func<int, int, int, Task>? MainSetsClickedAsync { get; set; }
		public Func<int, Task>? TempSetsClickedAsync { get; set; }

		public TileRummyVMData(CommandContainer command, IGamePackageResolver resolver, TileShuffler shuffle)
		{
			TempSets = new TempSetsObservable<EnumColorType, EnumColorType, TileInfo>(command, resolver);
			TempSets.HowManySets = 4;
			
			//it also means that if something else is needed, then its done via delgates.
			TempSets.SetClickedAsync += TempSets_SetClickedAsync;
			MainSets1 = new MainSets(command);
			MainSets1.SetClickedAsync += MainSets1_SetClickedAsync;
			//the main view model has to set the enable processes.
			PlayerHand1 = new TileHand(command);
			Pool1 = new PoolCP(command, resolver, shuffle);
		}

		private Task MainSets1_SetClickedAsync(int setNumber, int section, int deck)
		{
			if (MainSetsClickedAsync == null)
			{
				throw new BasicBlankException("Main sets function not created.  Rethink");
			}
			return MainSetsClickedAsync.Invoke(setNumber, section, deck);
		}

		private Task TempSets_SetClickedAsync(int index)
		{
			if (TempSetsClickedAsync == null)
			{
				throw new BasicBlankException("Temp sets function not created.  Rethink");
			}
			return TempSetsClickedAsync.Invoke(index);
		}

		bool IEnableAlways.CanEnableAlways()
		{
			return true;
		}
	}
}