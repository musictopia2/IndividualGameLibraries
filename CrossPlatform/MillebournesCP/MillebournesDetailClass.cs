using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace MillebournesCP
{
    [SingletonGame]
    public class MillebournesDetailClass : IGameInfo, ICardInfo<MillebournesCardInformation>
    {
        EnumGameType IGameInfo.GameType => EnumGameType.Rounds; //if rounds, change it here.

        bool IGameInfo.CanHaveExtraComputerPlayers => true; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Millebournes"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 5; //can't be 5 players though.

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 6; //if different, put here.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<MillebournesCardInformation>.CardsToPassOut => 6;

        CustomBasicList<int> ICardInfo<MillebournesCardInformation>.ExcludeList => new CustomBasicList<int>();

        bool ICardInfo<MillebournesCardInformation>.AddToDiscardAtBeginning => false;

        bool ICardInfo<MillebournesCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<MillebournesCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<MillebournesCardInformation>.PassOutAll => false;

        bool ICardInfo<MillebournesCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<MillebournesCardInformation>.NoPass => false;

        bool ICardInfo<MillebournesCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<MillebournesCardInformation> ICardInfo<MillebournesCardInformation>.DummyHand { get; set; } = new DeckObservableDict<MillebournesCardInformation>();

        bool ICardInfo<MillebournesCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<MillebournesCardInformation>.CanSortCardsToBeginWith => true;
    }
}