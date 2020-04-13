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
using System.Collections.Generic;
namespace MancalaCP.Data
{
    [SingletonGame]
    public class MancalaVMData : ObservableObject, IViewModelData
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
		public int SpaceSelected { get; set; }
		public int SpaceStarted { get; set; }

		internal Dictionary<int, SpaceInfo> SpaceList { get; set; } = new Dictionary<int, SpaceInfo>(); //this should be the main list.

        //any other ui related properties will be here.
        //can copy/paste for the actual view model.



        private string _instructions = "";
        [VM]
        public string Instructions
        {
            get { return _instructions; }
            set
            {
                if (SetProperty(ref _instructions, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _piecesAtStart;
        [VM]
        public int PiecesAtStart
        {
            get { return _piecesAtStart; }
            set
            {
                if (SetProperty(ref _piecesAtStart, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _piecesLeft;
        [VM]
        public int PiecesLeft
        {
            get { return _piecesLeft; }
            set
            {
                if (SetProperty(ref _piecesLeft, value))
                {
                    //can decide what to do when property changes
                }
            }
        }

    }
}
