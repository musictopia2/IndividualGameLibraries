using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace MonasteryCardGameCP
{
    [SingletonGame]
    public class MonasteryCardGameDetailClass : IGameInfo, ICardInfo<MonasteryCardInfo>
    {
        EnumGameType IGameInfo.GameType => EnumGameType.Rounds; //if rounds, change it here.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.NetworkOnly;

        string IGameInfo.GameName => "Monastery Card Game"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 4; //if different, put here.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<MonasteryCardInfo>.CardsToPassOut => 9; //change to what you need.

        CustomBasicList<int> ICardInfo<MonasteryCardInfo>.ExcludeList => new CustomBasicList<int>();

        bool ICardInfo<MonasteryCardInfo>.AddToDiscardAtBeginning => true;

        bool ICardInfo<MonasteryCardInfo>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<MonasteryCardInfo>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<MonasteryCardInfo>.PassOutAll => false;

        bool ICardInfo<MonasteryCardInfo>.PlayerGetsCards => true;

        bool ICardInfo<MonasteryCardInfo>.NoPass => false;

        bool ICardInfo<MonasteryCardInfo>.NeedsDummyHand => false;

        DeckObservableDict<MonasteryCardInfo> ICardInfo<MonasteryCardInfo>.DummyHand { get; set; } = new DeckObservableDict<MonasteryCardInfo>();

        bool ICardInfo<MonasteryCardInfo>.HasDrawAnimation => true;

        bool ICardInfo<MonasteryCardInfo>.CanSortCardsToBeginWith => true;
    }
}