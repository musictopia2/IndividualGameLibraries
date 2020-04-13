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
using ThinkTwiceCP.Data;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using BasicGameFrameworkLibrary.CommandClasses;
//i think this is the most common things i like to do
namespace ThinkTwiceCP.ViewModels
{
    //attempt to make this a screen.
	[InstanceGame]
    public class ScoreViewModel : Screen, IBlankGameVM
    {
		private int _itemSelected;
		private readonly ThinkTwiceVMData _model;
		private readonly ThinkTwiceGameContainer _gameContainer;

		public int ItemSelected
		{
			get { return _itemSelected; }
			set
			{
				if (SetProperty(ref _itemSelected, value))
				{
					//can decide what to do when property changes
				}

			}
		}

		private bool _isEnabled;

		public bool IsEnabled //needed for the frame.
		{
			get { return _isEnabled; }
			set
			{
				if (SetProperty(ref _isEnabled, value))
				{
					//can decide what to do when property changes
				}

			}
		}

		private string GetDescriptionText()
		{
			if (ItemSelected == 5)
				return "2 of a kind:  10 points" + Constants.vbCrLf + "3 of a kind:  20 points" + Constants.vbCrLf + "4 of a kind:  30 points" + Constants.vbCrLf + "5 of a kind:  50 points" + Constants.vbCrLf + "6 of a kind:  100 points";
			return "Sum of all the dice";
		} // hopefully its this simple (?)

		public ScoreViewModel(ThinkTwiceVMData model, ThinkTwiceGameContainer gameContainer)
		{
			_model = model;
			_gameContainer = gameContainer;
			_gameContainer.Command.ExecutingChanged += Command_ExecutingChanged;
			_model.PropertyChanged += PropertyChange;
			CommandContainer = _gameContainer.Command;
			ItemSelected = _model.ItemSelected;
		}

		private bool CanClickCommands(bool isMain)
		{
			if (_gameContainer.SaveRoot.CategoryRolled == -1)
			{
				return false;
			}
			if (isMain == false)
			{
				return true;
			}
			return ItemSelected > -1;
		}

		private void PropertyChange(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(ItemSelected))
			{
				ItemSelected = _model.ItemSelected;
			}
		}

		protected override Task TryCloseAsync()
		{
			_gameContainer.Command.ExecutingChanged -= Command_ExecutingChanged;
			_model.PropertyChanged -= PropertyChange;
			return base.TryCloseAsync();
		}
		private void Command_ExecutingChanged()
		{
			IsEnabled = !_gameContainer.Command.IsExecuting;
		}
		#region Command Functions
		public bool CanScoreDescription => CanClickCommands(true);
		[Command(EnumCommandCategory.Plain)]
		public async Task ScoreDescriptionAsync()
		{
			if (ItemSelected == -1)
			{
				throw new BasicBlankException("Nothing Selected");
			}
			var text = GetDescriptionText();
			await UIPlatform.ShowMessageAsync(text);
		}
		public bool CanChangeSelection => CanClickCommands(false);
		[Command(EnumCommandCategory.Plain)]
		public void ChangeSelection(string text)
		{
			_model.ItemSelected = _model.TextList.IndexOf(text);
		}
		public bool CanCalculateScore => CanClickCommands(true);

		public CommandContainer CommandContainer { get; set; }
		[Command(EnumCommandCategory.Plain)]
		public async Task CalculateScoreAsync()
		{
			if (_gameContainer.ScoreClickAsync == null)
			{
				throw new BasicBlankException("Nobody is handling the score click.  Rethink");
			}
			await _gameContainer.ScoreClickAsync.Invoke();
		}

        #endregion



    }
}
