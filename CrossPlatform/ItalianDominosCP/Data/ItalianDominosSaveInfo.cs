using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.Dominos;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace ItalianDominosCP.Data
{
	[SingletonGame]
	public class ItalianDominosSaveInfo : BasicSavedDominosClass<SimpleDominoInfo, ItalianDominosPlayerItem>
	{ //anything needed for autoresume is here.
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
					ItalianDominosVMData model = cons!.Resolve<ItalianDominosVMData>(); //other choice is to have the extra function there.
					model.UpTo = value;
				}

			}
		}
		private int _nextNumber;

		public int NextNumber
		{
			get { return _nextNumber; }
			set
			{
				if (SetProperty(ref _nextNumber, value))
				{
					//can decide what to do when property changes
					ItalianDominosVMData model = cons!.Resolve<ItalianDominosVMData>();
					model.NextNumber = value;
				}

			}
		}
		public ItalianDominosSaveInfo()
		{
			
		}
	}
}