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
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.Dominos;
//i think this is the most common things i like to do
namespace ItalianDominosCP.Data
{
    public class ItalianDominosPlayerItem : PlayerSingleHand<SimpleDominoInfo>
    { //anything needed is here
		//if not needed, take out.
		private int _totalScore;

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

		private bool _drewYet;

		public bool DrewYet
		{
			get { return _drewYet; }
			set
			{
				if (SetProperty(ref _drewYet, value))
				{
					//can decide what to do when property changes
				}

			}
		}

	}
}
