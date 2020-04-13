using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.ViewModels;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using HuseHeartsCP.Data;
using HuseHeartsCP.Logic;
using System.Threading.Tasks;

namespace HuseHeartsCP.ViewModels
{
    public class PassingViewModel : BasicSubmitViewModel
    {
        private readonly HuseHeartsMainGameClass _mainGame;
        private readonly HuseHeartsVMData _model;
        public override string Text => "Pass Cards";
        public PassingViewModel(CommandContainer commandContainer, HuseHeartsMainGameClass mainGame, HuseHeartsVMData model) : base(commandContainer)
        {
            _mainGame = mainGame;
            _model = model;
        }

        public override bool CanSubmit => true;

        public override async Task SubmitAsync()
        {
            //this is passing.
            int cardsSelected = _model.PlayerHand1!.HowManySelectedObjects;
            if (cardsSelected != 3)
            {
                await UIPlatform.ShowMessageAsync("Must pass 3 cards");
                return;
            }
            var tempList = _model.PlayerHand1.ListSelectedObjects().GetDeckListFromObjectList();
            if (_mainGame.BasicData!.MultiPlayer == true)
                await _mainGame.Network!.SendAllAsync("passcards", tempList);
            await _mainGame!.CardsPassedAsync(tempList);

        }
    }
}