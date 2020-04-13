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
using BasicGameFrameworkLibrary.GameGraphicsCP.BasicGameBoards;
using SkiaSharp;
using System.Reflection;
using CandylandCP.Data;
using SkiaSharpGeneralLibrary.SKExtensions;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
using CandylandCP.Logic;
using BasicGameFrameworkLibrary.DIContainers;
using CandylandCP.ViewModels;
using BasicGameFrameworkLibrary.CommandClasses;

namespace CandylandCP.GraphicsCP
{
    [SingletonGame]
    [AutoReset]
    public class CandylandBoardGraphicsCP : BaseGameBoardCP<CandylandPieceGraphicsCP>
    {
        //desktop is 1.3  we should have that in the desktop project.  not sure about xamarin forms.
        private readonly CandylandMainGameClass _mainGame;
        private readonly GameBoardVM _model;
        private readonly CommandContainer _command;

        //private readonly CandylandViewModel _thisMod;
        private readonly CandylandBoardProcesses _gameBoard1;
        public CandylandBoardGraphicsCP(CandylandBoardProcesses gameBoard1,
            IGamePackageResolver resolver,
            CandylandMainGameClass mainGame,
            GameBoardVM model,
            CommandContainer command
            ) : base(resolver)
        {
            _mainGame = mainGame;
            _model = model;
            _command = command;
            //_thisMod = MainContainer.Resolve<CandylandViewModel>();
            _gameBoard1 = gameBoard1; //looks like no overflow error this time.
            DrawBoardEarly = true;
        }
        public override string TagUsed => "main"; //its okay because candyland has nothing else.
        protected override SKSize OriginalSize { get; set; } = new SKSize(807, 675);
        private CustomBasicList<SKPath> _arr_Paths = new CustomBasicList<SKPath>();
        private struct TempSpace
        {
            public int Number;
            public SKRect Bounds;
        }
        private CustomBasicList<TempSpace> _arr_Spaces = new CustomBasicList<TempSpace>();
        private CustomBasicList<SKPoint> _positionList = new CustomBasicList<SKPoint>();
        protected override bool CanStartPaint()
        {
            return true;
        }
        protected override void CreateSpaces()
        {
            var bounds = GetBounds();
            SKPoint[] pts = new SKPoint[4];
            _arr_Paths = new CustomBasicList<SKPath>();
            SKPath gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.180156657963446f, bounds.Height * 0.869718309859155f);
            pts[1] = new SKPoint(bounds.Width * 0.18668407310705f, bounds.Height * 0.947183098591549f);
            pts[2] = new SKPoint(bounds.Width * 0.112271540469974f, bounds.Height * 0.947183098591549f);
            pts[3] = new SKPoint(bounds.Width * 0.121409921671018f, bounds.Height * 0.830985915492958f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.20757180156658f, bounds.Height * 0.859154929577465f);
            pts[1] = new SKPoint(bounds.Width * 0.245430809399478f, bounds.Height * 0.940140845070423f);
            pts[2] = new SKPoint(bounds.Width * 0.159268929503916f, bounds.Height * 0.945422535211268f);
            pts[3] = new SKPoint(bounds.Width * 0.159268929503916f, bounds.Height * 0.86443661971831f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.22845953002611f, bounds.Height * 0.852112676056338f);
            pts[1] = new SKPoint(bounds.Width * 0.272845953002611f, bounds.Height * 0.90669014084507f);
            pts[2] = new SKPoint(bounds.Width * 0.220626631853786f, bounds.Height * 0.945422535211268f);
            pts[3] = new SKPoint(bounds.Width * 0.18668407310705f, bounds.Height * 0.873239436619718f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.245430809399478f, bounds.Height * 0.818661971830986f);
            pts[1] = new SKPoint(bounds.Width * 0.302872062663185f, bounds.Height * 0.869718309859155f);
            pts[2] = new SKPoint(bounds.Width * 0.253263707571802f, bounds.Height * 0.929577464788732f);
            pts[3] = new SKPoint(bounds.Width * 0.202349869451697f, bounds.Height * 0.857394366197183f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.280678851174935f, bounds.Height * 0.794014084507042f);
            pts[1] = new SKPoint(bounds.Width * 0.331592689295039f, bounds.Height * 0.852112676056338f);
            pts[2] = new SKPoint(bounds.Width * 0.284595300261097f, bounds.Height * 0.90669014084507f);
            pts[3] = new SKPoint(bounds.Width * 0.220626631853786f, bounds.Height * 0.832746478873239f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.31331592689295f, bounds.Height * 0.765845070422535f);
            pts[1] = new SKPoint(bounds.Width * 0.356396866840731f, bounds.Height * 0.839788732394366f);
            pts[2] = new SKPoint(bounds.Width * 0.314621409921671f, bounds.Height * 0.873239436619718f);
            pts[3] = new SKPoint(bounds.Width * 0.253263707571802f, bounds.Height * 0.816901408450704f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.369451697127937f, bounds.Height * 0.751760563380282f);
            pts[1] = new SKPoint(bounds.Width * 0.37597911227154f, bounds.Height * 0.841549295774648f);
            pts[2] = new SKPoint(bounds.Width * 0.321148825065274f, bounds.Height * 0.869718309859155f);
            pts[3] = new SKPoint(bounds.Width * 0.297650130548303f, bounds.Height * 0.785211267605634f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.424281984334204f, bounds.Height * 0.76056338028169f);
            pts[1] = new SKPoint(bounds.Width * 0.412532637075718f, bounds.Height * 0.850352112676056f);
            pts[2] = new SKPoint(bounds.Width * 0.344647519582245f, bounds.Height * 0.852112676056338f);
            pts[3] = new SKPoint(bounds.Width * 0.325065274151436f, bounds.Height * 0.764084507042254f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.477806788511749f, bounds.Height * 0.778169014084507f);
            pts[1] = new SKPoint(bounds.Width * 0.438642297650131f, bounds.Height * 0.859154929577465f);
            pts[2] = new SKPoint(bounds.Width * 0.391644908616188f, bounds.Height * 0.836267605633803f);
            pts[3] = new SKPoint(bounds.Width * 0.404699738903394f, bounds.Height * 0.751760563380282f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.513054830287206f, bounds.Height * 0.804577464788732f);
            pts[1] = new SKPoint(bounds.Width * 0.468668407310705f, bounds.Height * 0.880281690140845f);
            pts[2] = new SKPoint(bounds.Width * 0.417754569190601f, bounds.Height * 0.836267605633803f);
            pts[3] = new SKPoint(bounds.Width * 0.456919060052219f, bounds.Height * 0.765845070422535f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.545691906005222f, bounds.Height * 0.834507042253521f);
            pts[1] = new SKPoint(bounds.Width * 0.486945169712794f, bounds.Height * 0.90669014084507f);
            pts[2] = new SKPoint(bounds.Width * 0.446475195822454f, bounds.Height * 0.867957746478873f);
            pts[3] = new SKPoint(bounds.Width * 0.506527415143603f, bounds.Height * 0.790492957746479f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.570496083550914f, bounds.Height * 0.866197183098592f);
            pts[1] = new SKPoint(bounds.Width * 0.518276762402089f, bounds.Height * 0.938380281690141f);
            pts[2] = new SKPoint(bounds.Width * 0.469973890339426f, bounds.Height * 0.890845070422535f);
            pts[3] = new SKPoint(bounds.Width * 0.519582245430809f, bounds.Height * 0.809859154929578f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.599216710182768f, bounds.Height * 0.882042253521127f);
            pts[1] = new SKPoint(bounds.Width * 0.565274151436031f, bounds.Height * 0.959507042253521f);
            pts[2] = new SKPoint(bounds.Width * 0.503916449086162f, bounds.Height * 0.919014084507042f);
            pts[3] = new SKPoint(bounds.Width * 0.548302872062663f, bounds.Height * 0.84330985915493f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.634464751958224f, bounds.Height * 0.88556338028169f);
            pts[1] = new SKPoint(bounds.Width * 0.629242819843342f, bounds.Height * 0.975352112676056f);
            pts[2] = new SKPoint(bounds.Width * 0.546997389033943f, bounds.Height * 0.961267605633803f);
            pts[3] = new SKPoint(bounds.Width * 0.554830287206266f, bounds.Height * 0.846830985915493f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.650130548302872f, bounds.Height * 0.889084507042254f);
            pts[1] = new SKPoint(bounds.Width * 0.68798955613577f, bounds.Height * 0.959507042253521f);
            pts[2] = new SKPoint(bounds.Width * 0.60443864229765f, bounds.Height * 0.96830985915493f);
            pts[3] = new SKPoint(bounds.Width * 0.60443864229765f, bounds.Height * 0.88556338028169f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.660574412532637f, bounds.Height * 0.880281690140845f);
            pts[1] = new SKPoint(bounds.Width * 0.734986945169713f, bounds.Height * 0.890845070422535f);
            pts[2] = new SKPoint(bounds.Width * 0.682767624020888f, bounds.Height * 0.984154929577465f);
            pts[3] = new SKPoint(bounds.Width * 0.618798955613577f, bounds.Height * 0.899647887323944f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.650130548302872f, bounds.Height * 0.867957746478873f);
            pts[1] = new SKPoint(bounds.Width * 0.715404699738903f, bounds.Height * 0.823943661971831f);
            pts[2] = new SKPoint(bounds.Width * 0.740208877284595f, bounds.Height * 0.924295774647887f);
            pts[3] = new SKPoint(bounds.Width * 0.643603133159269f, bounds.Height * 0.911971830985916f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.634464751958224f, bounds.Height * 0.852112676056338f);
            pts[1] = new SKPoint(bounds.Width * 0.672323759791123f, bounds.Height * 0.790492957746479f);
            pts[2] = new SKPoint(bounds.Width * 0.744125326370757f, bounds.Height * 0.823943661971831f);
            pts[3] = new SKPoint(bounds.Width * 0.668407310704961f, bounds.Height * 0.89612676056338f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.590078328981723f, bounds.Height * 0.823943661971831f);
            pts[1] = new SKPoint(bounds.Width * 0.651436031331593f, bounds.Height * 0.767605633802817f);
            pts[2] = new SKPoint(bounds.Width * 0.699738903394256f, bounds.Height * 0.809859154929578f);
            pts[3] = new SKPoint(bounds.Width * 0.640992167101828f, bounds.Height * 0.876760563380282f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.573107049608355f, bounds.Height * 0.746478873239437f);
            pts[1] = new SKPoint(bounds.Width * 0.64490861618799f, bounds.Height * 0.753521126760563f);
            pts[2] = new SKPoint(bounds.Width * 0.664490861618799f, bounds.Height * 0.804577464788732f);
            pts[3] = new SKPoint(bounds.Width * 0.590078328981723f, bounds.Height * 0.848591549295775f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.614882506527415f, bounds.Height * 0.677816901408451f);
            pts[1] = new SKPoint(bounds.Width * 0.654046997389034f, bounds.Height * 0.753521126760563f);
            pts[2] = new SKPoint(bounds.Width * 0.634464751958224f, bounds.Height * 0.774647887323944f);
            pts[3] = new SKPoint(bounds.Width * 0.563968668407311f, bounds.Height * 0.757042253521127f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.676240208877285f, bounds.Height * 0.672535211267606f);
            pts[1] = new SKPoint(bounds.Width * 0.678851174934726f, bounds.Height * 0.744718309859155f);
            pts[2] = new SKPoint(bounds.Width * 0.621409921671018f, bounds.Height * 0.755281690140845f);
            pts[3] = new SKPoint(bounds.Width * 0.60443864229765f, bounds.Height * 0.688380281690141f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.727154046997389f, bounds.Height * 0.698943661971831f);
            pts[1] = new SKPoint(bounds.Width * 0.703655352480418f, bounds.Height * 0.762323943661972f);
            pts[2] = new SKPoint(bounds.Width * 0.652741514360313f, bounds.Height * 0.73943661971831f);
            pts[3] = new SKPoint(bounds.Width * 0.663185378590078f, bounds.Height * 0.677816901408451f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.759791122715405f, bounds.Height * 0.721830985915493f);
            pts[1] = new SKPoint(bounds.Width * 0.737597911227154f, bounds.Height * 0.799295774647887f);
            pts[2] = new SKPoint(bounds.Width * 0.678851174934726f, bounds.Height * 0.762323943661972f);
            pts[3] = new SKPoint(bounds.Width * 0.702349869451697f, bounds.Height * 0.686619718309859f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.787206266318538f, bounds.Height * 0.737676056338028f);
            pts[1] = new SKPoint(bounds.Width * 0.779373368146214f, bounds.Height * 0.806338028169014f);
            pts[2] = new SKPoint(bounds.Width * 0.72845953002611f, bounds.Height * 0.794014084507042f);
            pts[3] = new SKPoint(bounds.Width * 0.74934725848564f, bounds.Height * 0.707746478873239f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.83289817232376f, bounds.Height * 0.751760563380282f);
            pts[1] = new SKPoint(bounds.Width * 0.83289817232376f, bounds.Height * 0.829225352112676f);
            pts[2] = new SKPoint(bounds.Width * 0.762402088772846f, bounds.Height * 0.806338028169014f);
            pts[3] = new SKPoint(bounds.Width * 0.77023498694517f, bounds.Height * 0.732394366197183f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.861618798955614f, bounds.Height * 0.744718309859155f);
            pts[1] = new SKPoint(bounds.Width * 0.866840731070496f, bounds.Height * 0.839788732394366f);
            pts[2] = new SKPoint(bounds.Width * 0.800261096605744f, bounds.Height * 0.827464788732394f);
            pts[3] = new SKPoint(bounds.Width * 0.801566579634465f, bounds.Height * 0.73943661971831f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.886422976501306f, bounds.Height * 0.735915492957747f);
            pts[1] = new SKPoint(bounds.Width * 0.921671018276762f, bounds.Height * 0.811619718309859f);
            pts[2] = new SKPoint(bounds.Width * 0.844647519582245f, bounds.Height * 0.825704225352113f);
            pts[3] = new SKPoint(bounds.Width * 0.843342036553525f, bounds.Height * 0.755281690140845f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.922976501305483f, bounds.Height * 0.727112676056338f);
            pts[1] = new SKPoint(bounds.Width * 0.966057441253264f, bounds.Height * 0.778169014084507f);
            pts[2] = new SKPoint(bounds.Width * 0.890339425587467f, bounds.Height * 0.822183098591549f);
            pts[3] = new SKPoint(bounds.Width * 0.866840731070496f, bounds.Height * 0.757042253521127f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.933420365535248f, bounds.Height * 0.702464788732394f);
            pts[1] = new SKPoint(bounds.Width * 0.989556135770235f, bounds.Height * 0.725352112676056f);
            pts[2] = new SKPoint(bounds.Width * 0.953002610966057f, bounds.Height * 0.797535211267606f);
            pts[3] = new SKPoint(bounds.Width * 0.89556135770235f, bounds.Height * 0.744718309859155f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.932114882506527f, bounds.Height * 0.679577464788732f);
            pts[1] = new SKPoint(bounds.Width * 0.990861618798956f, bounds.Height * 0.649647887323944f);
            pts[2] = new SKPoint(bounds.Width * 0.992167101827676f, bounds.Height * 0.742957746478873f);
            pts[3] = new SKPoint(bounds.Width * 0.919060052219321f, bounds.Height * 0.727112676056338f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.911227154046997f, bounds.Height * 0.65669014084507f);
            pts[1] = new SKPoint(bounds.Width * 0.968668407310705f, bounds.Height * 0.595070422535211f);
            pts[2] = new SKPoint(bounds.Width * 0.994778067885117f, bounds.Height * 0.674295774647887f);
            pts[3] = new SKPoint(bounds.Width * 0.934725848563969f, bounds.Height * 0.700704225352113f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.917754569190601f, bounds.Height * 0.572183098591549f);
            pts[1] = new SKPoint(bounds.Width * 0.882506527415144f, bounds.Height * 0.630281690140845f);
            pts[2] = new SKPoint(bounds.Width * 0.921671018276762f, bounds.Height * 0.676056338028169f);
            pts[3] = new SKPoint(bounds.Width * 0.976501305483029f, bounds.Height * 0.617957746478873f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.857702349869452f, bounds.Height * 0.544014084507042f);
            pts[1] = new SKPoint(bounds.Width * 0.852480417754569f, bounds.Height * 0.623239436619718f);
            pts[2] = new SKPoint(bounds.Width * 0.898172323759791f, bounds.Height * 0.644366197183099f);
            pts[3] = new SKPoint(bounds.Width * 0.932114882506527f, bounds.Height * 0.566901408450704f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.810704960835509f, bounds.Height * 0.536971830985916f);
            pts[1] = new SKPoint(bounds.Width * 0.823759791122715f, bounds.Height * 0.623239436619718f);
            pts[2] = new SKPoint(bounds.Width * 0.870757180156658f, bounds.Height * 0.625f);
            pts[3] = new SKPoint(bounds.Width * 0.882506527415144f, bounds.Height * 0.545774647887324f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.768929503916449f, bounds.Height * 0.551056338028169f);
            pts[1] = new SKPoint(bounds.Width * 0.789817232375979f, bounds.Height * 0.637323943661972f);
            pts[2] = new SKPoint(bounds.Width * 0.836814621409922f, bounds.Height * 0.623239436619718f);
            pts[3] = new SKPoint(bounds.Width * 0.823759791122715f, bounds.Height * 0.536971830985916f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.721932114882507f, bounds.Height * 0.559859154929578f);
            pts[1] = new SKPoint(bounds.Width * 0.748041775456919f, bounds.Height * 0.647887323943662f);
            pts[2] = new SKPoint(bounds.Width * 0.800261096605744f, bounds.Height * 0.626760563380282f);
            pts[3] = new SKPoint(bounds.Width * 0.787206266318538f, bounds.Height * 0.544014084507042f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.678851174934726f, bounds.Height * 0.580985915492958f);
            pts[1] = new SKPoint(bounds.Width * 0.70757180156658f, bounds.Height * 0.665492957746479f);
            pts[2] = new SKPoint(bounds.Width * 0.757180156657963f, bounds.Height * 0.633802816901408f);
            pts[3] = new SKPoint(bounds.Width * 0.738903394255875f, bounds.Height * 0.558098591549296f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.640992167101828f, bounds.Height * 0.595070422535211f);
            pts[1] = new SKPoint(bounds.Width * 0.660574412532637f, bounds.Height * 0.676056338028169f);
            pts[2] = new SKPoint(bounds.Width * 0.723237597911227f, bounds.Height * 0.653169014084507f);
            pts[3] = new SKPoint(bounds.Width * 0.693211488250653f, bounds.Height * 0.582746478873239f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.60443864229765f, bounds.Height * 0.60387323943662f);
            pts[1] = new SKPoint(bounds.Width * 0.609660574412533f, bounds.Height * 0.691901408450704f);
            pts[2] = new SKPoint(bounds.Width * 0.676240208877285f, bounds.Height * 0.677816901408451f);
            pts[3] = new SKPoint(bounds.Width * 0.660574412532637f, bounds.Height * 0.600352112676056f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.558746736292428f, bounds.Height * 0.609154929577465f);
            pts[1] = new SKPoint(bounds.Width * 0.566579634464752f, bounds.Height * 0.698943661971831f);
            pts[2] = new SKPoint(bounds.Width * 0.631853785900783f, bounds.Height * 0.688380281690141f);
            pts[3] = new SKPoint(bounds.Width * 0.617493472584856f, bounds.Height * 0.610915492957747f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.52088772845953f, bounds.Height * 0.609154929577465f);
            pts[1] = new SKPoint(bounds.Width * 0.510443864229765f, bounds.Height * 0.700704225352113f);
            pts[2] = new SKPoint(bounds.Width * 0.579634464751958f, bounds.Height * 0.705985915492958f);
            pts[3] = new SKPoint(bounds.Width * 0.577023498694517f, bounds.Height * 0.616197183098592f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.494778067885117f, bounds.Height * 0.60387323943662f);
            pts[1] = new SKPoint(bounds.Width * 0.463446475195822f, bounds.Height * 0.677816901408451f);
            pts[2] = new SKPoint(bounds.Width * 0.535248041775457f, bounds.Height * 0.705985915492958f);
            pts[3] = new SKPoint(bounds.Width * 0.544386422976501f, bounds.Height * 0.619718309859155f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.453002610966057f, bounds.Height * 0.575704225352113f);
            pts[1] = new SKPoint(bounds.Width * 0.425587467362924f, bounds.Height * 0.658450704225352f);
            pts[2] = new SKPoint(bounds.Width * 0.476501305483029f, bounds.Height * 0.691901408450704f);
            pts[3] = new SKPoint(bounds.Width * 0.531331592689295f, bounds.Height * 0.600352112676056f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.400783289817232f, bounds.Height * 0.556338028169014f);
            pts[1] = new SKPoint(bounds.Width * 0.390339425587467f, bounds.Height * 0.65669014084507f);
            pts[2] = new SKPoint(bounds.Width * 0.441253263707572f, bounds.Height * 0.681338028169014f);
            pts[3] = new SKPoint(bounds.Width * 0.476501305483029f, bounds.Height * 0.59330985915493f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.345953002610966f, bounds.Height * 0.556338028169014f);
            pts[1] = new SKPoint(bounds.Width * 0.35378590078329f, bounds.Height * 0.644366197183099f);
            pts[2] = new SKPoint(bounds.Width * 0.409921671018277f, bounds.Height * 0.654929577464789f);
            pts[3] = new SKPoint(bounds.Width * 0.425587467362924f, bounds.Height * 0.565140845070423f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.293733681462141f, bounds.Height * 0.566901408450704f);
            pts[1] = new SKPoint(bounds.Width * 0.322454308093995f, bounds.Height * 0.658450704225352f);
            pts[2] = new SKPoint(bounds.Width * 0.370757180156658f, bounds.Height * 0.649647887323944f);
            pts[3] = new SKPoint(bounds.Width * 0.369451697127937f, bounds.Height * 0.558098591549296f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.259791122715405f, bounds.Height * 0.59330985915493f);
            pts[1] = new SKPoint(bounds.Width * 0.280678851174935f, bounds.Height * 0.670774647887324f);
            pts[2] = new SKPoint(bounds.Width * 0.336814621409922f, bounds.Height * 0.644366197183099f);
            pts[3] = new SKPoint(bounds.Width * 0.31331592689295f, bounds.Height * 0.558098591549296f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.214099216710183f, bounds.Height * 0.586267605633803f);
            pts[1] = new SKPoint(bounds.Width * 0.234986945169713f, bounds.Height * 0.690140845070423f);
            pts[2] = new SKPoint(bounds.Width * 0.295039164490862f, bounds.Height * 0.669014084507042f);
            pts[3] = new SKPoint(bounds.Width * 0.284595300261097f, bounds.Height * 0.577464788732394f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.181462140992167f, bounds.Height * 0.602112676056338f);
            pts[1] = new SKPoint(bounds.Width * 0.18668407310705f, bounds.Height * 0.686619718309859f);
            pts[2] = new SKPoint(bounds.Width * 0.254569190600522f, bounds.Height * 0.676056338028169f);
            pts[3] = new SKPoint(bounds.Width * 0.248041775456919f, bounds.Height * 0.59330985915493f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.139686684073107f, bounds.Height * 0.602112676056338f);
            pts[1] = new SKPoint(bounds.Width * 0.139686684073107f, bounds.Height * 0.686619718309859f);
            pts[2] = new SKPoint(bounds.Width * 0.211488250652742f, bounds.Height * 0.686619718309859f);
            pts[3] = new SKPoint(bounds.Width * 0.204960835509138f, bounds.Height * 0.595070422535211f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.114882506527415f, bounds.Height * 0.584507042253521f);
            pts[1] = new SKPoint(bounds.Width * 0.0718015665796345f, bounds.Height * 0.663732394366197f);
            pts[2] = new SKPoint(bounds.Width * 0.143603133159269f, bounds.Height * 0.698943661971831f);
            pts[3] = new SKPoint(bounds.Width * 0.178851174934726f, bounds.Height * 0.60387323943662f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.0744125326370757f, bounds.Height * 0.561619718309859f);
            pts[1] = new SKPoint(bounds.Width * 0.0248041775456919f, bounds.Height * 0.632042253521127f);
            pts[2] = new SKPoint(bounds.Width * 0.0966057441253264f, bounds.Height * 0.677816901408451f);
            pts[3] = new SKPoint(bounds.Width * 0.131853785900783f, bounds.Height * 0.600352112676056f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.0600522193211488f, bounds.Height * 0.556971830985916f);
            pts[1] = new SKPoint(bounds.Width * 0, bounds.Height * 0.566901408450704f);
            pts[2] = new SKPoint(bounds.Width * 0.031331592689295f, bounds.Height * 0.63556338028169f);
            pts[3] = new SKPoint(bounds.Width * 0.097911227154047f, bounds.Height * 0.591549295774648f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.0783028720626632f, bounds.Height * 0.524802816901408f);
            pts[1] = new SKPoint(bounds.Width * 0, bounds.Height * 0.501760563380282f);
            pts[2] = new SKPoint(bounds.Width * 0, bounds.Height * 0.582746478873239f);
            pts[3] = new SKPoint(bounds.Width * 0.0796344647519582f, bounds.Height * 0.563380281690141f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.0731070496083551f, bounds.Height * 0.503521126760563f);
            pts[1] = new SKPoint(bounds.Width * 0.00261096605744125f, bounds.Height * 0.397887323943662f);
            pts[2] = new SKPoint(bounds.Width * 0.00261096605744125f, bounds.Height * 0.514084507042254f);
            pts[3] = new SKPoint(bounds.Width * 0.0613577023498695f, bounds.Height * 0.533450704225352f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.0783289817232376f, bounds.Height * 0.38556338028169f);
            pts[1] = new SKPoint(bounds.Width * 0.109660574412533f, bounds.Height * 0.492957746478873f);
            pts[2] = new SKPoint(bounds.Width * 0.0496083550913838f, bounds.Height * 0.522887323943662f);
            pts[3] = new SKPoint(bounds.Width * 0.0130548302872063f, bounds.Height * 0.434859154929577f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.135770234986945f, bounds.Height * 0.376760563380282f);
            pts[1] = new SKPoint(bounds.Width * 0.14490861618799f, bounds.Height * 0.491197183098592f);
            pts[2] = new SKPoint(bounds.Width * 0.0796344647519582f, bounds.Height * 0.5f);
            pts[3] = new SKPoint(bounds.Width * 0.0430809399477807f, bounds.Height * 0.390845070422535f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.191906005221932f, bounds.Height * 0.378521126760563f);
            pts[1] = new SKPoint(bounds.Width * 0.193211488250653f, bounds.Height * 0.480633802816901f);
            pts[2] = new SKPoint(bounds.Width * 0.12532637075718f, bounds.Height * 0.494718309859155f);
            pts[3] = new SKPoint(bounds.Width * 0.109660574412533f, bounds.Height * 0.387323943661972f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.238903394255875f, bounds.Height * 0.389084507042254f);
            pts[1] = new SKPoint(bounds.Width * 0.22845953002611f, bounds.Height * 0.496478873239437f);
            pts[2] = new SKPoint(bounds.Width * 0.16710182767624f, bounds.Height * 0.487676056338028f);
            pts[3] = new SKPoint(bounds.Width * 0.16579634464752f, bounds.Height * 0.38556338028169f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.278067885117493f, bounds.Height * 0.399647887323944f);
            pts[1] = new SKPoint(bounds.Width * 0.272845953002611f, bounds.Height * 0.492957746478873f);
            pts[2] = new SKPoint(bounds.Width * 0.210182767624021f, bounds.Height * 0.480633802816901f);
            pts[3] = new SKPoint(bounds.Width * 0.212793733681462f, bounds.Height * 0.389084507042254f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.301566579634465f, bounds.Height * 0.397887323943662f);
            pts[1] = new SKPoint(bounds.Width * 0.335509138381201f, bounds.Height * 0.477112676056338f);
            pts[2] = new SKPoint(bounds.Width * 0.254569190600522f, bounds.Height * 0.48943661971831f);
            pts[3] = new SKPoint(bounds.Width * 0.245430809399478f, bounds.Height * 0.394366197183099f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.301566579634465f, bounds.Height * 0.39612676056338f);
            pts[1] = new SKPoint(bounds.Width * 0.379895561357702f, bounds.Height * 0.411971830985916f);
            pts[2] = new SKPoint(bounds.Width * 0.309399477806789f, bounds.Height * 0.508802816901408f);
            pts[3] = new SKPoint(bounds.Width * 0.261096605744125f, bounds.Height * 0.401408450704225f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.317232375979112f, bounds.Height * 0.350352112676056f);
            pts[1] = new SKPoint(bounds.Width * 0.382506527415144f, bounds.Height * 0.366197183098592f);
            pts[2] = new SKPoint(bounds.Width * 0.368146214099217f, bounds.Height * 0.440140845070423f);
            pts[3] = new SKPoint(bounds.Width * 0.300261096605744f, bounds.Height * 0.413732394366197f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.340731070496084f, bounds.Height * 0.295774647887324f);
            pts[1] = new SKPoint(bounds.Width * 0.39686684073107f, bounds.Height * 0.350352112676056f);
            pts[2] = new SKPoint(bounds.Width * 0.364229765013055f, bounds.Height * 0.417253521126761f);
            pts[3] = new SKPoint(bounds.Width * 0.287206266318538f, bounds.Height * 0.387323943661972f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.389033942558747f, bounds.Height * 0.26056338028169f);
            pts[1] = new SKPoint(bounds.Width * 0.422976501305483f, bounds.Height * 0.332746478873239f);
            pts[2] = new SKPoint(bounds.Width * 0.378590078328982f, bounds.Height * 0.38556338028169f);
            pts[3] = new SKPoint(bounds.Width * 0.31331592689295f, bounds.Height * 0.34330985915493f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.449086161879896f, bounds.Height * 0.26056338028169f);
            pts[1] = new SKPoint(bounds.Width * 0.449086161879896f, bounds.Height * 0.322183098591549f);
            pts[2] = new SKPoint(bounds.Width * 0.390339425587467f, bounds.Height * 0.339788732394366f);
            pts[3] = new SKPoint(bounds.Width * 0.373368146214099f, bounds.Height * 0.276408450704225f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.503916449086162f, bounds.Height * 0.26056338028169f);
            pts[1] = new SKPoint(bounds.Width * 0.472584856396867f, bounds.Height * 0.339788732394366f);
            pts[2] = new SKPoint(bounds.Width * 0.426892950391645f, bounds.Height * 0.334507042253521f);
            pts[3] = new SKPoint(bounds.Width * 0.422976501305483f, bounds.Height * 0.244718309859155f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.539164490861619f, bounds.Height * 0.283450704225352f);
            pts[1] = new SKPoint(bounds.Width * 0.5f, bounds.Height * 0.359154929577465f);
            pts[2] = new SKPoint(bounds.Width * 0.447780678851175f, bounds.Height * 0.332746478873239f);
            pts[3] = new SKPoint(bounds.Width * 0.483028720626632f, bounds.Height * 0.255281690140845f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.571801566579634f, bounds.Height * 0.306338028169014f);
            pts[1] = new SKPoint(bounds.Width * 0.523498694516971f, bounds.Height * 0.392605633802817f);
            pts[2] = new SKPoint(bounds.Width * 0.480417754569191f, bounds.Height * 0.350352112676056f);
            pts[3] = new SKPoint(bounds.Width * 0.524804177545692f, bounds.Height * 0.279929577464789f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.60313315926893f, bounds.Height * 0.327464788732394f);
            pts[1] = new SKPoint(bounds.Width * 0.560052219321149f, bounds.Height * 0.415492957746479f);
            pts[2] = new SKPoint(bounds.Width * 0.507832898172324f, bounds.Height * 0.376760563380282f);
            pts[3] = new SKPoint(bounds.Width * 0.570496083550914f, bounds.Height * 0.285211267605634f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.626631853785901f, bounds.Height * 0.348591549295775f);
            pts[1] = new SKPoint(bounds.Width * 0.590078328981723f, bounds.Height * 0.429577464788732f);
            pts[2] = new SKPoint(bounds.Width * 0.539164490861619f, bounds.Height * 0.39612676056338f);
            pts[3] = new SKPoint(bounds.Width * 0.592689295039164f, bounds.Height * 0.306338028169014f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.661879895561358f, bounds.Height * 0.366197183098592f);
            pts[1] = new SKPoint(bounds.Width * 0.626631853785901f, bounds.Height * 0.459507042253521f);
            pts[2] = new SKPoint(bounds.Width * 0.560052219321149f, bounds.Height * 0.411971830985916f);
            pts[3] = new SKPoint(bounds.Width * 0.612271540469974f, bounds.Height * 0.327464788732394f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.693211488250653f, bounds.Height * 0.383802816901408f);
            pts[1] = new SKPoint(bounds.Width * 0.659268929503916f, bounds.Height * 0.46830985915493f);
            pts[2] = new SKPoint(bounds.Width * 0.60443864229765f, bounds.Height * 0.438380281690141f);
            pts[3] = new SKPoint(bounds.Width * 0.639686684073107f, bounds.Height * 0.350352112676056f);
            gp.AddPoly(pts, true);


