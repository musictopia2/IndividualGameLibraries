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
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
//i think this is the most common things i like to do
namespace TroubleCP.Data
{
    [SingletonGame]
    public class TroubleSaveInfo : BasicSavedBoardDiceGameClass<TroublePlayerItem>
    { //anything needed for autoresume is here.
		private string _instructions = "";

		public string Instructions
		{
			get { return _instructions; }
			set
			{
				if (SetProperty(ref _instructions, value))
				{
					//can decide what to do when property changes
					if(_model != null)
					{
						_model.Instructions = value;
					}
				}

			}
		}
		private TroubleVMData? _model;
		internal void LoadMod(TroubleVMData model)
		{
			_model = model;
			_model.Instructions = Instructions;
		}
		public CustomBasicList<MoveInfo> MoveList { get; set; } = new CustomBasicList<MoveInfo>();
		public EnumColorChoice OurColor { get; set; } //decided to use different enum.
		public int DiceNumber { get; set; } //hopefully this is fine too.
	}
}