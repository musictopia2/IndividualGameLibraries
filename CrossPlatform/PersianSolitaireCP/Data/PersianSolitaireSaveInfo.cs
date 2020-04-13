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
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.MainClasses;
//i think this is the most common things i like to do
namespace PersianSolitaireCP.Data
{
    [SingletonGame]
    public class PersianSolitaireSaveInfo : SolitaireSavedClass, IMappable
    {
		//anything else needed to save a game will be here.
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
		//think about dealnumber for view model (?)

	}
}