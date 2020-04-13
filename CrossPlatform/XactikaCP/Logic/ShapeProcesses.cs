using BasicGameFrameworkLibrary.Attributes;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks;
using XactikaCP.Data;

namespace XactikaCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class ShapeProcesses : IShapeProcesses
    {
        private readonly XactikaVMData _model;
        private readonly XactikaGameContainer _gameContainer;

        public ShapeProcesses(XactikaVMData model, XactikaGameContainer gameContainer)
        {
            _model = model;
            _gameContainer = gameContainer;
        }
        public async Task FirstCallShapeAsync()
        {
            _model.ShapeChoose1.Visible = true;
            if (_gameContainer.LoadShapeButtonAsync == null)
            {
                throw new BasicBlankException("Nobody is loading the shapes.  Rethink");
            }
            await _gameContainer.LoadShapeButtonAsync.Invoke(); //hopefully its this simple (?)
            var thisCard = _gameContainer.SaveRoot!.TrickList.Single();
            _model!.ShapeChoose1!.LoadPieceList(thisCard);
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }

        public async Task ShapeChosenAsync(EnumShapes shape)
        {
            var thisCard = _gameContainer.SaveRoot!.TrickList.Single();
            _gameContainer.SaveRoot.ShapeChosen = shape;
            _gameContainer.SaveRoot.Value = shape switch
            {
                EnumShapes.Balls => thisCard.HowManyBalls,
                EnumShapes.Cubes => thisCard.HowManyCubes,
                EnumShapes.Cones => thisCard.HowManyCones,
                EnumShapes.Stars => thisCard.HowManyStars,
                _ => throw new BasicBlankException("Don't know what to use"),
            };

            _gameContainer.SaveRoot.GameStatus = EnumStatusList.Normal;
            _model!.ShapeChoose1!.ChoosePiece(shape);
            if (_gameContainer.CloseShapeButtonAsync == null)
            {
                throw new BasicBlankException("Nobody is closing the shapes.  Rethink");
            }
            await _gameContainer.CloseShapeButtonAsync.Invoke();
            await _gameContainer.EndTurnAsync!.Invoke();
        }
    }
}