using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Extensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
namespace MillebournesCP
{
    [SingletonGame]
    public class ComputerAI
    {
        public struct MoveInfo
        {
            public int Team; // this is the team the card will be played on (if any)
            public bool WillThrowAway;
            public int Deck; // this is the card being played
            public EnumPileType WhichPile;
        }
        private DeckRegularDict<MillebournesCardInformation> _extendedList = new DeckRegularDict<MillebournesCardInformation>();
        private void GenerateExtendedList()
        {
            _extendedList = new DeckRegularDict<MillebournesCardInformation>();
            foreach (var thisCard in _mainGame.SingleInfo!.MainHandList)
                _extendedList.Add(thisCard);
            if (_mainGame.ThisMod!.Pile1!.PileEmpty() == false)
                _extendedList.Add(_mainGame.ThisMod.Pile1.GetCardInfo());
        }
        private DeckRegularDict<MillebournesCardInformation> PossibleMoves()
        {
            DeckRegularDict<MillebournesCardInformation> output = new DeckRegularDict<MillebournesCardInformation>();
            _extendedList.ForEach(thisCard =>
            {
                _mainGame.CurrentCP!.CurrentCard = thisCard;
                if (AcceptMove)
                    output.Add(thisCard);
            });
            return output;
        }
        private bool AcceptMove
        {
            get
            {
                return _mainGame!.CurrentCP!.CurrentCard!.CardType switch
                {
                    EnumCardCategories.Miles => _mainGame.CurrentCP.CanPlaceMiles(out _),
                    EnumCardCategories.Hazard => _mainGame.TeamList.Any(items => items.HasSpeedLimit == false && items.TeamNumber != _mainGame.SaveRoot!.CurrentTeam),
                    EnumCardCategories.Speed => _mainGame.TeamList.Any(items => items.WhichHazard == EnumHazardType.None && items.TeamNumber != _mainGame.SaveRoot!.CurrentTeam),
                    EnumCardCategories.Remedy => _mainGame.CurrentCP.CanFixHazard(out _),
                    EnumCardCategories.EndLimit => _mainGame.CurrentCP.CanEndSpeedLimit(out _),
                    EnumCardCategories.Safety => _mainGame.CurrentCP.CanPlaceSafety(out _),
                    _ => throw new BasicBlankException("Cannot find out whether the move was acceptable based on the card type"),
                };
            }
        }
        private int MilesPlaced(IDeckDict<MillebournesCardInformation> moveList)
        {
            var thisCard = moveList.Where(items => items.CardType == EnumCardCategories.Miles).OrderByDescending(items => items.Mileage).FirstOrDefault();
            if (thisCard == null)
                return 0;
            return thisCard.Deck;
        }
        private bool IsRoundAlmostOver(int previous, IDeckDict<MillebournesCardInformation> playList)
        {
            if (_mainGame.ThisMod!.Deck1!.IsEndOfDeck() == true)
                return true;
            int manys = playList.Count(items => items.Mileage == 200);
            int safes = _mainGame.TeamList.Sum(items => items.NumberOfSafeties) + previous;
            foreach (var thisTeam in _mainGame.TeamList)
            {
                if (thisTeam.TeamNumber != _mainGame.SaveRoot!.CurrentTeam && _mainGame.PlayerList.Count() <= 3 || _mainGame.PlayerList.Count() > 3)
                {
                    if (thisTeam.HasSpeedLimit == false && thisTeam.Miles == 800 && manys < 4 && thisTeam.WhichHazard == EnumHazardType.None && safes < 3)
                        return true;
                    if (thisTeam.HasSpeedLimit == false && thisTeam.Miles >= 900 && manys < 4 && thisTeam.WhichHazard == EnumHazardType.None && safes < 3)
                        return true;
                    if (thisTeam.Miles >= 950 && thisTeam.WhichHazard == EnumHazardType.None && safes < 3)
                        return true;
                }
            }
            return false;
        }
        private DeckRegularDict<MillebournesCardInformation> Hazardlist(IDeckDict<MillebournesCardInformation> moveList)
        {
            DeckRegularDict<MillebournesCardInformation> output = new DeckRegularDict<MillebournesCardInformation>();
            output.AddRange(moveList);
            output.KeepConditionalItems(items => items.CardType == EnumCardCategories.Hazard);
            return output;
        }
        private DeckRegularDict<MillebournesCardInformation> GetPlayList()
        {
            DeckRegularDict<MillebournesCardInformation> output = new DeckRegularDict<MillebournesCardInformation>();
            _mainGame.TeamList.ForEach(thisTeam =>
            {
                output.AddRange(thisTeam.CardsPlayed);
            });
            output.AddRange(_mainGame.ThisMod!.Pile2!.DiscardList());
            if (_mainGame.ThisMod.Pile2.PileEmpty() == false)
                output.Add(_mainGame.ThisMod.Pile2.GetCardInfo());
            output.AddRange(_extendedList);
            return output;
        }
        private int ThrowAwayMiles(IDeckDict<MillebournesCardInformation> throwList, bool hadSpeed)
        {
            var temps = throwList.OrderBy(items => items.Mileage).ToRegularDeckDict();
            if (hadSpeed == true)
                temps.KeepConditionalItems(items => items.CardType == EnumCardCategories.Miles && items.Mileage <= 50);
            else
                temps.KeepConditionalItems(items => items.CardType == EnumCardCategories.Miles);
            if (temps.Count == 0)
                return 0;
            return temps.First().Deck;
        }
        private bool DupCards(MillebournesCardInformation thisCard)
        {
            if (thisCard.CardType != EnumCardCategories.EndLimit && thisCard.CardType != EnumCardCategories.Remedy)
                return false;
            return _extendedList.Count(items => items.CardName == thisCard.CardName) > 1;
        }
        private MoveInfo ComputerThrow()
        {
            MoveInfo output = new MoveInfo();
            output.WillThrowAway = true;
            int milesNeeded = 1000 - _mainGame.CurrentCP!.Miles;
            DeckRegularDict<MillebournesCardInformation> throwList = _extendedList.Where(items => items.CardType != EnumCardCategories.Safety).ToRegularDeckDict();
            var playList = GetPlayList();
            foreach (var thisCard in throwList)
            {
                if (thisCard.CardType == EnumCardCategories.Miles && thisCard.Mileage == 200 && _mainGame.CurrentCP.HowMany200S >= 2)
                {
                    output.Deck = thisCard.Deck;
                    return output;
                }
                if (thisCard.CardType == EnumCardCategories.Miles && thisCard.Mileage > milesNeeded)
                {
                    output.Deck = thisCard.Deck;
                    return output;
                }
                if (thisCard.CardName == "Gasoline" && _mainGame.CurrentCP.SafetyHas("Extra Tank"))
                {
                    output.Deck = thisCard.Deck;
                    return output;
                }
                if (thisCard.CardName == "Repairs" && _mainGame.CurrentCP.SafetyHas("Driving Ace"))
                {
                    output.Deck = thisCard.Deck;
                    return output;
                }
                if (thisCard.CardName == "Spare Tire" && _mainGame.CurrentCP.SafetyHas("Puncture Proof"))
                {
                    output.Deck = thisCard.Deck;
                    return output;
                }
                if (thisCard.CardName == "Roll" && _mainGame.CurrentCP.SafetyHas("Right Of Way"))
                {
                    output.Deck = thisCard.Deck;
                    return output;
                }
                if (thisCard.CardName == "End Of Limit" && _mainGame.CurrentCP.SafetyHas("Right Of Way"))
                {
                    output.Deck = thisCard.Deck;
                    return output;
                }
                if (thisCard.CardName == "Roll" && playList.Count(items => items.CardName == "Stop") > 5)
                {
                    output.Deck = thisCard.Deck;
                    return output;
                }
                if (thisCard.CardName == "Gasoline" && playList.Count(items => items.CardName == "Out Of Gas") > 2)
                {
                    output.Deck = thisCard.Deck;
                    return output;
                }
                if (thisCard.CardName == "Spare Tire" && playList.Count(items => items.CardName == "Flat Tire") > 2)
                {
                    output.Deck = thisCard.Deck;
                    return output;
                }
                if (thisCard.CardName == "Repairs" && playList.Count(items => items.CardName == "Accident") > 2)
                {
                    output.Deck = thisCard.Deck;
                    return output;
                }
                if (thisCard.CardName == "End Of Limit" && playList.Count(items => items.CardName == "Speed Limit") > 3)
                {
                    output.Deck = thisCard.Deck;
                    return output;
                }
            }
            foreach (var thisCard in throwList)
            {
                if (DupCards(thisCard))
                {
                    output.Deck = thisCard.Deck;
                    return output;
                }
            }
            int decks = ThrowAwayMiles(throwList, _mainGame.CurrentCP.HasSpeedLimit);
            if (decks > 0)
            {
                output.Deck = decks;
                return output;
            }
            output.Deck = throwList.GetRandomItem().Deck;
            return output;
        }
        public bool CanGiveOne { get; private set; }

