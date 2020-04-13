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
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.CommandClasses;
using CribbageCP.Logic;
//i think this is the most common things i like to do
namespace CribbageCP.Data
{
    [SingletonGame]
	[AutoReset] //usually needs autoreset
    public class CribbageVMData : ObservableObject, IBasicCardGamesData<CribbageCard>, IBasicEnableProcess
    {

		public CribbageVMData(IEventAggregator aggregator, CommandContainer command, HiddenBoard board)
		{
			Deck1 = new DeckObservablePile<CribbageCard>(aggregator, command);
			Pile1 = new PileObservable<CribbageCard>(aggregator, command);
			PlayerHand1 = new HandObservable<CribbageCard>(command);
			MainFrame = new HandObservable<CribbageCard>(command);
			CribFrame = new HandObservable<CribbageCard>(command);
			CribFrame.Visible = false;
			MainFrame.Text = "Card List";
			CribFrame.Text = "Crib";
			MainFrame.SendEnableProcesses(this, () => false);
			CribFrame.SendEnableProcesses(this, () => false);
			//something else has to set the maxs.
			GameBoard1 = board;
			ScoreBoard1 = new ScoreBoardCP();

		}
		public ScoreBoardCP ScoreBoard1;
		public HandObservable<CribbageCard> CribFrame;
		public HandObservable<CribbageCard> MainFrame;
		public HiddenBoard GameBoard1;
		public DeckObservablePile<CribbageCard> Deck1 { get; set; }
		public PileObservable<CribbageCard> Pile1 { get; set; }
		public HandObservable<CribbageCard> PlayerHand1 { get; set; }
		public PileObservable<CribbageCard>? OtherPile { get; set; }

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
		private int _totalScore;
		[VM]
		public int TotalScore
		{
			get { return _totalScore; }
			set
			{
				if (SetProperty(ref _totalScore, value))
				{
					//can decide what to do when property changes
				}

			}
		}
		private int _totalCount;
		[VM]
		public int TotalCount
		{
			get { return _totalCount; }
			set
			{
				if (SetProperty(ref _totalCount, value))
				{
					//can decide what to do when property changes
				}

			}
		}
		private string _dealer = "";
		[VM]
		public string Dealer
		{
			get { return _dealer; }
			set
			{
				if (SetProperty(ref _dealer, value))
				{
					//can decide what to do when property changes
				}

			}
		}


		bool IBasicEnableProcess.CanEnableBasics()
		{
			return true;
		}
	}
}
