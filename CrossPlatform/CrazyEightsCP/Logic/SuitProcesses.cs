using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CrazyEightsCP.Data;
using System.Threading.Tasks;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using mm = BasicGameFrameworkLibrary.Extensions.CommonMessageStrings;

namespace CrazyEightsCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class SuitProcesses : ISuitProcesses, IChoosePieceNM
    {
        private readonly CrazyEightsGameContainer _gameContainer;
        private readonly CrazyEightsVMData _model;

        public SuitProcesses(CrazyEightsGameContainer gameContainer, CrazyEightsVMData model)
        {
            _gameContainer = gameContainer;
            _model = model;
        }

        public async Task ChoosePieceReceivedAsync(string data)
        {
            EnumSuitList Suit = await js.DeserializeObjectAsync<EnumSuitList>(data);
            await SuitChosenAsync(Suit);
        }
        public async Task SuitChosenAsync(EnumSuitList chosen)
        {
            _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
            if (_gameContainer.SingleInfo.CanSendMessage(_gameContainer.BasicData!))
                await _gameContainer.Network!.SendAllAsync(mm.ChosenPiece, chosen);
            _gameContainer.SaveRoot!.CurrentSuit = chosen;
            _gameContainer.SaveRoot.ChooseSuit = false;
            var thisCard = _model!.Pile1!.CurrentDisplayCard;
            thisCard.DisplaySuit = chosen;
            await _gameContainer.EndTurnAsync!.Invoke();
        }

    }
}