using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.RummyClasses;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.CollectionClasses;
using Newtonsoft.Json;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace GalaxyCardGameCP
{
    public class GalaxyCardGamePlayerItem : PlayerTrick<EnumSuitList, GalaxyCardGameCardInformation>
    { //anything needed is here
        [JsonIgnore]
        public HandViewModel<GalaxyCardGameCardInformation>? PlanetHand;
        public string PlanetData { get; set; } = "";
        [JsonIgnore]
        public MainSetsViewModel<EnumSuitList, EnumColorList, GalaxyCardGameCardInformation, MoonClass, SavedSet>? Moons;
        public CustomBasicList<SavedSet> SavedMoonList { get; set; } = new CustomBasicList<SavedSet>();
        private GalaxyCardGameMainGameClass? _mainGame;

        public void LoadPiles(GalaxyCardGameMainGameClass mainGame)
        {
            _mainGame = mainGame;
            PlanetHand = new HandViewModel<GalaxyCardGameCardInformation>(mainGame.ThisMod!);
            PlanetHand.BoardClickedAsync += PlanetHand_BoardClickedAsync;
            PlanetHand.Maximum = 2;
            PlanetHand.Visible = true;
            PlanetHand.Text = $"{NickName} Planet";
            PlanetHand.SendEnableProcesses(mainGame.ThisMod!, () =>
            {
                if (PlayerCategory != EnumPlayerCategory.Self)
                    return false;
                if (mainGame.SaveRoot!.GameStatus != EnumGameStatus.PlaceSets)
                    return false;
                return mainGame.HasAutomaticPlanet() || PlanetHand.HandList.Count == 0;

            });
            Moons = new MainSetsViewModel<EnumSuitList, EnumColorList, GalaxyCardGameCardInformation, MoonClass, SavedSet>(mainGame.ThisMod!);
            Moons.SetClickedAsync += Moons_SetClickedAsync;
            Moons.HasFrame = true;
            Moons.Text = $"{NickName} Moons";
            Moons.Visible = true;
            Moons.SendEnableProcesses(mainGame.ThisMod!, () =>
            {
                if (PlayerCategory != EnumPlayerCategory.Self)
                    return false;
                return CanEnableMoon();
            });
        }
        public bool CanEnableMoon()
        {
            if (_mainGame!.SaveRoot!.GameStatus != EnumGameStatus.PlaceSets)
                return false;
            if (_mainGame.HasAutomaticPlanet())
                return false;
            return PlanetHand!.HandList.Count > 0;
        }
        private async Task Moons_SetClickedAsync(int setNumber, int section, int deck)
        {
            MoonClass thisMoon = Moons!.GetIndividualSet(setNumber);
            if (_mainGame!.CanAddToMoon(thisMoon, out GalaxyCardGameCardInformation? thisCard, out string message) == false)
            {
                await _mainGame.ThisMod!.ShowGameMessageAsync($"{message} {Constants.vbCrLf}{Constants.vbCrLf}Because of the reason above, was unable to expand the moon");
                return;
            }
            if (_mainGame.ThisData!.MultiPlayer)
            {
                SendExpandedMoon temps = new SendExpandedMoon();
                temps.MoonID = setNumber;
                temps.Deck = thisCard!.Deck;
                await _mainGame.ThisNet!.SendAllAsync("expandmoon", temps);
            }
            await _mainGame.AddToMoonAsync(thisCard!.Deck, setNumber);
        }

        private async Task PlanetHand_BoardClickedAsync()
        {
            var thisList = _mainGame!.ThisMod!.PlayerHand1!.ListSelectedObjects();
            if (thisList.Count > 2)
            {
                await _mainGame.ThisMod.ShowGameMessageAsync("Can only select 2 cards at the most for the planet");
                return;
            }
            if (_mainGame.HasValidPlanet(thisList) == false)
            {
                await _mainGame.ThisMod.ShowGameMessageAsync("This is not a valid planet");
                return;
            }
            if (_mainGame.ThisData!.MultiPlayer)
            {
                var tempList = thisList.GetDeckListFromObjectList();
                await _mainGame.ThisNet!.SendAllAsync("createplanet", tempList);
            }
            await _mainGame.CreatePlanetAsync(thisList);
        }
    }
}