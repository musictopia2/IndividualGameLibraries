using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CribbageCP.Logic;
namespace CribbageCP.Data
{
    [SingletonGame]
    [AutoReset]
    public class CribbageDetailClass : IGameInfo, ICardInfo<CribbageCard>
    {
        private readonly CribbageDelegates _delegates;

        public CribbageDetailClass(CribbageDelegates delegates)
        {
            _delegates = delegates;
        }
        EnumGameType IGameInfo.GameType => EnumGameType.Rounds; //if rounds, change it here.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Cribbage"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 3; //if different, put here.

        bool IGameInfo.CanAutoSave => false;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<CribbageCard>.CardsToPassOut
        {
            get
            {
                if (_delegates.GetPlayerCount == null)
                {
                    throw new BasicBlankException("Nobody is handling get player count.  Rethink");
                }
                int count = _delegates.GetPlayerCount.Invoke();
                if (count == 2)
                    return 6;
                else if (count == 3)
                    return 5;
                else
                    throw new BasicBlankException("Only 2 or 3 players are supported");
            }
        }
        CustomBasicList<int> ICardInfo<CribbageCard>.PlayerExcludeList => new CustomBasicList<int>();

        bool ICardInfo<CribbageCard>.AddToDiscardAtBeginning => true;

        bool ICardInfo<CribbageCard>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<CribbageCard>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<CribbageCard>.PassOutAll => false;

        bool ICardInfo<CribbageCard>.PlayerGetsCards => true;

        bool ICardInfo<CribbageCard>.NoPass => false;

        bool ICardInfo<CribbageCard>.NeedsDummyHand => false;

        DeckObservableDict<CribbageCard> ICardInfo<CribbageCard>.DummyHand { get; set; } = new DeckObservableDict<CribbageCard>();

        bool ICardInfo<CribbageCard>.HasDrawAnimation => true;

        bool ICardInfo<CribbageCard>.CanSortCardsToBeginWith => true;

        CustomBasicList<int> ICardInfo<CribbageCard>.DiscardExcludeList(IListShuffler<CribbageCard> deckList)
        {
            return new CustomBasicList<int>();
        }
    }
}