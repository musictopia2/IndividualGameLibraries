using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using LifeBoardGameCP.Cards;
using LifeBoardGameCP.Data;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LifeBoardGameCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class CareerProcesses : ICareerProcesses
    {
        private readonly LifeBoardGameVMData _model;
        private readonly LifeBoardGameGameContainer _gameContainer;
        public CareerProcesses(LifeBoardGameVMData model, LifeBoardGameGameContainer gameContainer)
        {
            _model = model;
            _gameContainer = gameContainer;
        }
        private void RemoveCareer(int career)
        {
            var careerList = _gameContainer.SingleInfo!.GetCareerList();
            careerList.ForEach(thisCard =>
            {
                if (thisCard.Deck != career)
                    _gameContainer.SingleInfo!.Hand.RemoveObjectByDeck(thisCard.Deck);
            });
        }
        private bool PrivateCanGetSalary(string career1, string career2)
        {
            if (career1 != "Teacher")
                return true;
            return career2 == "";
        }
        public async Task ChoseCareerAsync(int deck)
        {
            if (_gameContainer.CanSendMessage())
            {
                await _gameContainer.Network!.SendAllAsync("chosecareer", deck);
            }
            if (_gameContainer.WasNight)
            {
                RemoveCareer(deck);
            }
            var thisCareer = CardsModule.GetCareerCard(deck);
            _gameContainer.SingleInfo!.Hand.Add(thisCareer);
            await _gameContainer.ShowCardAsync(thisCareer);
            string career1 = PopulatePlayerProcesses.CareerChosen(_gameContainer.SingleInfo, out string career2);
            if (career1 != "Teacher" && _gameContainer.SaveRoot.WasNight == false)
            {
                RemoveCareer(deck);
            }
            PopulatePlayerProcesses.FillInfo(_gameContainer.SingleInfo);
            if (PrivateCanGetSalary(career1, career2))
            {
                _gameContainer.SaveRoot.EndAfterSalary = _gameContainer.GameStatus == EnumWhatStatus.NeedNewCareer || _gameContainer.SaveRoot.WasNight;
                var thisSalary = _gameContainer.SingleInfo.GetSalaryCard();
                _gameContainer.SingleInfo.Hand.RemoveSpecificItem(thisSalary);
                _gameContainer.GameStatus = EnumWhatStatus.NeedChooseSalary;
                await _gameContainer.ContinueTurnAsync!.Invoke();
                return;
            }
            if (_gameContainer.SaveRoot.WasNight)
            {
                _gameContainer.SaveRoot.EndAfterSalary = true;
            }
            else if (_gameContainer.TeacherChooseSecondCareer)
            {
                _gameContainer.GameStatus = EnumWhatStatus.NeedNewCareer;
                _gameContainer.SaveRoot.MaxChosen = 1;
            }
            else
            {
                _gameContainer.GameStatus = EnumWhatStatus.NeedToSpin;
            }
            _gameContainer.RepaintBoard();
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }
        private bool ShowAllCareers => _gameContainer.SingleInfo!.OptionChosen == EnumStart.College || _gameContainer.WasNight == true;
        public void LoadCareerList() //if it does not load the career list, then rethink
        {
            _model.HandList.Text = "Career List";
            var firstList = _gameContainer.Random.GenerateRandomList(9);
            bool canShowAll = ShowAllCareers;
            DeckRegularDict<CareerInfo> tempList = new DeckRegularDict<CareerInfo>();
            firstList.ForEach(thisItem =>
            {
                tempList.Add(CardsModule.GetCareerCard(thisItem));
            });
            if (canShowAll == false)
            {
                tempList.KeepConditionalItems(items => items.DegreeRequired == false);
            }
            var finList = tempList.GetLoadedCards(_gameContainer!.PlayerList!);
            _model.HandList.HandList.ReplaceRange(finList); //hopefully smart enough on whether its unknown or not (?)
            _gameContainer!.MaxChosen = 1;
            if (_gameContainer.WasNight)
            {
                _model.Instructions = "Pick 2 cards at random for your new career for night school.";
                _gameContainer.MaxChosen = 2;
            }
            else if (_gameContainer.SingleInfo!.OptionChosen == EnumStart.College && _gameContainer.GameStatus == EnumWhatStatus.NeedChooseFirstCareer)
            {
                _model.Instructions = "Pick 3 cards at random for your first career since you went to college.";
                _gameContainer.MaxChosen = 3;
            }
            else if (_gameContainer.GameStatus == EnumWhatStatus.NeedChooseFirstCareer)
            {
                _model.Instructions = "Pick one card to choose your career.";
            }
            else
            {
                _model.Instructions = "Pick one card for your new career.";
            }
            if (_gameContainer.MaxChosen == 1)
            {
                _model.HandList.AutoSelect = HandObservable<LifeBaseCard>.EnumAutoType.SelectOneOnly;
            }
            else
            {
                _model.HandList.AutoSelect = HandObservable<LifeBaseCard>.EnumAutoType.SelectAsMany;
            }
        }
    }
}