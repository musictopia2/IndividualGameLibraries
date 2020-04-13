using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Linq;
using TileRummyCP.Data;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace TileRummyCP.Logic
{
    public class MainSets : MainSetsObservable<EnumColorType, EnumColorType, TileInfo, TileSet, SavedSet>
    {

        public MainSets(CommandContainer command) : base(command)
        {

        }

        public void RemoveSet(int index)
        {
            var thisSet = SetList[index];
            RemoveSet(thisSet);
        }
        public void RedoSets()
        {
            SetList.ForEach(thisTemp =>
            {
                thisTemp.HandList.ForEach(thisTile =>
                {
                    thisTile.IsUnknown = false; //to double check.
                    thisTile.Drew = false;
                    thisTile.IsSelected = false;
                });
            });
        }
        public bool PlayedAtLeastOneFromHand()
        {
            CustomBasicList<int> tempList = new CustomBasicList<int>();
            SetList.ForEach(thisTemp =>
            {
                thisTemp.HandList.ForEach(thisTile =>
                {
                    tempList.Add(thisTile.Deck);
                });
            });
            TileRummySaveInfo saveRoot = cons!.Resolve<TileRummySaveInfo>();
            foreach (var thisIndex in saveRoot.YourTiles)
            {
                if (tempList.Any(items => items == thisIndex))
                    return true;
            }
            return false;
        }

    }
}
