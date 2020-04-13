using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using BladesOfSteelCP.Data;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BladesOfSteelCP.ViewModels
{
    [InstanceGame]
    public class FaceoffViewModel : Screen, IBlankGameVM
    {
        private readonly BladesOfSteelVMData _model;
        private readonly BladesOfSteelGameContainer _gameContainer;

        public string Instructions { get; set; } = "";

        public FaceoffViewModel(CommandContainer commandContainer, BladesOfSteelVMData model, BladesOfSteelGameContainer gameContainer)
        {
            CommandContainer = commandContainer;
            _model = model;
            _gameContainer = gameContainer;
            _model.Deck1.DeckClickedAsync += Deck1_DeckClickedAsync;
            Instructions = "Face-Off.  Click the deck to draw a card at random.  Whoever draws a higher number goes first for the game.  If there is a tie; then repeat.";
        }

        private async Task Deck1_DeckClickedAsync()
        {
            await _gameContainer.DrawAsync!.Invoke();
            
        }

        protected override Task TryCloseAsync()
        {
            _model.Deck1.DeckClickedAsync -= Deck1_DeckClickedAsync;
            return base.TryCloseAsync();
        }
        public CommandContainer CommandContainer { get; set; }
    }
}
