using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using LifeCardGameCP.Cards;
using LifeCardGameCP.Data;
using LifeCardGameCP.Logic;
using System.Threading.Tasks;

namespace LifeCardGameCP.ViewModels
{
    [InstanceGame]
    public class OtherViewModel : Screen, IBlankGameVM
    {
        private readonly LifeCardGameVMData _model;
        private readonly LifeCardGameMainGameClass _mainGame;
        private readonly LifeCardGameGameContainer _gameContainer;

        public string OtherText { get; set; }

        public OtherViewModel(CommandContainer commandContainer, LifeCardGameVMData model, LifeCardGameMainGameClass mainGame, LifeCardGameGameContainer gameContainer)
        {
            CommandContainer = commandContainer;
            _model = model;
            _mainGame = mainGame;
            _gameContainer = gameContainer;
            OtherText = _model.OtherText;

        }
        public CommandContainer CommandContainer { get; set; }
        private (int yours, int others) CardsChosen()
        {
            int yours = _gameContainer!.SingleInfo!.LifeStory!.ObjectSelected();
            int others = _gameContainer.OtherCardSelected();
            return (yours, others);
        }
        [Command(EnumCommandCategory.Plain)]
        public async Task ProcessOtherAsync()
        {
            (int yours, int others) = CardsChosen();
            if (IsValidMove(yours, others) == false)
            {
                await UIPlatform.ShowMessageAsync("Illegal Move");
                return;
            }
            if (yours == 0 && others == 0)
                throw new BasicBlankException("Must have chosen at least one");
            int decks;
            if (yours == 0)
                decks = others;
            else if (others == 0)
                decks = yours;
            else
                decks = 0;
            if (decks > 0)
            {
                if (_gameContainer.BasicData!.MultiPlayer)
                    await _gameContainer.Network!.SendAllAsync("cardchosen", decks);
                var tempCard = _gameContainer.DeckList!.GetSpecificItem(decks);
                CommandContainer!.ManuelFinish = true;
                await _mainGame.ChoseSingleCardAsync(tempCard);
                return;
            }
            if (_gameContainer.BasicData!.MultiPlayer)
            {
                TradeCard thisTrade = new TradeCard();
                thisTrade.YourCard = yours;
                thisTrade.OtherCard = others;
                await _gameContainer.Network!.SendAllAsync("cardstraded", thisTrade);
            }
            var yourCard = _gameContainer.DeckList!.GetSpecificItem(yours);
            var opponentCard = _gameContainer.DeckList.GetSpecificItem(others);
            CommandContainer!.ManuelFinish = true;
            await _mainGame.TradeCardsAsync(yourCard, opponentCard);
        }

        private bool IsValidMove(int yours, int others)
        {
            var thisCard = _model.CurrentPile!.GetCardInfo();
            LifeCardGameCardInformation otherCard;
            if (thisCard.Action == EnumAction.Lawsuit || thisCard.Action == EnumAction.DonateToCharity)
            {
                if (others > 0)
                    throw new BasicBlankException("Should have only chosen your own card");

                if (yours == 0)
                    return false;
                otherCard = _gameContainer!.DeckList!.GetSpecificItem(yours);
                if (thisCard.Action == EnumAction.Lawsuit)
                    return otherCard.SpecialCategory != EnumSpecialCardCategory.Marriage && otherCard.Points >= 30;
                return otherCard.Category == EnumFirstCardCategory.Wealth && otherCard.Points > 5 && otherCard.SpecialCategory != EnumSpecialCardCategory.Passport;
            }
            if (thisCard.Action == EnumAction.MixUpAtVets || thisCard.Action == EnumAction.MovingHouse || thisCard.Action == EnumAction.CareerSwap)

                return yours > 0 && others > 0;
            if (yours > 0)
                throw new BasicBlankException("Should have only been allowed to choose another player");
            return others > 0;
        }

    }
}