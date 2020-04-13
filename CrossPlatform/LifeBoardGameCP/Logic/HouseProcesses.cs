using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using CommonBasicStandardLibraries.Exceptions;
using LifeBoardGameCP.Cards;
using LifeBoardGameCP.Data;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LifeBoardGameCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class HouseProcesses : IHouseProcesses
    {
        private readonly LifeBoardGameVMData _model;
        private readonly LifeBoardGameGameContainer _gameContainer;

        public HouseProcesses(LifeBoardGameVMData model, LifeBoardGameGameContainer gameContainer)
        {
            _model = model;
            _gameContainer = gameContainer;
        }
        public async Task ChoseHouseAsync(int deck)
        {
            if (_gameContainer.CanSendMessage())
            {
                await _gameContainer.Network!.SendAllAsync("chosehouse", deck);
            }
            HouseInfo thisHouse = CardsModule.GetHouseCard(deck);
            await _gameContainer.ShowCardAsync(thisHouse);
            _gameContainer.SingleInfo!.Hand.Add(thisHouse);
            _gameContainer.SaveRoot!.HouseList.Clear();
            PopulatePlayerProcesses.FillInfo(_gameContainer.SingleInfo); //i think here too.
            _gameContainer.TakeOutExpense(thisHouse.HousePrice);
            _gameContainer.GameStatus = EnumWhatStatus.NeedToSpin;
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }

        public void LoadHouseList()
        {
            _model.HandList!.Text = "House List";
            if (_gameContainer!.SaveRoot!.HouseList.Count != 2)
                throw new BasicBlankException("The house list must have 2 items");
            _model.HandList.HandList.ReplaceRange(_gameContainer.SaveRoot.HouseList);
            _model.Instructions = "Choose a house to buy or spin to not purchase either house";
            _model.HandList.AutoSelect = HandObservable<LifeBaseCard>.EnumAutoType.SelectOneOnly;
        }

        public async Task ShowYourHouseAsync()
        {
            if (_gameContainer.CardVisible == null)
            {
                throw new BasicBlankException("Nobody is handling card visible.  Rethink");
            }
            var card = _gameContainer.SingleInfo!.Hand.Single(x => x.CardCategory == EnumCardCategory.House);
            card.IsUnknown = false; //just in case.
            _model.SinglePile.AddCard(card);
            if (_gameContainer.CardVisible.Invoke() == true)
            {
                return;
            }
            
            await _gameContainer.ShowHouseAsync();
        }
    }
}