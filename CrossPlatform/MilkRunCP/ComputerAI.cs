using BasicGameFramework.Attributes;
using CommonBasicStandardLibraries.CollectionClasses;
namespace MilkRunCP
{
    [SingletonGame]
    public class ComputerAI
    {
        public struct MoveInfo
        {
            public int Player;
            public bool ToDiscard;
            public EnumMilkType Milk;
            public EnumPileType Pile;
            public int Deck;
        }
        private readonly MilkRunMainGameClass _mainGame;
        public ComputerAI(MilkRunMainGameClass mainGame)
        {
            _mainGame = mainGame;
        }
        private CustomBasicList<MoveInfo> MoveList(int deck)
        {
            CustomBasicList<MoveInfo> output = new CustomBasicList<MoveInfo>();
            int selfPlayer = _mainGame.PlayerList!.GetSelf().Id;
            int computerPlayer;
            MoveInfo thisMove;
            if (selfPlayer == 1)
                computerPlayer = 2;
            else
                computerPlayer = 1;
            if (_mainGame.CanMakeMove(computerPlayer, deck, EnumPileType.Deliveries, EnumMilkType.Chocolate))
            {
                thisMove = new MoveInfo();
                thisMove.Deck = deck;
                thisMove.Player = computerPlayer;
                thisMove.Milk = EnumMilkType.Chocolate;
                thisMove.Pile = EnumPileType.Deliveries;
                output.Add(thisMove);
            }
            if (_mainGame.CanMakeMove(computerPlayer, deck, EnumPileType.Go, EnumMilkType.Chocolate))
            {
                thisMove = new MoveInfo();
                thisMove.Deck = deck;
                thisMove.Player = computerPlayer;
                thisMove.Milk = EnumMilkType.Chocolate;
                thisMove.Pile = EnumPileType.Go;
                output.Add(thisMove);
            }
            if (_mainGame.CanMakeMove(computerPlayer, deck, EnumPileType.Limit, EnumMilkType.Chocolate))
            {
                thisMove = new MoveInfo();
                thisMove.Deck = deck;
                thisMove.Player = computerPlayer;
                thisMove.Milk = EnumMilkType.Chocolate;
                thisMove.Pile = EnumPileType.Limit;
                output.Add(thisMove);
            }
            if (_mainGame.CanMakeMove(computerPlayer, deck, EnumPileType.Deliveries, EnumMilkType.Strawberry))
            {
                thisMove = new MoveInfo();
                thisMove.Deck = deck;
                thisMove.Player = computerPlayer;
                thisMove.Milk = EnumMilkType.Strawberry;
                thisMove.Pile = EnumPileType.Deliveries;
                output.Add(thisMove);
            }
            if (_mainGame.CanMakeMove(computerPlayer, deck, EnumPileType.Go, EnumMilkType.Strawberry))
            {
                thisMove = new MoveInfo();
                thisMove.Deck = deck;
                thisMove.Player = computerPlayer;
                thisMove.Milk = EnumMilkType.Strawberry;
                thisMove.Pile = EnumPileType.Go;
                output.Add(thisMove);
            }
            if (_mainGame.CanMakeMove(computerPlayer, deck, EnumPileType.Limit, EnumMilkType.Strawberry))
            {
                thisMove = new MoveInfo();
                thisMove.Deck = deck;
                thisMove.Player = computerPlayer;
                thisMove.Milk = EnumMilkType.Strawberry;
                thisMove.Pile = EnumPileType.Limit;
                output.Add(thisMove);
            }
            //human player;
            if (_mainGame.CanMakeMove(selfPlayer, deck, EnumPileType.Deliveries, EnumMilkType.Chocolate))
            {
                thisMove = new MoveInfo();
                thisMove.Deck = deck;
                thisMove.Player = selfPlayer;
                thisMove.Milk = EnumMilkType.Chocolate;
                thisMove.Pile = EnumPileType.Deliveries;
                output.Add(thisMove);
            }
            if (_mainGame.CanMakeMove(selfPlayer, deck, EnumPileType.Go, EnumMilkType.Chocolate))
            {
                thisMove = new MoveInfo();
                thisMove.Deck = deck;
                thisMove.Player = selfPlayer;
                thisMove.Milk = EnumMilkType.Chocolate;
                thisMove.Pile = EnumPileType.Go;
                output.Add(thisMove);
            }
            if (_mainGame.CanMakeMove(selfPlayer, deck, EnumPileType.Limit, EnumMilkType.Chocolate))
            {
                thisMove = new MoveInfo();
                thisMove.Deck = deck;
                thisMove.Player = selfPlayer;
                thisMove.Milk = EnumMilkType.Chocolate;
                thisMove.Pile = EnumPileType.Limit;
                output.Add(thisMove);
            }
            if (_mainGame.CanMakeMove(selfPlayer, deck, EnumPileType.Deliveries, EnumMilkType.Strawberry))
            {
                thisMove = new MoveInfo();
                thisMove.Deck = deck;
                thisMove.Player = selfPlayer;
                thisMove.Milk = EnumMilkType.Strawberry;
                thisMove.Pile = EnumPileType.Deliveries;
                output.Add(thisMove);
            }
            if (_mainGame.CanMakeMove(selfPlayer, deck, EnumPileType.Go, EnumMilkType.Strawberry))
            {
                thisMove = new MoveInfo();
                thisMove.Deck = deck;
                thisMove.Player = selfPlayer;
                thisMove.Milk = EnumMilkType.Strawberry;
                thisMove.Pile = EnumPileType.Go;
                output.Add(thisMove);
            }
            if (_mainGame.CanMakeMove(selfPlayer, deck, EnumPileType.Limit, EnumMilkType.Strawberry))
            {
                thisMove = new MoveInfo();
                thisMove.Deck = deck;
                thisMove.Player = selfPlayer;
                thisMove.Milk = EnumMilkType.Strawberry;
                thisMove.Pile = EnumPileType.Limit;
                output.Add(thisMove);
            }
            return output;
        }
        public MoveInfo MoveToMake()
        {
            CustomBasicList<MoveInfo> newList = new CustomBasicList<MoveInfo>();
            _mainGame.SingleInfo!.MainHandList.ForEach(thisCard =>
            {
                newList.AddRange(MoveList(thisCard.Deck));
                MoveInfo thisMove = new MoveInfo();
                thisMove.ToDiscard = true;
                thisMove.Deck = thisCard.Deck;
                newList.Add(thisMove);
            });
            return newList.GetRandomItem();
        }
        public bool CanDraw => _mainGame.SaveRoot!.CardsDrawn < 2;
    }
}