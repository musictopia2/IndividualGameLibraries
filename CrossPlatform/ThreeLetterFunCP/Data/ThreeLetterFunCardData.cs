﻿using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.DIContainers;
using Newtonsoft.Json;
using ThreeLetterFunCP.Logic;
using vb = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.VBCompat;
using static BasicGameFrameworkLibrary.DIContainers.Helpers;
using SkiaSharp;

namespace ThreeLetterFunCP.Data
{
    public class ThreeLetterFunCardData : SimpleDeckObject, IDeckObject, IAdvancedDIContainer
    {
        //this needs something else to populate.

        [JsonIgnore]
        public IGamePackageResolver? MainContainer { get; set; }

        private ThreeLetterFunDeckInfo? _deckInfo;
        private GlobalHelpers? _thisGlobal;
        private ThreeLetterFunMainGameClass? _mainGame; //iffy.
        private CustomBasicList<char> _completeList = new CustomBasicList<char>();
        private CustomBasicList<char> _charList = new CustomBasicList<char>();

        public CustomBasicList<char> CharList
        {
            get { return _charList; }
            set
            {
                if (SetProperty(ref _charList, value))
                {
                    //can decide what to do when property changes
                    _completeList = CharList.ToCustomBasicList();
                }
            }
        }

        public CustomBasicList<char> GetCompleteList => _completeList.ToCustomBasicList();
        public EnumClickPosition ClickLocation { get; set; } = EnumClickPosition.None;

        public void ReloadSaved() //hopefully this simple now.
        {
            _completeList = CharList.ToCustomBasicList();
        }
        public bool CompletedWord()
        {
            if (_completeList.Exists(x => vb.AscW(x) == 0) == true)
                return false;
            return true;
        }
        private void SetObjects()
        {
            if (_deckInfo != null)
                return;
            PopulateContainer(this);
            _deckInfo = MainContainer!.Resolve<ThreeLetterFunDeckInfo>(); //need the full one.
            _thisGlobal = MainContainer.Resolve<GlobalHelpers>();
            _mainGame = MainContainer.Resolve<ThreeLetterFunMainGameClass>();
        }
        public ThreeLetterFunCardData CloneCard()
        {
            ThreeLetterFunCardData output = (ThreeLetterFunCardData)MemberwiseClone();
            output._completeList = _completeList.ToCustomBasicList();
            output.CharList = CharList.ToCustomBasicList(); //i think.
            return output;
        }
        private CustomBasicList<TileInformation> _tiles = new CustomBasicList<TileInformation>();

        public CustomBasicList<TileInformation> Tiles
        {
            get { return _tiles; }
            set
            {
                if (SetProperty(ref _tiles, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _hiddenValue;

        public int HiddenValue
        {
            get { return _hiddenValue; }
            set
            {
                if (SetProperty(ref _hiddenValue, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        public void ClearTiles()
        {
            _completeList = CharList.ToCustomBasicList();
            Tiles.Clear();
            HiddenValue += 1; // so it can repaint (?)
        }
        public string GetWord()
        {
            if (CompletedWord() == false)
                throw new BasicBlankException("Has to complete a word before it can get the word");
            return _completeList.First().ToString().ToLower() + _completeList[1].ToString().ToLower() + _completeList.Last().ToString().ToLower();
        }

        public int GetLetterPosition(EnumClickPosition whichCategory)
        {
            if (whichCategory == EnumClickPosition.None)
                throw new BasicBlankException("Must specify left or right");
            int x = default;
            int y = default;
            foreach (var thisItem in _completeList)
            {
                if (vb.AscW(thisItem) == 0)
                {
                    if (whichCategory == EnumClickPosition.Left)
                        return x;
                    else
                        y += 1;
                    if (y > 1)
                        return x;
                }
                x += 1;
            }
            return -1; //hopefully this is right (not sure)
        }
        public int LetterRemaining()
        {
            var thisItem = (from items in _completeList
                            where vb.AscW(items) == 0
                            select items).Single();
            return _completeList.IndexOf(thisItem);
        }

        public void AddLetter(int tileDeck, int index)
        {
            if (_completeList.Count == 0)
                throw new BasicBlankException("No Complete List");
            if (vb.AscW(_completeList[index]) > 0)
            {
                var thisChar = _completeList[index];
                Tiles.RemoveAllOnly(x => x.Letter == thisChar);
            }
            if (_thisGlobal == null)
                SetObjects();
            var thisTile = _thisGlobal!.GetTile(tileDeck);
            thisTile.Index = index; // i think
            Tiles.Add(thisTile);
            _completeList[index] = thisTile.Letter;
        }
        public bool IsValidWord()
        {
            if (_mainGame!.SaveRoot!.Level == EnumLevel.None)
                throw new BasicBlankException("Needs to choose a level before figure out whether its a valid word or not");
            var thisWord = GetWord();
            if (_mainGame.SaveRoot.Level == EnumLevel.Hard)
                return _thisGlobal!.SavedWords.Any(Items => Items.Word == thisWord);
            return _thisGlobal!.SavedWords.Any(Items => Items.Word == thisWord && Items.IsEasy == true);
        }
        public ThreeLetterFunCardData()
        {
            DefaultSize = new SKSize(73, 36);
        }
        public void Populate(int chosen)
        {
            if (chosen == 0)
                throw new BasicBlankException("Deck cannot be 0 for this game");
            SetObjects();
            if (_deckInfo!.PrivateSavedList.Count == 0)
                _deckInfo.InitCards();
            Deck = chosen;
            if (Deck > _deckInfo.PrivateSavedList.Count)
                throw new BasicBlankException($"{Deck} not found");
            CharList = _deckInfo.PrivateSavedList[Deck - 1].CharacterList.ToCustomBasicList();
        }
        public void Reset() { }
    }
}
