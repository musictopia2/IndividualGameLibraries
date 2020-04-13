using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.ViewModels;
using CommonBasicStandardLibraries.Exceptions;
using LifeBoardGameCP.Cards;
using LifeBoardGameCP.Data;
using LifeBoardGameCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LifeBoardGameCP.ViewModels
{
    public class ChooseCareerViewModel : BasicSubmitViewModel
    {
        private readonly LifeBoardGameGameContainer _gameContainer;
        private readonly LifeBoardGameVMData _model;
        private readonly ICareerProcesses _processes;
        public ChooseCareerViewModel(CommandContainer commandContainer,
            LifeBoardGameGameContainer gameContainer,
            LifeBoardGameVMData model,
            ICareerProcesses processes
            ) : base(commandContainer)
        {
            _gameContainer = gameContainer;
            _model = model;
            _processes = processes;
            //this time, can't double check unfortunately.
        }

        public override bool CanSubmit
        {
            get
            {
                if (_model.HandList.AutoSelect == HandObservable<LifeBaseCard>.EnumAutoType.SelectOneOnly)
                {
                    return _model.HandList.ObjectSelected() > 0;
                }
                if (_gameContainer.SaveRoot.GameStatus == EnumWhatStatus.NeedChooseFirstCareer)
                {
                    if (_gameContainer.SingleInfo!.OptionChosen != EnumStart.College)
                    {
                        throw new BasicBlankException("Should have been one option alone.");
                    }
                    return _model.HandList.HowManySelectedObjects == 3;
                }
                if (_gameContainer.SaveRoot.GameStatus == EnumWhatStatus.NeedNewCareer)
                {
                    return _model.HandList.HowManySelectedObjects == 2;
                }
                throw new BasicBlankException("Cannot figure out whether it can submit or not.  Rethink");
            }
        }
        private void AdditionalCareerInstructions()
        {
            var tempList = _model.HandList!.ListSelectedObjects();
            tempList.MakeAllObjectsKnown();
            _model.HandList.HandList.ReplaceRange(tempList);
            _model.HandList.Text = "Choose One Career";
            if (_gameContainer.WasNight)
            {
                _model.Instructions = "Choose one career or end turn and keep your current career";
            }
            _model.HandList.AutoSelect = HandObservable<LifeBaseCard>.EnumAutoType.SelectOneOnly;
            _model.HandList.UnselectAllObjects();
        }
        public override Task SubmitAsync()
        {
            if (_model.HandList!.AutoSelect == HandObservable<LifeBaseCard>.EnumAutoType.SelectAsMany)
            {
                AdditionalCareerInstructions();
                return Task.CompletedTask;
            }
            if (_gameContainer.GameStatus == EnumWhatStatus.NeedNewCareer || _gameContainer.GameStatus == EnumWhatStatus.NeedChooseFirstCareer)
            {
                return _processes.ChoseCareerAsync(_model.HandList.ObjectSelected());
            }
            throw new BasicBlankException("Unable to submit based on the status of the game.  Rethink");
        }
    }
}