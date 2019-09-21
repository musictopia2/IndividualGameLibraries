using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace FillOrBustCP
{
    [SingletonGame]
    public class FillOrBustDetailClass : IGameInfo, ICardInfo<FillOrBustCardInformation>
    {
        EnumGameType IGameInfo.GameType => EnumGameType.NewGame;

        bool IGameInfo.CanHaveExtraComputerPlayers => false;

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.HumanOnly;

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Fill Or Bust";

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 8; //otherwise, even small tablet does not work right.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<FillOrBustCardInformation>.CardsToPassOut => 0; //change to what you need.

        CustomBasicList<int> ICardInfo<FillOrBustCardInformation>.ExcludeList => new CustomBasicList<int>();

        bool ICardInfo<FillOrBustCardInformation>.AddToDiscardAtBeginning => false;

        bool ICardInfo<FillOrBustCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<FillOrBustCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<FillOrBustCardInformation>.PassOutAll => false;

        bool ICardInfo<FillOrBustCardInformation>.PlayerGetsCards => false;

        bool ICardInfo<FillOrBustCardInformation>.NoPass => true;

        bool ICardInfo<FillOrBustCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<FillOrBustCardInformation> ICardInfo<FillOrBustCardInformation>.DummyHand { get; set; } = new DeckObservableDict<FillOrBustCardInformation>();

        bool ICardInfo<FillOrBustCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<FillOrBustCardInformation>.CanSortCardsToBeginWith => false;
    }
}