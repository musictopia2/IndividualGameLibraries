using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Dominos;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceModels;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using DominosMexicanTrainCP.Logic;
using System;
using System.Threading.Tasks; 
namespace DominosMexicanTrainCP.Data
{
	[SingletonGame]
	[AutoReset]
	public class DominosMexicanTrainVMData : ObservableObject, IDominoGamesData<MexicanDomino>
	{
		//for mexican train dominos, will go ahead and change the type of dominos used.

		public DominosMexicanTrainVMData(CommandContainer command,
			IGamePackageResolver resolver,
			DominosBasicShuffler<MexicanDomino> shuffle,
			GlobalClass global,
			TrainStationBoardProcesses trainStation,
			IEventAggregator aggregator
			)
		{
			PlayerHand1 = new HandObservable<MexicanDomino>(command);
			BoneYard = new DominosBoneYardClass<MexicanDomino>(this, command, resolver, shuffle);

			PlayerHand1.ObjectClickedAsync += PlayerHand1_ObjectClickedAsync;
			PlayerHand1.BoardClickedAsync += PlayerHand1_BoardClickedAsync;
			_global = global;
			_global.BoneYard = BoneYard;
			TrainStation1 = trainStation;
			_aggregator = aggregator;
			PrivateTrain1 = new HandObservable<MexicanDomino>(command);
			PlayerHand1.AutoSelect = HandObservable<MexicanDomino>.EnumAutoType.None;
			PrivateTrain1.AutoSelect = HandObservable<MexicanDomino>.EnumAutoType.None;
			PrivateTrain1.BoardClickedAsync += PrivateTrain1_BoardClickedAsync;
			PrivateTrain1.ObjectClickedAsync += PrivateTrain1_ObjectClickedAsync;
		}

		public Func<MexicanDomino, int, Task>? PrivateTrainObjectClickedAsync { get; set; }
		public Func<Task>? PrivateTrainBoardClickedAsync { get; set; }
		private Task PrivateTrain1_ObjectClickedAsync(MexicanDomino payLoad, int index)
		{
			if (PrivateTrainObjectClickedAsync == null)
			{
				throw new BasicBlankException("Private train click not set.  Rethink");
			}
			return PrivateTrainObjectClickedAsync.Invoke(payLoad, index);
		}

		private Task PrivateTrain1_BoardClickedAsync()
		{
			if (PrivateTrainBoardClickedAsync == null)
			{
				throw new BasicBlankException("Private board click not set.  Rethink");
			}
			return PrivateTrainBoardClickedAsync.Invoke();
		}

		internal void UpdateCount(DominosMexicanTrainPlayerItem player)
		{
			UpdateCountEventModel thisC = new UpdateCountEventModel();
			thisC.ObjectCount = player.LongestTrainList.Count;
			_aggregator.Publish(thisC);
		}
		private Task PlayerHand1_BoardClickedAsync()
		{
			if (PlayerBoardClickedAsync == null)
			{
				throw new BasicBlankException("Board clicked was never created.  Rethink");
			}
			return PlayerBoardClickedAsync.Invoke();
		}

		private Task PlayerHand1_ObjectClickedAsync(MexicanDomino domino, int index)
		{
			if (HandClickedAsync == null)
			{
				throw new BasicBlankException("The hand clicked was never done.  Rethink");
			}
			return HandClickedAsync.Invoke(domino, index);
		}

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

		private int _upTo;
		[VM]
		public int UpTo
		{
			get { return _upTo; }
			set
			{
				if (SetProperty(ref _upTo, value))
				{
					//can decide what to do when property changes
				}
			}
		}

		public Func<MexicanDomino, Task>? DrewDominoAsync { get; set; }
		public HandObservable<MexicanDomino> PlayerHand1 { get; set; }
		public DominosBoneYardClass<MexicanDomino> BoneYard { get; set; }
		public Func<Task>? PlayerBoardClickedAsync { get; set; }
		public Func<MexicanDomino, int, Task>? HandClickedAsync { get; set; }

		public bool CanEnableBasics()
		{
			return true;
		}

		public bool CanEnableAlways()
		{
			return true;
		}

		private readonly GlobalClass _global;

		//any other ui related properties will be here.
		//can copy/paste for the actual view model.

		public TrainStationBoardProcesses TrainStation1;
		private readonly IEventAggregator _aggregator;
		public HandObservable<MexicanDomino> PrivateTrain1;
	}
}
