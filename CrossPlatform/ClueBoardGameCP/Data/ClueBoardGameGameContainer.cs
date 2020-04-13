using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.GameGraphicsCP.GameboardPositionHelpers;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using ClueBoardGameCP.Cards;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClueBoardGameCP.Data
{
    [SingletonGame]
    [AutoReset]
    public class ClueBoardGameGameContainer : BasicGameContainer<ClueBoardGamePlayerItem, ClueBoardGameSaveInfo>
    {
        public ClueBoardGameGameContainer(
            BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            RandomGenerator random) : base(basicData,
                test,
                gameInfo,
                delay,
                aggregator,
                command,
                resolver,
                random)
        {

        }

        public Dictionary<int, DetectiveInfo> DetectiveList = new Dictionary<int, DetectiveInfo>();
        public Dictionary<int, RoomInfo> RoomList = new Dictionary<int, RoomInfo>();
        public CharacterInfo? CurrentCharacter { get; set; }

        public Dictionary<int, CharacterInfo> CharacterList
        {
            get
            {
                return SaveRoot!.CharacterList;
            }
            set
            {
                SaveRoot!.CharacterList = value;
            }
        }
        public Dictionary<int, WeaponInfo> WeaponList
        {
            get
            {
                return SaveRoot!.WeaponList;
            }
            set
            {
                SaveRoot!.WeaponList = value;
            }
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
            if (thisCard.Name == SaveRoot!.CurrentPrediction!.CharacterName)
                return true;
            if (thisCard.Name == SaveRoot.CurrentPrediction.WeaponName)
                return true;
            if (thisCard.Name == SaveRoot.CurrentPrediction.RoomName)
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
        public PositionPieces Position = new PositionPieces();
        internal Func<int, Task>? SpaceClickedAsync { get; set; }
        internal Func<int, Task>? RoomClickedAsync { get; set; }
        internal bool CanClickSpace()
        {
            if (Test.DoubleCheck)
            {
                return true;
            }
            if (SaveRoot.GameStatus == EnumClueStatusList.FindClues)
            {
                return false;
            }
            if (SaveRoot.GameStatus == EnumClueStatusList.StartTurn)
            {
                return CurrentCharacter!.CurrentRoom > 0;
            }
            return SaveRoot.GameStatus == EnumClueStatusList.MoveSpaces;
        }
        public int TempClicked { get; set; } = 0;
        public static float BoardHeight { get; set; }
        public static float BoardWidth { get; set; }
    }
}