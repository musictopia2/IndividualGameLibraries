using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.SolitaireClasses.DataClasses;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
namespace PersianSolitaireCP.Data
{
	[SingletonGame]
	public class ScoreModel : ObservableObject, IScoreData
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
		private int _dealNumber;

		public int DealNumber
		{
			get { return _dealNumber; }
			set
			{
				if (SetProperty(ref _dealNumber, value))
				{
					_aggregator.Publish(this);
				}

			}
		}

		public ScoreModel(IEventAggregator aggregator)
		{
			_aggregator = aggregator;
		}

	}
}
