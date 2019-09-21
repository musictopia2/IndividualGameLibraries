using BasicGameFramework.Attributes;
using BasicGameFramework.GameGraphicsCP.GameboardPositionHelpers;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Collections.Generic;
using System.Linq;
namespace ClueBoardGameCP
{
    [SingletonGame]
    public class GlobalClass
    {
        public Dictionary<int, DetectiveInfo> DetectiveList = new Dictionary<int, DetectiveInfo>();
        public Dictionary<int, CharacterInfo> CharacterList
        {
            get
            {
                return _mainGame.SaveRoot!.CharacterList;
            }
            set
            {
                _mainGame.SaveRoot!.CharacterList = value;
            }
        }
        public Dictionary<int, WeaponInfo> WeaponList
        {
            get
            {
                return _mainGame.SaveRoot!.WeaponList;
            }
            set
            {
                _mainGame.SaveRoot!.WeaponList = value;
            }
        }
        public Dictionary<int, RoomInfo> RoomList = new Dictionary<int, RoomInfo>();
        public CharacterInfo? CurrentCharacter { get; set; }
        private readonly ClueBoardGameMainGameClass _mainGame;
        public GlobalClass(ClueBoardGameMainGameClass mainGame)
        {
            _mainGame = mainGame;
        }
        public CardInfo ClueInfo(int deck)
        {
            CardInfo output = new CardInfo();
            output.Deck = deck;
            int tempDeck;
            switch (deck)
            {
                case int _ when deck < 7:
                    {
                        output.WhatType = EnumCardType.IsCharacter;
                        tempDeck = deck;
                        break;
                    }

                case int _ when deck < 13:
                    {
                        output.WhatType = EnumCardType.IsWeapon;
                        tempDeck = deck - 6;
                        break;
                    }

                default:
                    {
                        output.WhatType = EnumCardType.IsRoom;
                        tempDeck = deck - 12;
                        break;
                    }
            }
            output.Name = FindCardName(tempDeck, output.WhatType);
            output.CardValue = deck.ToEnum<EnumCardValues>();
            if (string.IsNullOrEmpty(output.Name))
                throw new BasicBlankException("Cannot have a blank name");
            return output;
        }
        public bool CanGiveCard(CardInfo thisCard)
        {
            if (thisCard.Name == _mainGame.SaveRoot!.CurrentPrediction!.CharacterName)
                return true;
            if (thisCard.Name == _mainGame.SaveRoot.CurrentPrediction.WeaponName)
                return true;
            if (thisCard.Name == _mainGame.SaveRoot.CurrentPrediction.RoomName)
                return true;
            return false;
        }
        private string FindCardName(int deck, EnumCardType whatType)
        {
            if (whatType == EnumCardType.IsCharacter)
            {
                if (deck == 1)
                    return "Mrs. Peacock";
                if (deck == 2)
                    return "Mr. Green";
                if (deck == 3)
                    return "Professor Plum";
                if (deck == 4)
                    return "Miss Scarlet";
                if (deck == 5)
                    return "Mrs. White";
                if (deck == 6)
                    return "Colonel Mustard";
                throw new BasicBlankException("Not Found");
            }
            if (whatType == EnumCardType.IsRoom)
                return RoomList[deck].Name;
            if (whatType == EnumCardType.IsWeapon)
                return WeaponList[deck].Name;
            throw new BasicBlankException("Wrong type");
        }
        public CustomBasicList<WeaponInfo> WeaponsInRoom(int Room) //i think this should be fine too.
        {
            return WeaponList.Values.Where(items => items.Room == Room).ToCustomBasicList();
        }
        public CustomBasicList<CharacterInfo> CharactersOnStart()
        {
            return CharacterList.Values.Where(items => items.FirstNumber > 0 && items.CurrentRoom == 0 && items.PreviousRoom == 0 && items.Space == 0).ToCustomBasicList();
        }
        public CustomBasicList<CharacterInfo> CharactersOnBoard()
        {
            return CharacterList.Values.Where(items => items.Space > 0).ToCustomBasicList();
        }
        public CustomBasicList<CharacterInfo> CharactersInRoom(int room)
        {
            return CharacterList.Values.Where(items => items.CurrentRoom == room).ToCustomBasicList();
        }
        public RoomInfo GetRoom(string room)
        {
            IEnumerable<RoomInfo> temps;
            temps = from Rooms in RoomList.Values
                    where (Rooms.Name ?? "") == (room ?? "")
                    select Rooms;
            return temps.Single();
        }
        public WeaponInfo GetWeapon(string weapon)
        {
            IEnumerable<WeaponInfo> temps;
            temps = from Weapons in WeaponList.Values
                    where (Weapons.Name ?? "") == (weapon ?? "")
                    select Weapons;
            return temps.Single();
        }
        public CharacterInfo GetCharacter(string character)
        {
            IEnumerable<CharacterInfo> temps;
            temps = from Characters in CharacterList.Values
                    where (Characters.Name ?? "") == (character ?? "")
                    select Characters;
            return temps.Single();
        }
        public PositionPieces ThisPos = new PositionPieces(); //try to make it here instead of gameboard1.
    }
}