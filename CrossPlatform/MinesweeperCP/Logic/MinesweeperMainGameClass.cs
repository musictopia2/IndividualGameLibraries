using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using MinesweeperCP.Data;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace MinesweeperCP.Logic
{
    [SingletonGame]
    public class MinesweeperMainGameClass : IAggregatorContainer
    {
        private readonly ISaveSinglePlayerClass _thisState;
        private readonly RandomGenerator _rs;


        public int NumberOfColumns = 9;
        public int NumberOfRows = 9;
        public int NumberOfMines = 10;

        private CustomBasicList<MineSquareCP> _arr_Squares = new CustomBasicList<MineSquareCP>();

        public CustomBasicList<MineSquareCP> GetSquares()
        {
            return _arr_Squares;
        }

        //internal MinesweeperSaveInfo _saveRoot;
        public MinesweeperMainGameClass(ISaveSinglePlayerClass thisState,
            IEventAggregator aggregator,
            RandomGenerator rs
            //IGamePackageResolver container
            )
        {
            _thisState = thisState;
            Aggregator = aggregator;
            _rs = rs;
            //_saveRoot = container.ReplaceObject<MinesweeperSaveInfo>(); //can't create new one.  because if doing that, then anything that needs it won't have it.
        }


        public IEventAggregator Aggregator { get; }

        public async Task NewGameAsync()
        {
            CreateSquares();
            await Aggregator.SendLoadAsync();
            await ProcessGameStateAsync(EnumGameStates.NotFinished);
            //_arr_Squares.ForEach(x => x.IsNotifying = true);
        }
        //private async Task RestoreGameAsync()
        //{
        //    _saveRoot = await _thisState.RetrieveSinglePlayerGameAsync<MinesweeperSaveInfo>();

        //}
        public async Task ShowWinAsync()
        {
            await UIPlatform.ShowMessageAsync("You Win");
            //ThisMod.NewGameVisible = true;
            await _thisState.DeleteSinglePlayerGameAsync();
            //send message to show win.
            await this.SendGameOverAsync();

        }


        private async Task ProcessGameStateAsync(EnumGameStates state)
        {
            if (state == EnumGameStates.NotFinished)
                return;
            if (state == EnumGameStates.Won)
            {
                await this.MessageGameOverAsync("Congratulations, you won the game!");
                return;
            }
            if (state == EnumGameStates.Lost)
            {
                await this.MessageGameOverAsync("Sorry, you lost the game!");
                return;
            }
            throw new BasicBlankException("Rethink");
        }

        public void FlagSingleSquare(MineSquareCP thisSquare)
        {

            if (thisSquare.IsFlipped == true)
            {
                return;
            }
            thisSquare.Flagged = !thisSquare.Flagged;
            //ThisMod.NumberOfMinesLeft = GetMinesLeft();
        }

        public async Task ClickSingleSquareAsync(MineSquareCP thisSquare)
        {
            if (thisSquare.IsFlipped == true || thisSquare.Flagged == true)
            {
                return;
            }
            // if flaged, you have to unflag first.
            // if flipped, too late
            if (thisSquare.IsMine == true)
            {
                BlowUp();
                await ProcessGameStateAsync(EnumGameStates.Lost);
                return; // you lost now.
            }
            thisSquare.IsFlipped = true;
            if (thisSquare.NeighborMines == 0)
                FlipAllNeighbors(thisSquare);
            if (NumberOfMines == CountUnflippedSquares())
            {
                await ProcessGameStateAsync(EnumGameStates.Won);
                return;
            }
        }

        public int GetMinesLeft()
        {
            int int_Return = NumberOfMines;
            foreach (MineSquareCP obj_Square in _arr_Squares)
            {
                if ((obj_Square.Flagged))
                    int_Return -= 1;
            }
            return int_Return;
        }

        private int CountUnflippedSquares()
        {
            int int_Return = 0;
            foreach (MineSquareCP obj_Square in _arr_Squares)
            {
                if ((!obj_Square.IsFlipped))
                    int_Return += 1;
            }
            return int_Return;
        }

        private int GetRandomInteger(int int_Span)
        {
            int int_RandomInteger;
            if (int_Span <= 0)
                int_RandomInteger = 0;
            else
                int_RandomInteger = _rs.GetRandomNumber(int_Span, 0); //okay to make it 0 based.
            return int_RandomInteger;
        }

        private MineSquareCP GetSquare(int column, int row)
        {
            return (from Items in _arr_Squares
                    where Items.Column == column && Items.Row == row
                    select Items).SingleOrDefault();
        }

        private bool SquareHasMine(int column, int row)
        {
            var obj_CheckSquare = GetSquare(column, row);
            if (!(obj_CheckSquare == null))
            {
                if (obj_CheckSquare.IsMine)
                    return true;
            }
            return false;
        }

        private void BlowUp()
        {
            foreach (MineSquareCP obj_Square in _arr_Squares)
            {
                if ((obj_Square.IsMine))
                    obj_Square.IsFlipped = true;
                else if ((!obj_Square.IsMine) & (obj_Square.Flagged))
                    obj_Square.IsFlipped = true;
            }
        }

        private void FlipAllNeighbors(MineSquareCP obj_TempSquare)
        {
            MineSquareCP obj_Flip;
            for (var int_X = -1; int_X <= 1; int_X++)
            {
                for (var int_Y = -1; int_Y <= 1; int_Y++)
                {
                    obj_Flip = GetSquare(obj_TempSquare.Column + int_X, obj_TempSquare.Row + int_Y);
                    if (!(obj_Flip == null))
                    {
                        if ((!obj_Flip.Flagged) & (!obj_Flip.IsFlipped))
                        {
                            obj_Flip.IsFlipped = true;

                            if (obj_Flip.NeighborMines == 0)
                                FlipAllNeighbors(obj_Flip);
                        }
                    }
                }
            }
        }

        private int CountNeighborMines(MineSquareCP obj_TempSquare)
        {
            int int_Count = 0;

            for (var int_X = -1; int_X <= 1; int_X++)
            {
                for (var int_Y = -1; int_Y <= 1; int_Y++)
                {
                    if ((SquareHasMine(obj_TempSquare.Column + int_X, obj_TempSquare.Row + int_Y)))
                        int_Count += 1;
                }
            }

            return int_Count;
        }

        private void CreateSquares()
        {
            CustomBasicList<int> arr_Mines = new CustomBasicList<int>();
            int int_Square;
            int int_SquareCount = 1;
            _arr_Squares = new CustomBasicList<MineSquareCP>();
            while (arr_Mines.Count < NumberOfMines)
            {
                int_Square = GetRandomInteger((NumberOfRows * NumberOfColumns));
                if ((!(arr_Mines.Contains(int_Square))) & (int_Square > 0))
                    arr_Mines.Add(int_Square);
            }

            var loopTo = NumberOfRows;
            for (int int_Row = 1; int_Row <= loopTo; int_Row++)
            {
                var loopTo1 = NumberOfColumns;
                for (int int_Col = 1; int_Col <= loopTo1; int_Col++)
                {
                    var obj_TempSquare = new MineSquareCP(int_Col, int_Row);

                    if ((arr_Mines.Contains(int_SquareCount)))
                        obj_TempSquare.IsMine = true;

                    _arr_Squares.Add(obj_TempSquare);

                    int_SquareCount += 1;
                }
            }

            // *** Get numbers for each of the non-mine squares
            foreach (var obj_TempSquare in _arr_Squares)
                obj_TempSquare.NeighborMines = CountNeighborMines(obj_TempSquare);


            // needs a subscription
            // because from this point, the ui needs to do the ui portions.
            //Aggregator.Publish(new SubscribeGameBoardEventModel());
        }
        //public void DrawBoard(SKCanvas thisCanvas, float width, float height)
        //{
        //    var bounds = SKRect.Create(0, 0, width, height);
        //    var thisPaint = MiscHelpers.GetLinearGradientPaint(SKColors.White, SKColors.LightGray, bounds, MiscHelpers.EnumLinearGradientPercent.Angle90);
        //    thisCanvas.DrawRect(bounds, thisPaint);
        //}

    }
}
