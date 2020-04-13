using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.MiscProcesses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Linq;
using System.Reflection;
using XactikaCP.Cards;
using XactikaCP.Data;
using XactikaCP.MiscImages;

namespace XactikaCP.Logic
{
    public class ChooseShapeObservable : SimpleControlObservable
    {
        private readonly XactikaGameContainer _gameContainer;
        public ChooseShapeObservable(XactikaGameContainer gameContainer) : base(gameContainer.Command)
        {
            MethodInfo method = this.GetPrivateMethod(nameof(ProcessPieceSelected));
            ShapeSelectedCommand = new ControlCommand(this, method, gameContainer.Command);
            _gameContainer = gameContainer;
        }

        private EnumShapes _shapeChosen = EnumShapes.None;
        public EnumShapes ShapeChosen
        {
            get
            {
                return _shapeChosen;
            }

            set
            {
                if (SetProperty(ref _shapeChosen, value) == true)
                    // code to run

                    _gameContainer.SaveRoot!.ShapeChosen = value;
            }
        }


        private int _howMany = 0;

        public int HowMany
        {
            get
            {
                return _howMany;
            }

            set
            {
                if (SetProperty(ref _howMany, value) == true)
                {
                }
            }
        }

        public ControlCommand ShapeSelectedCommand { get; set; }
        public CustomBasicCollection<PieceCP> PieceList { get; set; } = new CustomBasicCollection<PieceCP>();
        private void ProcessPieceSelected(PieceCP piece)
        {
            PieceList.ForEach(tempPiece =>
            {
                if (tempPiece.Equals(piece))
                    tempPiece.IsSelected = true;
                else
                    tempPiece.IsSelected = false;
            });
            HowMany = piece.HowMany;
            ShapeChosen = piece.ShapeUsed;
        }



        protected override void EnableChange()
        {
            ShapeSelectedCommand.ReportCanExecuteChange();
            PieceList.ForEach(piece => piece.IsEnabled = IsEnabled); //try this.
        }

        protected override void PrivateEnableAlways() { }

        //not sure if we need visible or not (?)
        //we might
        private bool _visible;

        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (SetProperty(ref _visible, value))
                {
                    //can decide what to do when property changes
                }

            }
        }


        public void ChoosePiece(EnumShapes shape)
        {
            if (PieceList.Count > 0)
            {
                var piece = (from x in PieceList
                             where (int)x.ShapeUsed == (int)shape
                             select x).Single();
                piece.IsSelected = false; // its obvious now.
                PieceList.ReplaceAllWithGivenItem(piece); // this will become the new list
            }
            else
            {
                PieceCP newPiece = new PieceCP();
                newPiece.ShapeUsed = shape;
                newPiece.HowMany = _gameContainer.SaveRoot!.Value;
                PieceList.ReplaceAllWithGivenItem(newPiece);
            }
            Visible = true; // obviously
        }
        public void Reset()
        {
            ShapeChosen = EnumShapes.None;
            HowMany = 0;
            _gameContainer.SaveRoot!.ShapeChosen = EnumShapes.None;
            Visible = false; // i think
        }
        public void LoadPieceList(XactikaCardInformation card) // has to be loaded with the card chosen
        {
            CustomBasicCollection<PieceCP> tempList = new CustomBasicCollection<PieceCP>();
            PieceCP thisPiece = new PieceCP();
            thisPiece.HowMany = card.HowManyBalls;
            thisPiece.ShapeUsed = EnumShapes.Balls;
            tempList.Add(thisPiece);
            thisPiece = new PieceCP();
            thisPiece.HowMany = card.HowManyCones;
            thisPiece.ShapeUsed = EnumShapes.Cones;
            tempList.Add(thisPiece);
            thisPiece = new PieceCP();
            thisPiece.HowMany = card.HowManyCubes;
            thisPiece.ShapeUsed = EnumShapes.Cubes;
            tempList.Add(thisPiece);
            thisPiece = new PieceCP();
            thisPiece.HowMany = card.HowManyStars;
            thisPiece.ShapeUsed = EnumShapes.Stars;
            tempList.Add(thisPiece);
            PieceList.ReplaceRange(tempList);
            Visible = true;
        }

    }
}
