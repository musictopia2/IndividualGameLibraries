using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModels;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FluxxCP.Containers;
using FluxxCP.Logic;
using System.Threading.Tasks;

namespace FluxxCP.ViewModels
{
    public class GiveViewModel : BasicSubmitViewModel
    {
        private readonly FluxxVMData _model;
        private readonly FluxxGameContainer _gameContainer;
        private readonly IGiveTaxationProcesses _processes;
        public override string Text => "Give";
        public GiveViewModel(
            CommandContainer commandContainer,
            FluxxVMData model,
            FluxxGameContainer gameContainer,
            IGiveTaxationProcesses processes
            ) : base(commandContainer)
        {
            _model = model;
            _gameContainer = gameContainer;
            _processes = processes;
        }

        public override bool CanSubmit => true;

        public override async Task SubmitAsync()
        {
            if (_model.Keeper1!.HowManySelectedObjects > 0)
            {
                await UIPlatform.ShowMessageAsync("Cannot select any keeper cards because you have to give the cards from your hand");
                _model.UnselectAllCards();
                return;
            }
            if (_model.Goal1!.ObjectSelected() > 0)
            {
                await UIPlatform.ShowMessageAsync("Cannot select any goal cards because you have to give the cards from your hand");
                _model.UnselectAllCards();
                return;
            }
            int howMany = _gameContainer.IncreaseAmount() + 1;
            if (_model.PlayerHand1!.HowManySelectedObjects == howMany || _model.PlayerHand1.HowManySelectedObjects == _model.PlayerHand1.HandList.Count)
            {
                var thisList = _model.PlayerHand1.ListSelectedObjects(true);
                await _processes.GiveCardsForTaxationAsync(thisList);
                return;
            }
            if (howMany > _model.PlayerHand1.HandList.Count)
                howMany = _model.PlayerHand1.HandList.Count;
            await UIPlatform.ShowMessageAsync($"Must give {howMany} not {_model.PlayerHand1.HowManySelectedObjects} cards");
            _model.UnselectAllCards();
        }
    }
}
