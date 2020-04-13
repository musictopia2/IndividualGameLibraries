using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using LifeBoardGameCP.Data;
using LifeBoardGameCP.Logic;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LifeBoardGameCP.ViewModels
{
    [InstanceGame]
    public class ChooseGenderViewModel : Screen, IBlankGameVM, IDisposable
    {
        private readonly LifeBoardGameVMData _model;
        private readonly IGenderProcesses _processes;
        private readonly LifeBoardGameGameContainer _gameContainer;
        private string _turn = "";

        public string Turn
        {
            get { return _turn; }
            set
            {
                if (SetProperty(ref _turn, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        private string _instructions = "";

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

        public ChooseGenderViewModel(CommandContainer commandContainer, LifeBoardGameVMData model, IGenderProcesses processes, LifeBoardGameGameContainer gameContainer)
        {
            CommandContainer = commandContainer;
            _model = model;
            _processes = processes;
            _gameContainer = gameContainer;
            _gameContainer.SelectGenderAsync = _processes.ChoseGenderAsync;
            _processes.SetInstructions = (x => Instructions = x);
            _processes.SetTurn = (x => Turn = x); //has to set delegates before init obviously.
            _model.GenderChooser.ItemClickedAsync += GenderChooser_ItemClickedAsync;
            //_processes.Init();
        }

        private Task GenderChooser_ItemClickedAsync(EnumGender piece)
        {
            //this is what you chose.
            return _processes.ChoseGenderAsync(piece);
        }

        public CommandContainer CommandContainer { get; set; }

        public void Dispose()
        {
            _model.GenderChooser.ItemClickedAsync -= GenderChooser_ItemClickedAsync;
        }
    }
}
