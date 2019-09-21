using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace MilkRunCP
{
    [SingletonGame]
    public class MilkRunDetailClass : IGameInfo, ICardInfo<MilkRunCardInformation>
    {
        EnumGameType IGameInfo.GameType => EnumGameType.NewGame; //if rounds, change it here.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Milk Run"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 2; //if different, put here.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<MilkRunCardInformation>.CardsToPassOut => 6;

        CustomBasicList<int> ICardInfo<MilkRunCardInformation>.ExcludeList => new CustomBasicList<int>();

        bool ICardInfo<MilkRunCardInformation>.AddToDiscardAtBeginning => false;

        bool ICardInfo<MilkRunCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<MilkRunCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<MilkRunCardInformation>.PassOutAll => false;

        bool ICardInfo<MilkRunCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<MilkRunCardInformation>.NoPass => false;

        bool ICardInfo<MilkRunCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<MilkRunCardInformation> ICardInfo<MilkRunCardInformation>.DummyHand { get; set; } = new DeckObservableDict<MilkRunCardInformation>();

        bool ICardInfo<MilkRunCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<MilkRunCardInformation>.CanSortCardsToBeginWith => true;
    }
}