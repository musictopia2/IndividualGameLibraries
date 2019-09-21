using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicGameDataClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
namespace FluxxCP
{
    [SingletonGame]
    public class FluxxDetailClass : IGameInfo, ICardInfo<FluxxCardInformation>, INewCard<FluxxCardInformation>
    {
        EnumGameType IGameInfo.GameType => EnumGameType.NewGame; //if rounds, change it here.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Fluxx"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 6; //if different, put here.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<FluxxCardInformation>.CardsToPassOut => 3;

        CustomBasicList<int> ICardInfo<FluxxCardInformation>.ExcludeList => new CustomBasicList<int>() { 1 }; //so this will not be considered.

        bool ICardInfo<FluxxCardInformation>.AddToDiscardAtBeginning => false;

        bool ICardInfo<FluxxCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<FluxxCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<FluxxCardInformation>.PassOutAll => false;

        bool ICardInfo<FluxxCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<FluxxCardInformation>.NoPass => false;

        bool ICardInfo<FluxxCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<FluxxCardInformation> ICardInfo<FluxxCardInformation>.DummyHand { get; set; } = new DeckObservableDict<FluxxCardInformation>();

        bool ICardInfo<FluxxCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<FluxxCardInformation>.CanSortCardsToBeginWith => true;

        FluxxCardInformation INewCard<FluxxCardInformation>.GetNewCard(int chosen)
        {
            return GetNewCard(chosen);
        }
        public static FluxxCardInformation GetNewCard(int chosen)
        {
            if (chosen <= 0)
                throw new BasicBlankException("Must choose a value above 0 for GetNewCard for Fluxx");
            if (chosen <= 22)
                return new RuleCard();
            if (chosen <= 40)
                return new KeeperCard();
            if (chosen <= 63)
                return new GoalCard();
            if (chosen <= 83)
                return new ActionCard();
            throw new BasicBlankException("Must go only up to 83");
        }
    }
}