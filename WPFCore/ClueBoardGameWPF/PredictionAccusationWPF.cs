using ClueBoardGameCP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using ClueBoardGameCP.ViewModels;
using CommonBasicStandardLibraries.Exceptions;
using BasicGamingUIWPFLibrary.Helpers;

namespace ClueBoardGameWPF
{
    public class PredictionAccusationWPF : UserControl
    {
        public void LoadControls(ClueBoardGameGameContainer gameContainer)
        {
            StackPanel firstStack = new StackPanel();
            firstStack.Orientation = Orientation.Horizontal;
            StackPanel firstRow = new StackPanel();
            StackPanel secondRow = new StackPanel();
            StackPanel thirdRow = new StackPanel();
            firstStack.Children.Add(firstRow);
            firstStack.Children.Add(secondRow);
            firstStack.Children.Add(thirdRow);
            var thisList = gameContainer.DetectiveList.Values.Where(items => items.Category == EnumCardType.IsRoom).ToCustomBasicList();
            Button thisBut;
            var thisLabel = GetLabel(EnumCardType.IsRoom);
            firstRow.Children.Add(thisLabel);
            thisList.ForEach(thisItem =>
            {
                thisBut = GetButton(thisItem);
                firstRow.Children.Add(thisBut);
            });
            thisList = gameContainer.DetectiveList.Values.Where(items => items.Category == EnumCardType.IsCharacter).ToCustomBasicList();
            thisLabel = GetLabel(EnumCardType.IsCharacter);
            secondRow.Children.Add(thisLabel);
            thisList.ForEach(thisItem =>
            {
                thisBut = GetButton(thisItem);
                secondRow.Children.Add(thisBut);
            });
            thisList = gameContainer.DetectiveList.Values.Where(items => items.Category == EnumCardType.IsWeapon).ToCustomBasicList();
            thisLabel = GetLabel(EnumCardType.IsWeapon);
            thirdRow.Children.Add(thisLabel);
            thisList.ForEach(thisItem =>
            {
                thisBut = GetButton(thisItem);
                thirdRow.Children.Add(thisBut);
            });
            Content = firstStack;
        }
        private TextBlock GetLabel(EnumCardType thisCat)
        {
            TextBlock output = new TextBlock();
            output.FontSize = 14;
            output.Foreground = Brushes.White;
            output.HorizontalAlignment = HorizontalAlignment.Center;
            output.FontWeight = FontWeights.Bold;
            switch (thisCat)
            {
                case EnumCardType.IsRoom:
                    output.Text = "Rooms";
                    break;
                case EnumCardType.IsWeapon:
                    output.Text = "Weapons";
                    break;
                case EnumCardType.IsCharacter:
                    output.Text = "Characters";
                    break;
                default:
                    break;
            }
            return output;
        }
        private Button GetButton(DetectiveInfo thisDet)
        {
            Button output = new Button();
            output.Content = thisDet.Name;
            output.FontSize = 22;
            PredictionConverter thisC = new PredictionConverter();
            Binding firstBind;
            //Binding secondBind;
            string secondName;
            switch (thisDet.Category)
            {
                case EnumCardType.IsRoom:
                    firstBind = new Binding(nameof(ClueBoardGameMainViewModel.CurrentRoomName));
                    secondName = nameof(ClueBoardGameMainViewModel.CurrentRoomClick);
                    //secondBind = new Binding(nameof(ClueBoardGameMainViewModel.CurrentRoomCommand));
                    break;
                case EnumCardType.IsWeapon:
                    firstBind = new Binding(nameof(ClueBoardGameMainViewModel.CurrentWeaponName));
                    secondName = nameof(ClueBoardGameMainViewModel.CurrentWeaponClick);
                    //secondBind = new Binding(nameof(ClueBoardGameMainViewModel.CurrentWeaponCommand));
                    break;
                case EnumCardType.IsCharacter:
                    firstBind = new Binding(nameof(ClueBoardGameMainViewModel.CurrentCharacterName));
                    secondName = nameof(ClueBoardGameMainViewModel.CurrentCharacterClick);
                    // = new Binding(nameof(ClueBoardGameMainViewModel.CurrentCharacterCommand));
                    break;
                default:
                    throw new BasicBlankException("Not Supported");
            }
            output.CommandParameter = thisDet.Name;
            firstBind.Converter = thisC;
            firstBind.ConverterParameter = thisDet.Name;
            output.SetBinding(BackgroundProperty, firstBind);
            output.Name = secondName;
            GamePackageViewModelBinder.ManuelElements.Add(output);
            return output;
        }
    }
}
