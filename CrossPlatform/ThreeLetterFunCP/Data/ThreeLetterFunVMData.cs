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
using ThreeLetterFunCP.Logic;
//i think this is the most common things i like to do
namespace ThreeLetterFunCP.Data
{
    [SingletonGame]
    public class ThreeLetterFunVMData : ObservableObject, IViewModelData
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

		public INewCard? NewUI;

		public TileBoardObservable? TileBoard1 { get; set; }



		private string _playerWon = "";
		[VM]
		public string PlayerWon
		{
			get { return _playerWon; }
			set
			{
				if (SetProperty(ref _playerWon, value))
				{
					//can decide what to do when property changes
				}

			}
		}

		private ThreeLetterFunCardData? _currentCard;
		[VM]
		public ThreeLetterFunCardData? CurrentCard
		{
			get { return _currentCard; }
			set
			{
				if (SetProperty(ref _currentCard, value))
				{
					if (NewUI != null)
						NewUI.ShowNewCard();
				}

			}
		}

	}
}
