using BasicGameFrameworkLibrary.ChooserClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using GoFishCP.Data;
using System.Linq;

namespace GoFishCP.Logic
{
    public class GoFishChooserCP : SimpleEnumPickerVM<EnumCardValueList, NumberPieceCP>
    {
        public void LoadFromHandCardValues(GoFishPlayerItem thisPlayer) //its smart enough to take their hand part
        {
            var thisList = thisPlayer.MainHandList.GroupBy(items => items.Value).Select(Items => Items.Key).ToCustomBasicList();
            CustomBasicList<NumberPieceCP> TempList = new CustomBasicList<NumberPieceCP>();
            thisList.ForEach(items =>
            {
                IEnumPiece<EnumCardValueList> thisPiece = new NumberPieceCP();
                thisPiece.EnumValue = items;
                thisPiece.IsEnabled = IsEnabled;
                thisPiece.IsSelected = false;
                TempList.Add((NumberPieceCP)thisPiece);
            });
            ItemList.ReplaceRange(TempList);
        }
        public GoFishChooserCP(CommandContainer command) : base(command, new CardValueListChooser()) { }
    }
}