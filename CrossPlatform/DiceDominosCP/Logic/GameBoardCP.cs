using System;
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
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.DrawableListsViewModels;
using BasicGameFrameworkLibrary.Dominos;
using BasicGameFrameworkLibrary.CommonInterfaces;
using DiceDominosCP.Data;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.Extensions;
//i think this is the most common things i like to do
namespace DiceDominosCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class GameBoardCP : GameBoardObservable<SimpleDominoInfo>
    {
        private readonly DiceDominosGameContainer _gameContainer;
        private readonly DiceDominosVMData _model;

        public GameBoardCP(DiceDominosGameContainer gameContainer, DiceDominosVMData model) : base(gameContainer.Command)
        {
            Columns = 7;
            Rows = 4;
            Text = "Dominos";
            HasFrame = true;
            _gameContainer = gameContainer;
            _model = model;
        }
        private bool HasSix()
        {
            return _model.Cup!.DiceList.Any(items => items.Value == 6);
        }
        public int DiceValue(int index)
        {
            if (index == 1)
                return _model.Cup!.DiceList.First().Value;
            else if (index == 2)
                return _model.Cup!.DiceList.Last().Value;
            else
                throw new BasicBlankException($"Must be 1 or 2, not {index} to find the dice value");
        }
        public void MakeMove(int deck)
        {
            SimpleDominoInfo thisDomino = ObjectList.GetSpecificItem(deck);
            thisDomino.Visible = false;
        }
        public bool IsValidMove(int deck)
        {
            if (_gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Computer && _gameContainer.Test.AllowAnyMove == true)
                return true; //because we are allowing any move for testing.
            SimpleDominoInfo thisDomino = ObjectList.GetSpecificItem(deck);
            if (HasSix() == false)
            {
                // this means no 6
                if (thisDomino.FirstNum == DiceValue(1) && thisDomino.SecondNum == DiceValue(2))
                    return true;
                if (thisDomino.FirstNum == DiceValue(2) && thisDomino.SecondNum == DiceValue(1))
                    return true;
                return false;
            }
            if (thisDomino.FirstNum == DiceValue(1) && thisDomino.SecondNum == DiceValue(2))
                return true;
            if (thisDomino.FirstNum == DiceValue(2) && thisDomino.SecondNum == DiceValue(1))
                return true;
            if (DiceValue(1) == 6 && DiceValue(2) == 6 && (thisDomino.FirstNum == 0 || thisDomino.FirstNum == 6) && (thisDomino.SecondNum == 0 || thisDomino.SecondNum == 6))
                return true;
            if (DiceValue(1) == 6 && thisDomino.FirstNum == 0 && DiceValue(2) == thisDomino.SecondNum)
                return true;
            if (DiceValue(1) == 6 && thisDomino.SecondNum == 0 && DiceValue(2) == thisDomino.FirstNum)
                return true;
            if (DiceValue(2) == 6 && thisDomino.SecondNum == 0 && DiceValue(1) == thisDomino.FirstNum)
                return true;
            if (DiceValue(2) == 6 && thisDomino.FirstNum == 0 && DiceValue(1) == thisDomino.SecondNum)
                return true;
            return false;
        }
        protected override async Task ClickProcessAsync(SimpleDominoInfo payLoad)
        {
            if (_gameContainer.DominoClickedAsync == null)
            {
                throw new BasicBlankException("Nobody is handling the domino clicked.  Rethink");
            }
            await _gameContainer.DominoClickedAsync.Invoke(payLoad);
        }
        public void ClearPieces()
        {
            ObjectList.ReplaceRange(_gameContainer.DominosShuffler);
        }
        public DeckRegularDict<SimpleDominoInfo> GetVisibleList()
        {
            return ObjectList.Where(Items => Items.Visible).ToRegularDeckDict();
        }
        public async Task ShowDominoAsync(int deck)
        {
            SimpleDominoInfo thisDomino = ObjectList.GetSpecificItem(deck);
            thisDomino.IsUnknown = false;
            await _gameContainer.Delay.DelaySeconds(2);
            thisDomino.IsUnknown = true;
        }
    }
}
