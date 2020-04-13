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
//i think this is the most common things i like to do
namespace BowlingDiceGameCP.Data
{
    [SingletonGame]
    public class BowlingDiceGameVMData : ObservableObject, IViewModelData
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

        //any other ui related properties will be here.
        //can copy/paste for the actual view model.

        private bool _isExtended;
        [VM]
        public bool IsExtended
        {
            get { return _isExtended; }
            set
            {
                if (SetProperty(ref _isExtended, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _whichPart;
        [VM]
        public int WhichPart
        {
            get { return _whichPart; }
            set
            {
                if (SetProperty(ref _whichPart, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _whatFrame;
        [VM]
        public int WhatFrame
        {
            get { return _whatFrame; }
            set
            {
                if (SetProperty(ref _whatFrame, value))
                {
                    //can decide what to do when property changes
                }
            }
        }


    }
}