        private MoveInfo BestHazard(IDeckDict<MillebournesCardInformation> thisList)
        {
            CustomBasicList<MoveInfo> possibleList = new CustomBasicList<MoveInfo>();
            MoveInfo thisMove;
            thisList.ForEach(thisCard =>
            {
                _mainGame.TeamList.ForEach(thisTeam =>
                {
                    thisTeam.CurrentCard = thisCard;
                    if (thisCard.CardType == EnumCardCategories.Speed)
                    {
                        if (thisTeam.CanGiveSpeedLimit(out _))
                        {
                            thisMove = new MoveInfo();
                            thisMove.WhichPile = EnumPileType.Speed;
                            thisMove.Team = thisTeam.TeamNumber;
                            thisMove.Deck = thisCard.Deck;
                            possibleList.Add(thisMove);
                        }
                    }
                    else if (thisTeam.CanGiveHazard(out _))
                    {
                        thisMove = new MoveInfo();
                        thisMove.WhichPile = EnumPileType.Hazard;
                        thisMove.Team = thisTeam.TeamNumber;
                        thisMove.Deck = thisCard.Deck;
                        possibleList.Add(thisMove);
                    }
                });
            });
            if (possibleList.Count == 0)
            {
                CanGiveOne = false;
                return new MoveInfo();
            }
            CanGiveOne = true;
            return possibleList.GetRandomItem();
        }
        private int SafetyForHazard()
        {
            DeckRegularDict<MillebournesCardInformation> whatList = _extendedList.Where(items => items.CardType == EnumCardCategories.Safety).ToRegularDeckDict();
            if (whatList.Count == 0)
                return 0;
            foreach (var thisCard in whatList)
            {
                if (thisCard.CardName == "Extra Tank" && _mainGame.CurrentCP!.WhichHazard == EnumHazardType.OutOfGas)
                    return thisCard.Deck;
                if (thisCard.CardName == "Driving Ace" && _mainGame.CurrentCP!.WhichHazard == EnumHazardType.Accident)
                    return thisCard.Deck;
                if (thisCard.CardName == "Puncture Proof" && _mainGame.CurrentCP!.WhichHazard == EnumHazardType.FlatTire)
                    return thisCard.Deck;
                if (thisCard.CardName == "Right Of Way" && (_mainGame.CurrentCP!.HasSpeedLimit == true || _mainGame.CurrentCP.WhichHazard == EnumHazardType.StopSign))
                    return thisCard.Deck;
            }
            return 0;
        }
        private int WhatSafety()
        {
            DeckRegularDict<MillebournesCardInformation> whatList = _extendedList.Where(items => items.CardType == EnumCardCategories.Safety).ToRegularDeckDict();
            if (whatList.Count == 0)
                return 0;
            var playList = GetPlayList();
            if (IsRoundAlmostOver(whatList.Count, playList))
                return whatList.GetRandomItem().Deck;
            foreach (var thisCard in whatList)
            {
                if (thisCard.CardName == "Extra Tank" && playList.Count(items => items.CardName == "Out Of Gas") == 3)
                    return thisCard.Deck;
                if (thisCard.CardName == "Driving Ace" && playList.Count(items => items.CardName == "Flat Tire") == 3)
                    return thisCard.Deck;
                if (thisCard.CardName == "Puncture Proof" && playList.Count(items => items.CardName == "Accident") == 3)
                    return thisCard.Deck;
                if (thisCard.CardName == "Right Of Way" && playList.Count(items => items.CardName == "Stop") == 6 && playList.Count(items => items.CardName == "Speed Limit") == 4)
                    return thisCard.Deck;
            }
            return 0;
        }
        private CustomBasicList<MoveInfo> RemedyList(IDeckDict<MillebournesCardInformation> moveList)
        {
            CustomBasicList<MoveInfo> output = new CustomBasicList<MoveInfo>();
            var temps = moveList.Where(items => items.CardType == EnumCardCategories.Remedy || items.CardType == EnumCardCategories.EndLimit).ToRegularDeckDict();
            temps.ForEach(thisCard =>
            {
                MoveInfo thisMove = new MoveInfo();
                thisMove.Deck = thisCard.Deck;
                if (thisCard.CardType == EnumCardCategories.Remedy)
                    thisMove.WhichPile = EnumPileType.Hazard;
                else
                    thisMove.WhichPile = EnumPileType.Speed;
                thisMove.Team = _mainGame.SaveRoot!.CurrentTeam;
                output.Add(thisMove);
            });
            return output;
        }
        private readonly MillebournesMainGameClass _mainGame;
        public ComputerAI(MillebournesMainGameClass mainGame)
        {
            _mainGame = mainGame;
        }
        public MoveInfo ComputerMove()
        {
            _mainGame.SingleInfo = _mainGame.PlayerList!.GetWhoPlayer();
            _mainGame.CurrentCP = _mainGame.FindTeam(_mainGame.SingleInfo.Team);
            GenerateExtendedList();
            var moveList = PossibleMoves();
            if (moveList.Count == 0)
                return ComputerThrow();
            var whatHazards = Hazardlist(moveList);
            if (whatHazards.Count > 0)
            {
                MoveInfo newMove = BestHazard(whatHazards);
                if (CanGiveOne)
                    return newMove;
            }
            int safes = WhatSafety();
            MoveInfo thisMove;
            if (safes > 0)
            {
                thisMove = new MoveInfo();
                thisMove.Team = _mainGame.SaveRoot!.CurrentTeam;
                thisMove.Deck = safes;
                thisMove.WhichPile = EnumPileType.Safety;
                return thisMove;
            }
            var newList = RemedyList(moveList);
            if (newList.Count > 0)
                return newList.GetRandomItem();
            safes = SafetyForHazard();
            if (safes > 0)
            {
                thisMove = new MoveInfo();
                thisMove.Team = _mainGame.SaveRoot!.CurrentTeam;
                thisMove.Deck = safes;
                thisMove.WhichPile = EnumPileType.Safety;
                return thisMove;
            }
            int miles = MilesPlaced(moveList);
            if (miles > 0)
            {
                thisMove = new MoveInfo();
                thisMove.Team = _mainGame.SaveRoot!.CurrentTeam;
                thisMove.Deck = miles;
                thisMove.WhichPile = EnumPileType.Miles;
                return thisMove;
            }
            return ComputerThrow();
        }
    }
}