using BasicGameFrameworkLibrary.Attributes;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using DutchBlitzCP.Cards;
using DutchBlitzCP.Data;
using System.Linq;

namespace DutchBlitzCP.Logic
{
    public class ComputerAI
    {
        private readonly DutchBlitzGameContainer _gameContainer;
        private readonly DutchBlitzVMData _model;

        public ComputerAI(DutchBlitzGameContainer gameContainer, DutchBlitzVMData model)
        {
            _moves = 0;
            _gameContainer = gameContainer;
            _model = model;
        }
        public enum EnumMoveType
        {
            ContinueMove = 1, ToPublic, Transfer
        }
        private const int _maxMoves = 8;
        private int _moves;
        public struct MoveInfo
        {
            public bool NewPublicPile { get; set; }
            public int PublicPile { get; set; }
            public bool FromStock { get; set; }
            public int DiscardPile { get; set; }
            public bool FromHand { get; set; }
            public bool AddDiscard { get; set; }
        }
        public void TransferCards()
        {
            _moves += 2;
            var thisCard = _gameContainer.CurrentComputer!.StockList.First();
            _gameContainer.CurrentComputer.Discard.Add(thisCard);
            _gameContainer.CurrentComputer.StockList.RemoveSpecificItem(thisCard);
            UpdateComputerCount();
        }
        private void UpdateComputerCount()
        {
            _gameContainer.SingleInfo!.StockLeft = _gameContainer.CurrentComputer!.StockList.Count;
        }
        public DutchBlitzCardInformation CardToUseForPublic(MoveInfo thisMove)
        {
            _moves += 3;
            DutchBlitzCardInformation thisCard;
            if (thisMove.FromStock)
            {
                thisCard = _gameContainer.CurrentComputer!.StockList.First();
                _gameContainer.CurrentComputer.StockList.RemoveSpecificItem(thisCard);
                UpdateComputerCount();
                return thisCard;
            }
            if (thisMove.FromHand)
            {
                thisCard = _gameContainer.CurrentComputer!.PileList.Last();
                _gameContainer.CurrentComputer.PileList.RemoveSpecificItem(thisCard);
                return thisCard;
            }
            thisCard = _gameContainer.CurrentComputer!.Discard[thisMove.DiscardPile];
            _gameContainer.CurrentComputer.Discard.RemoveSpecificItem(thisCard);
            return thisCard;
        }
        public void DrawCards()
        {
            _moves++;
            if (_gameContainer.CurrentComputer!.DeckList.Count == 0)
            {
                _gameContainer.CurrentComputer.DeckList.ReplaceRange(_gameContainer.CurrentComputer.PileList);
                _gameContainer.CurrentComputer.PileList.Clear();
            }
            int incs;
            if (_gameContainer.PlayerList.Count() == 2)
                incs = 1;
            else if (_gameContainer.PlayerList.Count() == 3)
                incs = 2;
            else
                incs = 3;
            incs.Times(x =>
            {
                if (_gameContainer.CurrentComputer.DeckList.Count == 0)
                    return;
                var thisCard = _gameContainer.CurrentComputer.DeckList.First();
                _gameContainer.CurrentComputer.PileList.Add(thisCard);
                _gameContainer.CurrentComputer.DeckList.RemoveSpecificItem(thisCard);
            });
        }
        public bool CanEndTurn => _moves >= _maxMoves;
        public void DeductMovePublic() => _moves += 3;
        public EnumMoveType CalculateMoveType(MoveInfo thisMove)
        {
            if (thisMove.DiscardPile == -1 && thisMove.FromStock == false && thisMove.FromHand == false)
                return EnumMoveType.ContinueMove;
            if (thisMove.AddDiscard && thisMove.FromStock)
                return EnumMoveType.Transfer;
            if (thisMove.NewPublicPile || thisMove.PublicPile > -1)
                return EnumMoveType.ToPublic;
            throw new BasicBlankException("Cannot figure out the move type for this");
        }
        public MoveInfo ComputerMove()
        {
            MoveInfo output = new MoveInfo();
            var thisCard = _gameContainer.CurrentComputer!.StockList.First();
            if (thisCard.CardValue == 1)
            {
                output.FromStock = true;
                output.NewPublicPile = true;
                return output;
            }
            int maxs = _model.PublicPiles1!.MaxPiles();
            int x;
            bool rets;
            for (x = 1; x <= maxs; x++)
            {
                rets = _model.PublicPiles1.CanAddToPile(thisCard, x - 1); // 0 based i think
                if (rets == true)
                {
                    output.FromStock = true;
                    output.PublicPile = x - 1; // because 0 based now.
                    return output;
                }
            }
            if (_gameContainer.CurrentComputer.Discard.Count < _gameContainer.MaxDiscard)
            {
                output.FromStock = true;
                output.AddDiscard = true;
                return output;
            }
            int y = 0;
            foreach (var tempCard in _gameContainer.CurrentComputer.Discard)
            {
                y++;
                if (tempCard.CardValue == 1)
                {
                    output.NewPublicPile = true;
                    output.DiscardPile = y - 1;
                    return output;
                }
                for (x = 1; x <= maxs; x++)
                {
                    rets = _model.PublicPiles1.CanAddToPile(tempCard, x - 1);
                    if (rets)
                    {
                        output.DiscardPile = y - 1;
                        output.PublicPile = x - 1;
                        return output;
                    }
                }
            }
            if (_gameContainer.CurrentComputer.PileList.Count > 0)
            {
                thisCard = _gameContainer.CurrentComputer.PileList.Last();
                if (thisCard.CardValue == 1)
                {
                    output.FromHand = true;
                    output.NewPublicPile = true;
                    return output;
                }
                for (x = 1; x <= maxs; x++)
                {
                    rets = _model.PublicPiles1.CanAddToPile(thisCard, x - 1);
                    if (rets)
                    {
                        output.FromHand = true;
                        output.PublicPile = x - 1;
                        return output;
                    }
                }
            }
            output.DiscardPile = -1;
            output.PublicPile = -1;
            output.FromStock = false;
            output.FromHand = false;
            return output;
        }
    }
}
