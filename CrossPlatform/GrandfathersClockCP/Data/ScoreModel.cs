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
using BasicGameFrameworkLibrary.SolitaireClasses.DataClasses;
using BasicGameFrameworkLibrary.Attributes;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.SolitaireClasses.ClockClasses;
//i think this is the most common things i like to do
namespace GrandfathersClockCP.Data
{
    [SingletonGame]
    public class ScoreModel : ObservableObject, IScoreData, IClockVM
    {
		private int _score;
		private readonly IEventAggregator _aggregator;

		public int Score
		{
			get { return _score; }
			set
			{
				if (SetProperty(ref _score, value))
				{
					//can decide what to do when property changes
					_aggregator.Publish(this);
				}

			}
		}
		public ScoreModel(IEventAggregator aggregator)
		{
			_aggregator = aggregator;
		}

		Task IClockVM.ClockClickedAsync(int index)
		{
			throw new BasicBlankException("Rethinking may be necessary");
		}
	}
}
