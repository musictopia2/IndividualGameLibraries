using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.DIContainers;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.CollectionClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TileRummyCP.Data;

namespace TileRummyCP.Logic
{
    public class TileShuffler : IDeckShuffler<TileInfo>, IScatterList<TileInfo>,
         IAdvancedDIContainer, ISimpleList<TileInfo>, IListShuffler<TileInfo>
    {
        private readonly BasicObjectShuffler<TileInfo> _thisShuffle;
        private readonly DeckRegularDict<TileInfo> _objectList = new DeckRegularDict<TileInfo>();
        public int Count => _objectList.Count;
        public bool NeedsToRedo { get => _thisShuffle.NeedsToRedo; set => _thisShuffle.NeedsToRedo = value; }
        public IGamePackageResolver? MainContainer { get => _thisShuffle.MainContainer; set => _thisShuffle.MainContainer = value; }
        public TileShuffler()
        {
            _thisShuffle = new BasicObjectShuffler<TileInfo>(_objectList); //i think.
            NeedsToRedo = true; //for this one, must be redo.
        }
        public Task<DeckObservableDict<TileInfo>> GetListFromJsonAsync(string jsonData)
        {
            return _thisShuffle.GetListFromJsonAsync(jsonData);
        }
        public void ClearObjects()
        {
            _thisShuffle.ClearObjects();
        }
        public void OrderedObjects()
        {
            _thisShuffle.OrderedObjects();
        }
        public void ShuffleObjects()
        {
            _thisShuffle.ShuffleObjects();
        }
        public void ReshuffleFirstObjects(IDeckDict<TileInfo> thisList, int startAt, int endAt)
        {
            _thisShuffle.ReshuffleFirstObjects(thisList, startAt, endAt);
        }
        public TileInfo GetSpecificItem(int deck)
        {
            return _thisShuffle.GetSpecificItem(deck);
        }
        public bool ObjectExist(int deck)
        {
            return _thisShuffle.ObjectExist(deck);
        }
        public int GetDeckCount()
        {
            return _thisShuffle.GetDeckCount();
        }
        public Task ForEachAsync(BasicDataFunctions.ActionAsync<TileInfo> action)
        {
            return _objectList.ForEachAsync(action);
        }
        public void ForEach(Action<TileInfo> action)
        {
            _objectList.ForEach(action);
        }
        public void ForConditionalItems(Predicate<TileInfo> match, Action<TileInfo> action)
        {
            _objectList.ForConditionalItems(match, action);
        }
        public Task ForConditionalItemsAsync(Predicate<TileInfo> match, BasicDataFunctions.ActionAsync<TileInfo> action)
        {
            return _objectList.ForConditionalItemsAsync(match, action);
        }
        public bool Exists(Predicate<TileInfo> match)
        {
            return _objectList.Exists(match);
        }
        public bool Contains(TileInfo item)
        {
            return _objectList.Contains(item);
        }
        public TileInfo Find(Predicate<TileInfo> match)
        {
            return _objectList.Find(match);
        }
        public TileInfo FindOnlyOne(Predicate<TileInfo> match)
        {
            return _objectList.FindOnlyOne(match);
        }
        public ICustomBasicList<TileInfo> FindAll(Predicate<TileInfo> match)
        {
            return _objectList.FindAll(match);
        }
        public int FindFirstIndex(Predicate<TileInfo> match)
        {
            return _objectList.FindFirstIndex(match);
        }
        public int FindFirstIndex(int startIndex, Predicate<TileInfo> match)
        {
            return _objectList.FindFirstIndex(startIndex, match);
        }
        public int FindFirstIndex(int startIndex, int count, Predicate<TileInfo> match)
        {
            return _objectList.FindFirstIndex(startIndex, count, match);
        }
        public TileInfo FindLast(Predicate<TileInfo> match)
        {
            return _objectList.FindLast(match);
        }
        public int FindLastIndex(Predicate<TileInfo> match)
        {
            return _objectList.FindLastIndex(match);
        }
        public int FindLastIndex(int startIndex, Predicate<TileInfo> match)
        {
            return _objectList.FindLastIndex(startIndex, match);
        }
        public int FindLastIndex(int startIndex, int count, Predicate<TileInfo> match)
        {
            return _objectList.FindLastIndex(startIndex, count, match);
        }
        public int HowMany(Predicate<TileInfo> match)
        {
            return _objectList.HowMany(match);
        }
        public int IndexOf(TileInfo value)
        {
            return _objectList.IndexOf(value);
        }
        public int IndexOf(TileInfo value, int Index)
        {
            return _objectList.IndexOf(value, Index);
        }
        public int IndexOf(TileInfo value, int Index, int Count)
        {
            return _objectList.IndexOf(value, Index, Count);
        }
        public bool TrueForAll(Predicate<TileInfo> match)
        {
            return _objectList.TrueForAll(match);
        }
        public IEnumerator<TileInfo> GetEnumerator()
        {
            return _objectList.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _objectList.GetEnumerator();
        }

        public void RelinkObject(int oldDeck, TileInfo newObject)
        {
            _thisShuffle.RelinkObject(oldDeck, newObject);
        }

        public void PutCardOnTop(int deck)
        {
            _thisShuffle.PutCardOnTop(deck);
        }
    }
}
