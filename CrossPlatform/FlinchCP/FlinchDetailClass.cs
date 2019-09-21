using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace FlinchCP
{
    [SingletonGame]
    public class FlinchDetailClass : IGameInfo, ICardInfo<FlinchCardInformation>
    {
        EnumGameType IGameInfo.GameType => EnumGameType.NewGame;

        bool IGameInfo.CanHaveExtraComputerPlayers => true;

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Flinch";

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 4;

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //requires landscape because of the many piles.

        int ICardInfo<FlinchCardInformation>.CardsToPassOut => 10; //change to what you need.

        CustomBasicList<int> ICardInfo<FlinchCardInformation>.ExcludeList => new CustomBasicList<int>();

        bool ICardInfo<FlinchCardInformation>.AddToDiscardAtBeginning => false;

        bool ICardInfo<FlinchCardInformation>.ReshuffleAllCardsFromDiscard => true;

        bool ICardInfo<FlinchCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<FlinchCardInformation>.PassOutAll => false;

        bool ICardInfo<FlinchCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<FlinchCardInformation>.NoPass => false;

        bool ICardInfo<FlinchCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<FlinchCardInformation> ICardInfo<FlinchCardInformation>.DummyHand { get; set; } = new DeckObservableDict<FlinchCardInformation>();

        bool ICardInfo<FlinchCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<FlinchCardInformation>.CanSortCardsToBeginWith => false;
    }
}