﻿// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.CustomControls;
using Com.Aurora.Shared.Extensions;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Com.Aurora.AuWeather.ViewModels;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Com.Aurora.AuWeather.SettingOptions
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ImmersiveSetting : Page, IThemeble
    {
        private Windows.ApplicationModel.Resources.ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();
        public ImmersiveSetting()
        {
            InitializeComponent();
            Context.FetchDataComplete += Context_FetchDataComplete;
            Context.SetLocalComplete += Context_SetLocalComplete;
            App.Current.Suspending += Current_Suspending;
        }

        private void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            Context.SaveAll();
        }

        private void Context_SetLocalComplete(object sender, ViewModels.Events.FetchDataCompleteEventArgs e)
        {
            if (Context.localFile != null)
            {
                var refresh = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(() =>
                  {
                      var title = e.Parameter as string;
                      switch (title)
                      {
                          case "Sunny":
                              HideList(SunnyList, SunnyLocal, SunnyLocalGrid, SunnyButton);
                              break;
                          case "Starry":
                              HideList(StarryList, StarryLocal, StarryLocalGrid, StarryButton);
                              break;
                          case "Cloudy":
                              HideList(CloudyList, CloudyLocal, CloudyLocalGrid, CloudyButton);
                              break;
                          case "Overcast":
                              HideList(OvercastList, OvercastLocal, OvercastLocalGrid, OvercastButton);
                              break;
                          case "Rainny":
                              HideList(RainnyList, RainnyLocal, RainnyLocalGrid, RainnyButton);
                              break;
                          case "Snowy":
                              HideList(SnowyList, SnowyLocal, SnowyLocalGrid, SnowyButton);
                              break;
                          case "Foggy":
                              HideList(FoggyList, FoggyLocal, FoggyLocalGrid, FoggyButton);
                              break;
                          case "Haze":
                              HideList(HazeList, HazeLocal, HazeLocalGrid, HazeButton);
                              break;
                          default:
                              break;
                      }
                  }));
            }
        }

        private void Context_FetchDataComplete(object sender, ViewModels.Events.FetchDataCompleteEventArgs e)
        {
            var task = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(() =>
              {
                  if (!Context.CurrentList.IsNullorEmpty())
                  {
                      Root.SelectionChanged -= Root_SelectionChanged;
                      foreach (var item in Context.CurrentList)
                      {
                          try
                          {
                              item.Thumbnail = new BitmapImage(item.Path);
                          }
                          catch (Exception)
                          {
                          }
                      }
                      switch (Root.SelectedIndex)
                      {
                          case 0:
                              if (Context.localFile != null && Context.SunnyState == Models.ImmersiveBackgroundState.Local)
                              {
                                  HideList(SunnyList, SunnyLocal, SunnyLocalGrid, SunnyButton);
                              }
                              else
                              {
                                  ShowList(SunnyList, SunnyLocal, SunnyLocalGrid, SunnyButton);
                              }
                              SunnyList.SelectionChanged -= List_SelectionChanged;
                              SunnyList.ItemsSource = Context.CurrentList;
                              SunnyList.SelectedIndex = Context.sunnyP;
                              SunnyList.SelectionChanged += List_SelectionChanged;
                              break;
                          case 1:
                              if (Context.localFile != null && Context.StarryState == Models.ImmersiveBackgroundState.Local)
                              {
                                  HideList(StarryList, StarryLocal, StarryLocalGrid, StarryButton);
                              }
                              else
                              {
                                  ShowList(StarryList, StarryLocal, StarryLocalGrid, StarryButton);
                              }
                              StarryList.SelectionChanged -= List_SelectionChanged;
                              StarryList.ItemsSource = Context.CurrentList;
                              StarryList.SelectedIndex = Context.starryP;
                              StarryList.SelectionChanged += List_SelectionChanged;
                              break;
                          case 2:
                              if (Context.localFile != null && Context.CloudyState == Models.ImmersiveBackgroundState.Local)
                              {
                                  HideList(CloudyList, CloudyLocal, CloudyLocalGrid, CloudyButton);
                              }
                              else
                              {
                                  ShowList(CloudyList, CloudyLocal, CloudyLocalGrid, CloudyButton);
                              }
                              CloudyList.SelectionChanged -= List_SelectionChanged;
                              CloudyList.ItemsSource = Context.CurrentList;
                              CloudyList.SelectedIndex = Context.cloudyP;
                              CloudyList.SelectionChanged += List_SelectionChanged;
                              break;
                          case 3:
                              if (Context.localFile != null && Context.RainnyState == Models.ImmersiveBackgroundState.Local)
                              {
                                  HideList(OvercastList, OvercastLocal, OvercastLocalGrid, OvercastButton);
                              }
                              else
                              {
                                  ShowList(OvercastList, OvercastLocal, OvercastLocalGrid, OvercastButton);
                              }
                              OvercastList.SelectionChanged -= List_SelectionChanged;
                              OvercastList.ItemsSource = Context.CurrentList;
                              OvercastList.SelectedIndex = Context.overcastP;
                              OvercastList.SelectionChanged += List_SelectionChanged;
                              break;
                          case 4:
                              if (Context.localFile != null && Context.RainnyState == Models.ImmersiveBackgroundState.Local)
                              {
                                  HideList(RainnyList, RainnyLocal, RainnyLocalGrid, RainnyButton);
                              }
                              else
                              {
                                  ShowList(RainnyList, RainnyLocal, RainnyLocalGrid, RainnyButton);
                              }
                              RainnyList.SelectionChanged -= List_SelectionChanged;
                              RainnyList.ItemsSource = Context.CurrentList;
                              RainnyList.SelectedIndex = Context.rainnyP;
                              RainnyList.SelectionChanged += List_SelectionChanged;
                              break;
                          case 5:
                              if (Context.localFile != null && Context.SnowyState == Models.ImmersiveBackgroundState.Local)
                              {
                                  HideList(SnowyList, SnowyLocal, SnowyLocalGrid, SnowyButton);
                              }
                              else
                              {
                                  ShowList(SnowyList, SnowyLocal, SnowyLocalGrid, SnowyButton);
                              }
                              SnowyList.SelectionChanged -= List_SelectionChanged;
                              SnowyList.ItemsSource = Context.CurrentList;
                              SnowyList.SelectedIndex = Context.snowyP;
                              SnowyList.SelectionChanged += List_SelectionChanged;
                              break;
                          case 6:
                              if (Context.localFile != null && Context.FoggyState == Models.ImmersiveBackgroundState.Local)
                              {
                                  HideList(FoggyList, FoggyLocal, FoggyLocalGrid, FoggyButton);
                              }
                              else
                              {
                                  ShowList(FoggyList, FoggyLocal, FoggyLocalGrid, FoggyButton);
                              }
                              FoggyList.SelectionChanged -= List_SelectionChanged;
                              FoggyList.ItemsSource = Context.CurrentList;
                              FoggyList.SelectedIndex = Context.foggyP;
                              FoggyList.SelectionChanged += List_SelectionChanged;
                              break;
                          case 7:
                              if (Context.localFile != null && Context.HazeState == Models.ImmersiveBackgroundState.Local)
                              {
                                  HideList(HazeList, HazeLocal, HazeLocalGrid, HazeButton);
                              }
                              else
                              {
                                  ShowList(HazeList, HazeLocal, HazeLocalGrid, HazeButton);
                              }
                              HazeList.SelectionChanged -= List_SelectionChanged;
                              HazeList.ItemsSource = Context.CurrentList;
                              HazeList.SelectedIndex = Context.hazeP;
                              HazeList.SelectionChanged += List_SelectionChanged;
                              break;
                          default:
                              break;
                      }
                      Root.SelectionChanged += Root_SelectionChanged;
                  }
              }));
        }

        private void ShowList(ListView list, Image image, Grid grid, Button button)
        {
            Context.ShowAssets(list.ItemsSource as CurrentImmersiveList);
            grid.Visibility = Visibility.Collapsed;
            image.Source = null;
            button.Content = loader.GetString("Add_Image");
        }

        private async void HideList(ListView list, Image image, Grid grid, Button button)
        {
            using (var bs = await Context.localFile.OpenReadAsync())
            {
                var bitmap = new BitmapImage();
                await bitmap.SetSourceAsync(bs);
                image.Source = bitmap;
            }
            grid.Visibility = Visibility.Visible;
            Context.HideAssets(list.ItemsSource as CurrentImmersiveList);
            button.Content = loader.GetString("Replace_Image");
        }

        private void Root_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearItemsSource();
            Context.ChangeCurrentList(Root.SelectedIndex);
        }

        private void ClearItemsSource()
        {
            SunnyList.ItemsSource = null;
            StarryList.ItemsSource = null;
            OvercastList.ItemsSource = null;
            CloudyList.ItemsSource = null;
            RainnyList.ItemsSource = null;
            SnowyList.ItemsSource = null;
            FoggyList.ItemsSource = null;
            HazeList.ItemsSource = null;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Context.SaveAll();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Context.SaveAll();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Context.DeleteLocal(Root.SelectedIndex);
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                Context.ChoseLocal(Root.SelectedIndex, file);
                Context.ClearShuffle(Root.SelectedIndex);
            }

        }

        private async void LocalButton_Click(object sender, RoutedEventArgs e)
        {
            //((Root.SelectedItem as PivotItem).Content as ListView).SelectedIndex = 0;
            //((((sender as Button).Parent as Grid).Parent as StackPanel).Children[1] as ListView).SelectedIndex = 0;
            await Context.DeleteLocal(Root.SelectedIndex);
        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Context.ChangeSelection(Root.SelectedIndex, (sender as ListView).SelectedIndex);
        }

        public void ChangeThemeColor(Color color)
        {
            var color1 = Color.FromArgb(Convert.ToByte(color.A * 0.9), color.R, color.G, color.B);
            var color2 = Color.FromArgb(Convert.ToByte(color.A * 0.6), color.R, color.G, color.B);
            var color3 = Color.FromArgb(Convert.ToByte(color.A * 0.8), color.R, color.G, color.B);
            (Resources["SystemControlBackgroundAccentBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlDisabledAccentBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlForegroundAccentBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHighlightAccentBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHighlightAltAccentBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHighlightAltListAccentHighBrush"] as SolidColorBrush).Color = color1;
            (Resources["SystemControlHighlightAltListAccentLowBrush"] as SolidColorBrush).Color = color2;
            (Resources["SystemControlHighlightAltListAccentMediumBrush"] as SolidColorBrush).Color = color3;
            (Resources["SystemControlHighlightListAccentHighBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHighlightListAccentLowBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHighlightListAccentMediumBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHyperlinkTextBrush"] as SolidColorBrush).Color = color;
            (Resources["ContentDialogBorderThemeBrush"] as SolidColorBrush).Color = color;
            (Resources["JumpListDefaultEnabledBackground"] as SolidColorBrush).Color = color;
            (Resources["SystemThemeMainBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlBackgroundAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlDisabledAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlForegroundAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAltAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAltListAccentHighBrush"] as SolidColorBrush).Color = color1;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAltListAccentLowBrush"] as SolidColorBrush).Color = color2;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAltListAccentMediumBrush"] as SolidColorBrush).Color = color3;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightListAccentHighBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightListAccentLowBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightListAccentMediumBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHyperlinkTextBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["ContentDialogBorderThemeBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["JumpListDefaultEnabledBackground"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemThemeMainBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlBackgroundAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlDisabledAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlForegroundAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAltAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAltListAccentHighBrush"] as SolidColorBrush).Color = color1;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAltListAccentLowBrush"] as SolidColorBrush).Color = color2;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAltListAccentMediumBrush"] as SolidColorBrush).Color = color3;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightListAccentHighBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightListAccentLowBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightListAccentMediumBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHyperlinkTextBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["ContentDialogBorderThemeBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["JumpListDefaultEnabledBackground"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemThemeMainBrush"] as SolidColorBrush).Color = color;
        }
    }
}
