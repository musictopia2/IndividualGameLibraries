using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using FluxxCP;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace FluxxWPF
{
    public class KeeperUI : UserControl
    {
        private KeeperViewModel? _keeperMod;
        public void LoadControls()
        {
            FluxxViewModel thisMod = Resolve<FluxxViewModel>();
            FluxxMainGameClass mainGame = Resolve<FluxxMainGameClass>();
            var tempObj = thisMod.KeeperControl1;
            _keeperMod = (KeeperViewModel)tempObj!;
            Binding thisBind = GetVisibleBinding(nameof(KeeperViewModel.EntireVisible));
            SetBinding(VisibilityProperty, thisBind);
            DataContext = _keeperMod; //maybe i forgot this.
            Grid thisGrid = new Grid();
            if (mainGame.PlayerList.Count() <= 3)
                AddAutoRows(thisGrid, 3);
            else
                AddAutoRows(thisGrid, 4);
            if (mainGame.PlayerList.Count() == 2 || mainGame.PlayerList.Count() == 4)
            {
                AddLeftOverColumn(thisGrid, 50);
                AddLeftOverColumn(thisGrid, 50);
            }
            else
            {
                AddLeftOverColumn(thisGrid, 33);
                AddLeftOverColumn(thisGrid, 33);
                AddLeftOverColumn(thisGrid, 33);
            }
            var thisList = mainGame.PlayerList!.GetAllPlayersStartingWithSelf();
            var thisCard = new ShowCardUI();
            thisCard.LoadControls(EnumShowCategory.KeeperScreen);
            thisCard.Width = 1000;
            AddControlToGrid(thisGrid, thisCard, 0, 0);
            Grid.SetColumnSpan(thisCard, 3);
            thisCard.HorizontalAlignment = HorizontalAlignment.Left;
            thisCard.VerticalAlignment = VerticalAlignment.Top;
            var thisBut = GetGamingButton("Close Keeper Screen", nameof(KeeperViewModel.CloseCommand));
            thisBind = new Binding(nameof(KeeperViewModel.Section));
            var ThisConverter = new KeeperVisibleConverter();
            thisBind.ConverterParameter = EnumKeeperVisibleCategory.Close;
            thisBind.Converter = ThisConverter;
            thisBut.SetBinding(Button.VisibilityProperty, thisBind); // i think
            AddControlToGrid(thisGrid, thisBut, thisGrid.RowDefinitions.Count - 1, 0); // i think
            Grid.SetColumnSpan(thisBut, 3);
            thisBut.HorizontalAlignment = HorizontalAlignment.Center;
            thisBut.VerticalAlignment = VerticalAlignment.Top;
            thisBut = GetGamingButton("Test", nameof(KeeperViewModel.ProcessCommand));
            thisBind = new Binding(nameof(KeeperViewModel.Section));
            thisBind.Converter = new KeeperTextConverter();
            thisBut.SetBinding(Button.ContentProperty, thisBind);
            thisBind = new Binding(nameof(KeeperViewModel.Section));
            thisBind.Converter = new KeeperVisibleConverter();
            thisBind.ConverterParameter = EnumKeeperVisibleCategory.Actions;
            thisBut.SetBinding(Button.VisibilityProperty, thisBind);
            AddControlToGrid(thisGrid, thisBut, thisGrid.RowDefinitions.Count - 1, 0); // i think
            thisBut.HorizontalAlignment = HorizontalAlignment.Center;
            thisBut.VerticalAlignment = VerticalAlignment.Top;
            Grid.SetColumnSpan(thisBut, 3);
            int x = 0;
            int row = 0;
            int column = 0;
            foreach (var thisPlayer in thisList)
            {
                x += 1;
                if (x == 1 && thisPlayer.PlayerCategory != EnumPlayerCategory.Self)
                    throw new Exception("Failed to do self first");
                KeeperHandWPF thisHand = new KeeperHandWPF();
                var thisKeeper = _keeperMod.GetKeeperHand(thisPlayer);
                thisHand.LoadList(thisKeeper, "");
                if (x == 1)
                {
                    row = 1;
                    column = 0;
                }
                else if (x == 2)
                {
                    row = 1;
                    column = 1;
                }
                else if (x == 3 && mainGame.PlayerList.Count() == 4)
                {
                    row = 2;
                    column = 0;
                }
                else if (x == 3)
                {
                    row = 1;
                    column = 2;
                }
                else if (x == 4 && mainGame.PlayerList.Count() == 4)
                {
                    row = 2;
                    column = 1;
                }
                else if (x == 4)
                {
                    row = 2;
                    column = 0;
                }
                else if (x == 5)
                {
                    row = 2;
                    column = 1;
                }
                else if (x == 6)
                {
                    row = 2;
                    column = 2;
                }
                thisHand.MinWidth = 300;
                AddControlToGrid(thisGrid, thisHand, row, column);
            }
            Content = thisGrid;
        }
    }
}