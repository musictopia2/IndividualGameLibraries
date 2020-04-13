using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using GolfCardGameCP.Data;
using GolfCardGameCP.Logic;
using System.Linq;
using System.Threading.Tasks;

namespace GolfCardGameCP.ViewModels
{
    [InstanceGame]
    public class FirstViewModel : Screen, IBlankGameVM
    {
        private readonly GolfCardGameVMData _model;
        private readonly GolfCardGameGameContainer _gameContainer;
        private readonly IBeginningProcesses _processes;

        public string Instructions { get; set; } = "";
        public FirstViewModel(
            CommandContainer commandContainer,
            GolfCardGameVMData model,
            GolfCardGameGameContainer gameContainer,
            IBeginningProcesses processes
            )
        {
            CommandContainer = commandContainer;
            _model = model;
            _gameContainer = gameContainer;
            _processes = processes;
            Instructions = "Choose the 2 cards to put into your hand";
            //well see what this will do for the clicking.
        }

        public CommandContainer CommandContainer { get; set; }

        [Command(EnumCommandCategory.Plain)]
        public async Task ChooseFirstCardsAsync()
        {
            if (_model.Beginnings1.CanContinue == false)
            {
                await UIPlatform.ShowMessageAsync("Sorry, must select 2 and only 2 cards to put into your hand");
                return;
            }
            _model.Beginnings1.GetSelectInfo(out DeckRegularDict<RegularSimpleCard> selectList, out DeckRegularDict<RegularSimpleCard> unselectList);
            if (_gameContainer.BasicData!.MultiPlayer == true)
            {
                SendBeginning thisB = new SendBeginning();
                thisB.SelectList = selectList;
                thisB.UnsSelectList = unselectList;
                thisB.Player = _gameContainer.PlayerList!.Where(items => items.PlayerCategory == EnumPlayerCategory.Self).Single().Id;
                await _gameContainer.Network!.SendAllAsync("selectbeginning", thisB);
            }

            int player = _gameContainer.PlayerList.Single(items => items.PlayerCategory == EnumPlayerCategory.Self).Id;
            await _processes.SelectBeginningAsync(player, selectList, unselectList);
        }
    }
}