            _arr_Paths.Add(gp);

            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.725848563968668f, bounds.Height * 0.408450704225352f);
            pts[1] = new SKPoint(bounds.Width * 0.701044386422977f, bounds.Height * 0.492957746478873f);
            pts[2] = new SKPoint(bounds.Width * 0.64621409921671f, bounds.Height * 0.459507042253521f);
            pts[3] = new SKPoint(bounds.Width * 0.672323759791123f, bounds.Height * 0.38556338028169f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.757180156657963f, bounds.Height * 0.424295774647887f);
            pts[1] = new SKPoint(bounds.Width * 0.736292428198433f, bounds.Height * 0.501760563380282f);
            pts[2] = new SKPoint(bounds.Width * 0.673629242819843f, bounds.Height * 0.473591549295775f);
            pts[3] = new SKPoint(bounds.Width * 0.698433420365535f, bounds.Height * 0.394366197183099f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.798955613577023f, bounds.Height * 0.431338028169014f);
            pts[1] = new SKPoint(bounds.Width * 0.776762402088773f, bounds.Height * 0.515845070422535f);
            pts[2] = new SKPoint(bounds.Width * 0.7088772845953f, bounds.Height * 0.491197183098592f);
            pts[3] = new SKPoint(bounds.Width * 0.737597911227154f, bounds.Height * 0.408450704225352f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.826370757180157f, bounds.Height * 0.441901408450704f);
            pts[1] = new SKPoint(bounds.Width * 0.825065274151436f, bounds.Height * 0.517605633802817f);
            pts[2] = new SKPoint(bounds.Width * 0.753263707571802f, bounds.Height * 0.503521126760563f);
            pts[3] = new SKPoint(bounds.Width * 0.772845953002611f, bounds.Height * 0.417253521126761f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.869451697127937f, bounds.Height * 0.441901408450704f);
            pts[1] = new SKPoint(bounds.Width * 0.873368146214099f, bounds.Height * 0.524647887323944f);
            pts[2] = new SKPoint(bounds.Width * 0.796344647519582f, bounds.Height * 0.526408450704225f);
            pts[3] = new SKPoint(bounds.Width * 0.804177545691906f, bounds.Height * 0.440140845070423f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.906005221932115f, bounds.Height * 0.431338028169014f);
            pts[1] = new SKPoint(bounds.Width * 0.928198433420366f, bounds.Height * 0.51056338028169f);
            pts[2] = new SKPoint(bounds.Width * 0.847258485639687f, bounds.Height * 0.517605633802817f);
            pts[3] = new SKPoint(bounds.Width * 0.830287206266319f, bounds.Height * 0.445422535211268f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.933420365535248f, bounds.Height * 0.429577464788732f);
            pts[1] = new SKPoint(bounds.Width * 0.968668407310705f, bounds.Height * 0.498239436619718f);
            pts[2] = new SKPoint(bounds.Width * 0.903394255874674f, bounds.Height * 0.508802816901408f);
            pts[3] = new SKPoint(bounds.Width * 0.886422976501306f, bounds.Height * 0.450704225352113f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.953002610966057f, bounds.Height * 0.422535211267606f);
            pts[1] = new SKPoint(bounds.Width * 0.997389033942559f, bounds.Height * 0.447183098591549f);
            pts[2] = new SKPoint(bounds.Width * 0.954308093994778f, bounds.Height * 0.51056338028169f);
            pts[3] = new SKPoint(bounds.Width * 0.913838120104439f, bounds.Height * 0.440140845070423f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.953002610966057f, bounds.Height * 0.408450704225352f);
            pts[1] = new SKPoint(bounds.Width * 0.997389033942559f, bounds.Height * 0.380281690140845f);
            pts[2] = new SKPoint(bounds.Width * 0.997389033942559f, bounds.Height * 0.450704225352113f);
            pts[3] = new SKPoint(bounds.Width * 0.934725848563969f, bounds.Height * 0.436619718309859f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.932114882506527f, bounds.Height * 0.383802816901408f);
            pts[1] = new SKPoint(bounds.Width * 0.996083550913838f, bounds.Height * 0.313380281690141f);
            pts[2] = new SKPoint(bounds.Width * 0.998694516971279f, bounds.Height * 0.408450704225352f);
            pts[3] = new SKPoint(bounds.Width * 0.950391644908616f, bounds.Height * 0.429577464788732f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.924281984334204f, bounds.Height * 0.302816901408451f);
            pts[1] = new SKPoint(bounds.Width * 0.907310704960836f, bounds.Height * 0.38556338028169f);
            pts[2] = new SKPoint(bounds.Width * 0.946475195822454f, bounds.Height * 0.420774647887324f);
            pts[3] = new SKPoint(bounds.Width * 0.989556135770235f, bounds.Height * 0.325704225352113f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.879895561357702f, bounds.Height * 0.301056338028169f);
            pts[1] = new SKPoint(bounds.Width * 0.873368146214099f, bounds.Height * 0.375f);
            pts[2] = new SKPoint(bounds.Width * 0.945169712793734f, bounds.Height * 0.399647887323944f);
            pts[3] = new SKPoint(bounds.Width * 0.955613577023499f, bounds.Height * 0.304577464788732f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.865535248041776f, bounds.Height * 0.292253521126761f);
            pts[1] = new SKPoint(bounds.Width * 0.81331592689295f, bounds.Height * 0.359154929577465f);
            pts[2] = new SKPoint(bounds.Width * 0.894255874673629f, bounds.Height * 0.397887323943662f);
            pts[3] = new SKPoint(bounds.Width * 0.933420365535248f, bounds.Height * 0.308098591549296f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.866840731070496f, bounds.Height * 0.288732394366197f);
            pts[1] = new SKPoint(bounds.Width * 0.783289817232376f, bounds.Height * 0.26056338028169f);
            pts[2] = new SKPoint(bounds.Width * 0.822454308093995f, bounds.Height * 0.420774647887324f);
            pts[3] = new SKPoint(bounds.Width * 0.900783289817232f, bounds.Height * 0.309859154929577f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.849869451697128f, bounds.Height * 0.205985915492958f);
            pts[1] = new SKPoint(bounds.Width * 0.899477806788512f, bounds.Height * 0.28169014084507f);
            pts[2] = new SKPoint(bounds.Width * 0.808093994778068f, bounds.Height * 0.352112676056338f);
            pts[3] = new SKPoint(bounds.Width * 0.762402088772846f, bounds.Height * 0.253521126760563f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.881201044386423f, bounds.Height * 0.183098591549296f);
            pts[1] = new SKPoint(bounds.Width * 0.939947780678851f, bounds.Height * 0.251760563380282f);
            pts[2] = new SKPoint(bounds.Width * 0.869451697127937f, bounds.Height * 0.309859154929577f);
            pts[3] = new SKPoint(bounds.Width * 0.814621409921671f, bounds.Height * 0.234154929577465f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.908616187989556f, bounds.Height * 0.167253521126761f);
            pts[1] = new SKPoint(bounds.Width * 0.968668407310705f, bounds.Height * 0.195422535211268f);
            pts[2] = new SKPoint(bounds.Width * 0.913838120104439f, bounds.Height * 0.27112676056338f);
            pts[3] = new SKPoint(bounds.Width * 0.847258485639687f, bounds.Height * 0.220070422535211f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.902088772845953f, bounds.Height * 0.149647887323944f);
            pts[1] = new SKPoint(bounds.Width * 0.976501305483029f, bounds.Height * 0.116197183098592f);
            pts[2] = new SKPoint(bounds.Width * 0.966057441253264f, bounds.Height * 0.225352112676056f);
            pts[3] = new SKPoint(bounds.Width * 0.877284595300261f, bounds.Height * 0.204225352112676f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.887728459530026f, bounds.Height * 0.121478873239437f);
            pts[1] = new SKPoint(bounds.Width * 0.939947780678851f, bounds.Height * 0.0616197183098592f);
            pts[2] = new SKPoint(bounds.Width * 0.975195822454308f, bounds.Height * 0.147887323943662f);
            pts[3] = new SKPoint(bounds.Width * 0.889033942558747f, bounds.Height * 0.177816901408451f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.861618798955614f, bounds.Height * 0.0475352112676056f);
            pts[1] = new SKPoint(bounds.Width * 0.861618798955614f, bounds.Height * 0.126760563380282f);
            pts[2] = new SKPoint(bounds.Width * 0.903394255874674f, bounds.Height * 0.154929577464789f);
            pts[3] = new SKPoint(bounds.Width * 0.973890339425587f, bounds.Height * 0.051056338028169f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.81201044386423f, bounds.Height * 0.0404929577464789f);
            pts[1] = new SKPoint(bounds.Width * 0.83289817232376f, bounds.Height * 0.13556338028169f);
            pts[2] = new SKPoint(bounds.Width * 0.894255874673629f, bounds.Height * 0.132042253521127f);
            pts[3] = new SKPoint(bounds.Width * 0.898172323759791f, bounds.Height * 0.028169014084507f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.759791122715405f, bounds.Height * 0.051056338028169f);
            pts[1] = new SKPoint(bounds.Width * 0.802872062663185f, bounds.Height * 0.149647887323944f);
            pts[2] = new SKPoint(bounds.Width * 0.865535248041776f, bounds.Height * 0.133802816901408f);
            pts[3] = new SKPoint(bounds.Width * 0.843342036553525f, bounds.Height * 0.0440140845070423f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.714099216710183f, bounds.Height * 0.0862676056338028f);
            pts[1] = new SKPoint(bounds.Width * 0.780678851174935f, bounds.Height * 0.167253521126761f);
            pts[2] = new SKPoint(bounds.Width * 0.843342036553525f, bounds.Height * 0.137323943661972f);
            pts[3] = new SKPoint(bounds.Width * 0.797650130548303f, bounds.Height * 0.0422535211267606f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.689295039164491f, bounds.Height * 0.119718309859155f);
            pts[1] = new SKPoint(bounds.Width * 0.74934725848564f, bounds.Height * 0.188380281690141f);
            pts[2] = new SKPoint(bounds.Width * 0.7911227154047f, bounds.Height * 0.153169014084507f);
            pts[3] = new SKPoint(bounds.Width * 0.736292428198433f, bounds.Height * 0.0721830985915493f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.66710182767624f, bounds.Height * 0.147887323943662f);
            pts[1] = new SKPoint(bounds.Width * 0.723237597911227f, bounds.Height * 0.21830985915493f);
            pts[2] = new SKPoint(bounds.Width * 0.772845953002611f, bounds.Height * 0.174295774647887f);
            pts[3] = new SKPoint(bounds.Width * 0.710182767624021f, bounds.Height * 0.116197183098592f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.64621409921671f, bounds.Height * 0.158450704225352f);
            pts[1] = new SKPoint(bounds.Width * 0.689295039164491f, bounds.Height * 0.244718309859155f);
            pts[2] = new SKPoint(bounds.Width * 0.738903394255875f, bounds.Height * 0.204225352112676f);
            pts[3] = new SKPoint(bounds.Width * 0.698433420365535f, bounds.Height * 0.125f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.627937336814621f, bounds.Height * 0.165492957746479f);
            pts[1] = new SKPoint(bounds.Width * 0.638381201044386f, bounds.Height * 0.267605633802817f);
            pts[2] = new SKPoint(bounds.Width * 0.710182767624021f, bounds.Height * 0.237676056338028f);
            pts[3] = new SKPoint(bounds.Width * 0.681462140992167f, bounds.Height * 0.153169014084507f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.592689295039164f, bounds.Height * 0.161971830985915f);
            pts[1] = new SKPoint(bounds.Width * 0.578328981723238f, bounds.Height * 0.262323943661972f);
            pts[2] = new SKPoint(bounds.Width * 0.651436031331593f, bounds.Height * 0.262323943661972f);
            pts[3] = new SKPoint(bounds.Width * 0.640992167101828f, bounds.Height * 0.151408450704225f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.554830287206266f, bounds.Height * 0.160211267605634f);
            pts[1] = new SKPoint(bounds.Width * 0.535248041775457f, bounds.Height * 0.251760563380282f);
            pts[2] = new SKPoint(bounds.Width * 0.612271540469974f, bounds.Height * 0.267605633802817f);
            pts[3] = new SKPoint(bounds.Width * 0.614882506527415f, bounds.Height * 0.181338028169014f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.502610966057441f, bounds.Height * 0.140845070422535f);
            pts[1] = new SKPoint(bounds.Width * 0.492167101827676f, bounds.Height * 0.223591549295775f);
            pts[2] = new SKPoint(bounds.Width * 0.56266318537859f, bounds.Height * 0.27112676056338f);
            pts[3] = new SKPoint(bounds.Width * 0.600522193211488f, bounds.Height * 0.174295774647887f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.446475195822454f, bounds.Height * 0.137323943661972f);
            pts[1] = new SKPoint(bounds.Width * 0.456919060052219f, bounds.Height * 0.214788732394366f);
            pts[2] = new SKPoint(bounds.Width * 0.519582245430809f, bounds.Height * 0.230633802816901f);
            pts[3] = new SKPoint(bounds.Width * 0.527415143603133f, bounds.Height * 0.149647887323944f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.392950391644909f, bounds.Height * 0.161971830985915f);
            pts[1] = new SKPoint(bounds.Width * 0.430809399477807f, bounds.Height * 0.23943661971831f);
            pts[2] = new SKPoint(bounds.Width * 0.490861618798956f, bounds.Height * 0.230633802816901f);
            pts[3] = new SKPoint(bounds.Width * 0.473890339425587f, bounds.Height * 0.140845070422535f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.351174934725849f, bounds.Height * 0.174295774647887f);
            pts[1] = new SKPoint(bounds.Width * 0.39686684073107f, bounds.Height * 0.26056338028169f);
            pts[2] = new SKPoint(bounds.Width * 0.453002610966057f, bounds.Height * 0.246478873239437f);
            pts[3] = new SKPoint(bounds.Width * 0.426892950391645f, bounds.Height * 0.151408450704225f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.326370757180157f, bounds.Height * 0.200704225352113f);
            pts[1] = new SKPoint(bounds.Width * 0.349869451697128f, bounds.Height * 0.27112676056338f);
            pts[2] = new SKPoint(bounds.Width * 0.407310704960835f, bounds.Height * 0.242957746478873f);
            pts[3] = new SKPoint(bounds.Width * 0.381201044386423f, bounds.Height * 0.179577464788732f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.288511749347258f, bounds.Height * 0.204225352112676f);
            pts[1] = new SKPoint(bounds.Width * 0.309399477806789f, bounds.Height * 0.292253521126761f);
            pts[2] = new SKPoint(bounds.Width * 0.365535248041775f, bounds.Height * 0.269366197183099f);
            pts[3] = new SKPoint(bounds.Width * 0.345953002610966f, bounds.Height * 0.179577464788732f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.24934725848564f, bounds.Height * 0.202464788732394f);
            pts[1] = new SKPoint(bounds.Width * 0.258485639686684f, bounds.Height * 0.309859154929577f);
            pts[2] = new SKPoint(bounds.Width * 0.323759791122715f, bounds.Height * 0.292253521126761f);
            pts[3] = new SKPoint(bounds.Width * 0.309399477806789f, bounds.Height * 0.193661971830986f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.20757180156658f, bounds.Height * 0.197183098591549f);
            pts[1] = new SKPoint(bounds.Width * 0.210182767624021f, bounds.Height * 0.311619718309859f);
            pts[2] = new SKPoint(bounds.Width * 0.281984334203655f, bounds.Height * 0.308098591549296f);
            pts[3] = new SKPoint(bounds.Width * 0.283289817232376f, bounds.Height * 0.220070422535211f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.178851174934726f, bounds.Height * 0.211267605633803f);
            pts[1] = new SKPoint(bounds.Width * 0.147519582245431f, bounds.Height * 0.299295774647887f);
            pts[2] = new SKPoint(bounds.Width * 0.236292428198433f, bounds.Height * 0.311619718309859f);
            pts[3] = new SKPoint(bounds.Width * 0.245430809399478f, bounds.Height * 0.21830985915493f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.155352480417755f, bounds.Height * 0.190140845070423f);
            pts[1] = new SKPoint(bounds.Width * 0.097911227154047f, bounds.Height * 0.279929577464789f);
            pts[2] = new SKPoint(bounds.Width * 0.171018276762402f, bounds.Height * 0.316901408450704f);
            pts[3] = new SKPoint(bounds.Width * 0.216710182767624f, bounds.Height * 0.21830985915493f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.131853785900783f, bounds.Height * 0.177816901408451f);
            pts[1] = new SKPoint(bounds.Width * 0.0561357702349869f, bounds.Height * 0.234154929577465f);
            pts[2] = new SKPoint(bounds.Width * 0.116187989556136f, bounds.Height * 0.309859154929577f);
            pts[3] = new SKPoint(bounds.Width * 0.176240208877285f, bounds.Height * 0.211267605633803f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.131853785900783f, bounds.Height * 0.151408450704225f);
            pts[1] = new SKPoint(bounds.Width * 0.0404699738903394f, bounds.Height * 0.170774647887324f);
            pts[2] = new SKPoint(bounds.Width * 0.0678851174934726f, bounds.Height * 0.27112676056338f);
            pts[3] = new SKPoint(bounds.Width * 0.18668407310705f, bounds.Height * 0.205985915492958f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.143603133159269f, bounds.Height * 0.123239436619718f);
            pts[1] = new SKPoint(bounds.Width * 0.0261096605744125f, bounds.Height * 0.102112676056338f);
            pts[2] = new SKPoint(bounds.Width * 0.0352480417754569f, bounds.Height * 0.209507042253521f);
            pts[3] = new SKPoint(bounds.Width * 0.154046997389034f, bounds.Height * 0.186619718309859f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.139686684073107f, bounds.Height * 0.105633802816901f);
            pts[1] = new SKPoint(bounds.Width * 0.0691906005221932f, bounds.Height * 0.0352112676056338f);
            pts[2] = new SKPoint(bounds.Width * 0.0248041775456919f, bounds.Height * 0.112676056338028f);
            pts[3] = new SKPoint(bounds.Width * 0.139686684073107f, bounds.Height * 0.170774647887324f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.163185378590078f, bounds.Height * 0.0933098591549296f);
            pts[1] = new SKPoint(bounds.Width * 0.131853785900783f, bounds.Height * 0.00880281690140845f);
            pts[2] = new SKPoint(bounds.Width * 0.0509138381201044f, bounds.Height * 0.0651408450704225f);
            pts[3] = new SKPoint(bounds.Width * 0.120104438642298f, bounds.Height * 0.163732394366197f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.191906005221932f, bounds.Height * 0.0792253521126761f);
            pts[1] = new SKPoint(bounds.Width * 0.189295039164491f, bounds.Height * 0.0105633802816901f);
            pts[2] = new SKPoint(bounds.Width * 0.122715404699739f, bounds.Height * 0.0140845070422535f);
            pts[3] = new SKPoint(bounds.Width * 0.129242819843342f, bounds.Height * 0.102112676056338f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.219321148825065f, bounds.Height * 0.0897887323943662f);
            pts[1] = new SKPoint(bounds.Width * 0.238903394255875f, bounds.Height * 0.0123239436619718f);
            pts[2] = new SKPoint(bounds.Width * 0.16579634464752f, bounds.Height * 0.0105633802816901f);
            pts[3] = new SKPoint(bounds.Width * 0.163185378590078f, bounds.Height * 0.0933098591549296f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.262402088772846f, bounds.Height * 0.0985915492957746f);
            pts[1] = new SKPoint(bounds.Width * 0.281984334203655f, bounds.Height * 0.0193661971830986f);
            pts[2] = new SKPoint(bounds.Width * 0.221932114882507f, bounds.Height * 0.0123239436619718f);
            pts[3] = new SKPoint(bounds.Width * 0.203655352480418f, bounds.Height * 0.0845070422535211f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.298955613577023f, bounds.Height * 0.109154929577465f);
            pts[1] = new SKPoint(bounds.Width * 0.327676240208877f, bounds.Height * 0.028169014084507f);
            pts[2] = new SKPoint(bounds.Width * 0.27023498694517f, bounds.Height * 0.0123239436619718f);
            pts[3] = new SKPoint(bounds.Width * 0.227154046997389f, bounds.Height * 0.096830985915493f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.343342036553525f, bounds.Height * 0.126760563380282f);
            pts[1] = new SKPoint(bounds.Width * 0.369451697127937f, bounds.Height * 0.0369718309859155f);
            pts[2] = new SKPoint(bounds.Width * 0.31201044386423f, bounds.Height * 0.0264084507042254f);
            pts[3] = new SKPoint(bounds.Width * 0.280678851174935f, bounds.Height * 0.096830985915493f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.39556135770235f, bounds.Height * 0.132042253521127f);
            pts[1] = new SKPoint(bounds.Width * 0.407310704960835f, bounds.Height * 0.0440140845070423f);
            pts[2] = new SKPoint(bounds.Width * 0.331592689295039f, bounds.Height * 0.0334507042253521f);
            pts[3] = new SKPoint(bounds.Width * 0.322454308093995f, bounds.Height * 0.123239436619718f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.449086161879896f, bounds.Height * 0.126760563380282f);
            pts[1] = new SKPoint(bounds.Width * 0.436031331592689f, bounds.Height * 0.051056338028169f);
            pts[2] = new SKPoint(bounds.Width * 0.377284595300261f, bounds.Height * 0.0475352112676056f);
            pts[3] = new SKPoint(bounds.Width * 0.361618798955614f, bounds.Height * 0.117957746478873f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            gp = new SKPath();
            pts[0] = new SKPoint(bounds.Width * 0.52088772845953f, bounds.Height * 0.132042253521127f);
            pts[1] = new SKPoint(bounds.Width * 0.513054830287206f, bounds.Height * 0.0352112676056338f);
            pts[2] = new SKPoint(bounds.Width * 0.413838120104439f, bounds.Height * 0.0334507042253521f);
            pts[3] = new SKPoint(bounds.Width * 0.421671018276762f, bounds.Height * 0.139084507042254f);
            gp.AddPoly(pts, true);
            _arr_Paths.Add(gp);
            _arr_Spaces = new CustomBasicList<TempSpace>();
            int Index = 0;
            foreach (var thisPath in _arr_Paths)
            {
                Index += 1;
                TempSpace thisSpace = new TempSpace();
                thisSpace.Bounds = thisPath.TightBounds; // try this.
                thisSpace.Number = Index;
                _arr_Spaces.Add(thisSpace);
            }
            TempSpace finalSpace = new TempSpace();
            finalSpace.Bounds = SKRect.Create(bounds.Left + bounds.Width * 0.464f, bounds.Top + bounds.Height * 0.01f, bounds.Width / 8, bounds.Height / 10);
            finalSpace.Number = 127;
            _arr_Spaces.Add(finalSpace);
        }
        private void CreatePositions()
        {
            _positionList = new CustomBasicList<SKPoint>
            {
                new SKPoint() { X = 121, Y = 601 },
                new SKPoint() { X = 155, Y = 599 },
                new SKPoint() { X = 180, Y = 588 },
                new SKPoint() { X = 205, Y = 573 },
                new SKPoint() { X = 230, Y = 559 },
                new SKPoint() { X = 253, Y = 543 },
                new SKPoint() { X = 278, Y = 530 },
                new SKPoint() { X = 308, Y = 525 },
                new SKPoint() { X = 345, Y = 537 },
                new SKPoint() { X = 372, Y = 549 },
                new SKPoint() { X = 398, Y = 565 },
                new SKPoint() { X = 417, Y = 587 },
                new SKPoint() { X = 445, Y = 602 },
                new SKPoint() { X = 476, Y = 616 },
                new SKPoint() { X = 516, Y = 624 },
                new SKPoint() { X = 544, Y = 608 },
                new SKPoint() { X = 548, Y = 576 },
                new SKPoint() { X = 531, Y = 549 },
                new SKPoint() { X = 507, Y = 534 },
                new SKPoint() { X = 482, Y = 513 },
                new SKPoint() { X = 490, Y = 485 },
                new SKPoint() { X = 517, Y = 468 },
                new SKPoint() { X = 552, Y = 474 },
                new SKPoint() { X = 582, Y = 492 },
                new SKPoint() { X = 608, Y = 507 },
                new SKPoint() { X = 639, Y = 514 },
                new SKPoint() { X = 675, Y = 517 },
                new SKPoint() { X = 700, Y = 512 },
                new SKPoint() { X = 735, Y = 505 },
                new SKPoint() { X = 763, Y = 485 },
                new SKPoint() { X = 769, Y = 458 },
                new SKPoint() { X = 756, Y = 425 },
                new SKPoint() { X = 733, Y = 401 },
                new SKPoint() { X = 699, Y = 385 },
                new SKPoint() { X = 663, Y = 380 },
                new SKPoint() { X = 634, Y = 387 },
                new SKPoint() { X = 599, Y = 398 },
                new SKPoint() { X = 566, Y = 411 },
                new SKPoint() { X = 536, Y = 420 },
                new SKPoint() { X = 497, Y = 431 },
                new SKPoint() { X = 464, Y = 438 },
                new SKPoint() { X = 427, Y = 438 },
                new SKPoint() { X = 391, Y = 434 },
                new SKPoint() { X = 361, Y = 415 },
                new SKPoint() { X = 326, Y = 401 },
                new SKPoint() { X = 290, Y = 398 },
                new SKPoint() { X = 257, Y = 404 },
                new SKPoint() { X = 224, Y = 411 },
                new SKPoint() { X = 192, Y = 420 },
                new SKPoint() { X = 155, Y = 429 },
                new SKPoint() { X = 121, Y = 431 },
                new SKPoint() { X = 83, Y = 424 },
                new SKPoint() { X = 47, Y = 406 },
                new SKPoint() { X = 25, Y = 383 },
                new SKPoint() { X = 24, Y = 351 },
                new SKPoint() { X = 32, Y = 327 },
                new SKPoint() { X = 51, Y = 300 },
                new SKPoint() { X = 85, Y = 285 },
                new SKPoint() { X = 126, Y = 285 },
                new SKPoint() { X = 164, Y = 289 },
                new SKPoint() { X = 196, Y = 292 },
                new SKPoint() { X = 230, Y = 292 },
                new SKPoint() { X = 258, Y = 277 },
                new SKPoint() { X = 274, Y = 248 },
                new SKPoint() { X = 284, Y = 222 },
                new SKPoint() { X = 302, Y = 198 },
                new SKPoint() { X = 335, Y = 186 },
                new SKPoint() { X = 366, Y = 190 },
                new SKPoint() { X = 397, Y = 202 },
                new SKPoint() { X = 422, Y = 216 },
                new SKPoint() { X = 448, Y = 230 },
                new SKPoint() { X = 472, Y = 246 },
                new SKPoint() { X = 496, Y = 259 },
                new SKPoint() { X = 524, Y = 273 },
                new SKPoint() { X = 552, Y = 283 },
                new SKPoint() { X = 582, Y = 295 },
                new SKPoint() { X = 610, Y = 304 },
                new SKPoint() { X = 642, Y = 312 },
                new SKPoint() { X = 675, Y = 316 },
                new SKPoint() { X = 713, Y = 317 },
                new SKPoint() { X = 747, Y = 312 },
                new SKPoint() { X = 774, Y = 298 },
                new SKPoint() { X = 776, Y = 272 },
                new SKPoint() { X = 768, Y = 244 },
                new SKPoint() { X = 745, Y = 225 },
                new SKPoint() { X = 715, Y = 219 },
                new SKPoint() { X = 684, Y = 213 },
                new SKPoint() { X = 661, Y = 193 },
                new SKPoint() { X = 676, Y = 163 },
                new SKPoint() { X = 711, Y = 148 },
                new SKPoint() { X = 741, Y = 126 },
                new SKPoint() { X = 747, Y = 97 },
                new SKPoint() { X = 737, Y = 68 },
                new SKPoint() { X = 704, Y = 47 },
                new SKPoint() { X = 671, Y = 50 },
                new SKPoint() { X = 640, Y = 60 },
                new SKPoint() { X = 611, Y = 71 },
                new SKPoint() { X = 584, Y = 87 },
                new SKPoint() { X = 563, Y = 104 },
                new SKPoint() { X = 541, Y = 120 },
                new SKPoint() { X = 517, Y = 132 },
                new SKPoint() { X = 482, Y = 134 },
                new SKPoint() { X = 447, Y = 131 },
                new SKPoint() { X = 411, Y = 120 },
                new SKPoint() { X = 372, Y = 112 },
                new SKPoint() { X = 339, Y = 123 },
                new SKPoint() { X = 311, Y = 139 },
                new SKPoint() { X = 280, Y = 146 },
                new SKPoint() { X = 246, Y = 152 },
                new SKPoint() { X = 212, Y = 157 },
                new SKPoint() { X = 178, Y = 161 },
                new SKPoint() { X = 141, Y = 162 },
                new SKPoint() { X = 106, Y = 157 },
                new SKPoint() { X = 79, Y = 138 },
                new SKPoint() { X = 64, Y = 113 },
                new SKPoint() { X = 58, Y = 83 },
                new SKPoint() { X = 68, Y = 53 },
                new SKPoint() { X = 93, Y = 31 },
                new SKPoint() { X = 125, Y = 20 },
                new SKPoint() { X = 160, Y = 21 },
                new SKPoint() { X = 194, Y = 27 },
                new SKPoint() { X = 226, Y = 34 },
                new SKPoint() { X = 259, Y = 46 },
                new SKPoint() { X = 295, Y = 55 },
                new SKPoint() { X = 328, Y = 59 },
                new SKPoint() { X = 358, Y = 55 },
                new SKPoint() { X = 416, Y = 42 } // last one is the castle.
            };
        }
        private SKPaint? _bitPaint;
        private SKBitmap? _thisBit;
        private void CreatePaints()
        {
            Assembly thisAssembly;
            thisAssembly = Assembly.GetAssembly(GetType());
            _bitPaint = MiscHelpers.GetBitmapPaint();
            _thisBit = thisAssembly.GetSkBitmap("candylandboard.png");
            ExtraTop = -10;
            ExtraLeft = -4;
        }
        private CandylandPieceGraphicsCP GetPiece(CandylandPlayerItem thisPlayer) // this has to figure out location as well
        {
            var thisPos = _positionList[thisPlayer.SpaceNumber - 1];
            var newPos = GetActualPoint(thisPos);
            CandylandPieceGraphicsCP thisPiece = new CandylandPieceGraphicsCP();
            thisPiece.Location = newPos;
            thisPiece.ActualHeight = PieceHeight;
            thisPiece.ActualWidth = PieceWidth;
            thisPiece.MainColor = ColorForPiece(thisPlayer);
            thisPiece.NeedsToClear = false;
            return thisPiece;
        }
        public string ColorForPiece(CandylandPlayerItem thisPlayer)
        {
            var index = thisPlayer.Id;
            return index switch
            {
                1 => cs.Aqua,
                2 => cs.LimeGreen,
                3 => cs.Orange,
                4 => cs.Gray,
                _ => throw new Exception("Only 1 to 4 are supported for piece color"),
            };
        }
        public override void DrawGraphicsForBoard(SKCanvas canvas, float width, float height)
        {
            SetUpPaints(); //do here too.                
            var thisRect = SKRect.Create(0, 0, width, height);
            canvas.DrawBitmap(_thisBit, thisRect, _bitPaint);
        }
        protected override void DrawBoard(SKCanvas canvas)
        {
            canvas.Clear();
            CandylandPieceGraphicsCP thisPiece;
            var tempList = (from Items in _mainGame.PlayerList
                            where Items.SpaceNumber > 0
                            select Items).ToList(); // so at the beginning will not be considered
            tempList.Remove(_mainGame.SingleInfo);

            foreach (var thisPlayer in tempList)
            {
                thisPiece = GetPiece(thisPlayer);
                thisPiece.DrawImage(canvas);
            }
            if (_mainGame.SingleInfo!.SpaceNumber > 0)
            {
                thisPiece = GetPiece(_mainGame.SingleInfo);
                thisPiece.DrawImage(canvas);
            }
        }
        protected override void SetUpPaints() //this means i have to do set up paints twice overall.  its a sacrifice for another part.
        {
            CreatePaints();
            CreatePositions();
            var tempSize = GetActualSize(33, 43);
            PieceHeight = (int)tempSize.Height;
            PieceWidth = (int)tempSize.Width;
        }
        protected override async Task ClickProcessAsync(SKPoint thisPoint)
        {
            if (_command.IsExecuting)
            {
                return;
            }

            CandylandCardData thisCard;
            thisCard = _mainGame.SaveRoot!.CurrentCard!; //i think
            foreach (var thisItem in _arr_Spaces)
            {
                bool rets;
                if (thisItem.Number < 127)
                {
                    var tempSpace = _arr_Paths[thisItem.Number - 1];
                    rets = MiscHelpers.DidClickPath(tempSpace, thisPoint);
                }
                else
                    rets = MiscHelpers.DidClickRectangle(thisItem.Bounds, thisPoint);
                if (rets == true)
                {
                    bool isValid;
                    isValid = _gameBoard1.IsValidMove(thisItem.Number, thisCard.WhichCard, _mainGame, thisCard.HowMany);
                    if (isValid == true)
                        if (thisItem.Number == 127)
                        {
                            await _model.CastleAsync();
                            return;
                        }
                        else
                        {
                            await _model.MakeMoveAsync(thisItem.Number);
                            return;
                        }
                }
            }
        }
    }
}
