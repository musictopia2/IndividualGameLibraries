using BasicGameFramework.CommandClasses;
using BasicGameFramework.MiscViewModels;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Linq;
namespace XactikaCP
{
    public class ChooseShapeViewModel : SimpleControlViewModel
    {
        private EnumShapes _ShapeChosen = EnumShapes.None;
        public EnumShapes ShapeChosen
        {
            get
            {
                return _ShapeChosen;
            }

            set
            {
                if (SetProperty(ref _ShapeChosen, value) == true)
                    // code to run

                    _mainGame.SaveRoot!.ShapeChosen = value;
            }
        }


        private int _HowMany = 0;
        public int HowMany
        {
            get
            {
                return _HowMany;
            }

            set
            {
                if (SetProperty(ref _HowMany, value) == true)
                {
                }
            }
        }

        public ControlCommand<PieceCP> ShapeSelectedCommand { get; set; }
        private readonly XactikaMainGameClass _mainGame;
        public ChooseShapeViewModel(IBasicGameVM thisMod) : base(thisMod)
        {
            _mainGame = thisMod!.MainContainer!.Resolve<XactikaMainGameClass>();
            ShapeSelectedCommand = new ControlCommand<PieceCP>(this, thisPiece =>
            {
                PieceList.ForEach(tempPiece =>
                {
                    if (tempPiece.Equals(thisPiece))
                        tempPiece.IsSelected = true;
                    else
                        tempPiece.IsSelected = false;
                });
                HowMany = thisPiece.HowMany;
                ShapeChosen = thisPiece.ShapeUsed;
            }, thisMod, thisMod.CommandContainer!);
        }
        public CustomBasicCollection<PieceCP> PieceList { get; set; } = new CustomBasicCollection<PieceCP>();
        protected override void EnableChange()
        {
            ShapeSelectedCommand.ReportCanExecuteChange();
            PieceList.ForEach(thisPiece => thisPiece.IsEnabled = IsEnabled); //try this.
        }
        protected override void PrivateEnableAlways() { }
        protected override void VisibleChange() { }
        public void ChoosePiece(EnumShapes shape)
        {
            if (PieceList.Count > 0)
            {
                var ThisPiece = (from Items in PieceList
                                 where (int)Items.ShapeUsed == (int)shape
                                 select Items).Single();
                ThisPiece.IsSelected = false; // its obvious now.
                PieceList.ReplaceAllWithGivenItem(ThisPiece); // this will become the new list
            }
            else
            {
                PieceCP newPiece = new PieceCP();
                newPiece.ShapeUsed = shape;
                newPiece.HowMany = _mainGame.SaveRoot!.Value;
                PieceList.ReplaceAllWithGivenItem(newPiece);
            }
            Visible = true; // obviously
        }
        public void Reset()
        {
            ShapeChosen = EnumShapes.None;
            HowMany = 0;
            _mainGame.SaveRoot!.ShapeChosen = EnumShapes.None;
            Visible = false; // i think
        }
        public void LoadPieceList(XactikaCardInformation thisCard) // has to be loaded with the card chosen
        {
            CustomBasicCollection<PieceCP> tempList = new CustomBasicCollection<PieceCP>();
            PieceCP thisPiece = new PieceCP();
            thisPiece.HowMany = thisCard.HowManyBalls;
            thisPiece.ShapeUsed = EnumShapes.Balls;
            tempList.Add(thisPiece);
            thisPiece = new PieceCP();
            thisPiece.HowMany = thisCard.HowManyCones;
            thisPiece.ShapeUsed = EnumShapes.Cones;
            tempList.Add(thisPiece);
            thisPiece = new PieceCP();
            thisPiece.HowMany = thisCard.HowManyCubes;
            thisPiece.ShapeUsed = EnumShapes.Cubes;
            tempList.Add(thisPiece);
            thisPiece = new PieceCP();
            thisPiece.HowMany = thisCard.HowManyStars;
            thisPiece.ShapeUsed = EnumShapes.Stars;
            tempList.Add(thisPiece);
            PieceList.ReplaceRange(tempList);
            Visible = true;
        }
    }
}