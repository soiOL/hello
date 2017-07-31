﻿// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.Models;
using Com.Aurora.Shared.MVVM;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.System.Threading;
using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.AuWeather.ViewModels.Events;
using Com.Aurora.Shared.Helpers;
using Com.Aurora.AuWeather.Models.HeWeather;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;
using Com.Aurora.AuWeather.Tile;
using System.Collections.Generic;
using Com.Aurora.Shared.Extensions;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Com.Aurora.AuWeather.Core.Models;
using Com.Aurora.Shared.Converters;
using Com.Aurora.AuWeather.Models.HeWeather.JsonContract;
using Com.Aurora.AuWeather.Core.SQL;

namespace Com.Aurora.AuWeather.ViewModels
{
    public class CitiesPageViewModel : ViewModelBase
    {
        private SettingsModel settings = SettingsModel.Current;

        public event EventHandler<LocationUpdateEventArgs> LocationUpdate;
        public event EventHandler<FetchDataFailedEventArgs> FetchDataFailed;

        public Cities Cities { get; set; } = new Cities();

        private int currentIndex = -1;
        private ElementTheme theme;

        public int CurrentIndex
        {
            get
            {
                return currentIndex;
            }
            set
            {
                SetProperty(ref currentIndex, value);
            }
        }

        public ElementTheme Theme
        {
            get
            {
                return theme;
            }
            set
            {
                SetProperty(ref theme, value);
            }
        }

        public bool EnableLocate { get; private set; }

        public CitiesPageViewModel()
        {
            Theme = settings.Preferences.GetTheme();
            if (settings.Cities.EnableLocate)
            {
                if (settings.Cities.LocatedCity != null)
                    Cities.Add(new CityViewModel(settings.Cities.LocatedCity, true));
            }
            foreach (var city in settings.Cities.SavedCities)
            {
                Cities.Add(new CityViewModel(city, false));
            }
            Cities.Add(new CityViewModel());
            if (Cities.IsNullorEmpty())
            {
                var t = ThreadPool.RunAsync(async (work) =>
                  {
                      await Task.Delay(1000);
                      this.OnFetchDataFailed();
                  });
                return;
            }
            var task = ThreadPool.RunAsync(async (work) =>
            {
                var ask = ThreadPool.RunAsync(async (m) =>
                {
                    await SearchExistingDataAsync();
                    EnableLocate = settings.Cities.EnableLocate;
                    RequireLocationUpdate();
                });
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(async () =>
                {
                    await Init();
                    var t = ThreadPool.RunAsync((x) =>
                    {
                        Update();
                    });
                }));
            });
        }

        internal void DeniePos()
        {
            if (settings.Cities.LocatedCity != null && settings.Cities.EnableLocate)
            {
                Cities.RemoveAt(0);
            }
            settings.Cities.LocatedCity = null;
            settings.Cities.Save();
        }

        private async void Update()
        {
            var licens = new License.License();
            if (licens.IsPurchased)
            {
                foreach (var item in Cities)
                {
                    if (!item.Updated)
                    {
                        var task = ThreadPool.RunAsync(async (work) =>
                        {
                            string resstr = await Request.GetRequestAsync(settings, item.Id, item.City, item.longitude, item.latitude, item.zmw);
                            if (!resstr.IsNullorEmpty())
                            {
                                item.data = resstr;
                                await settings.Cities.SaveDataAsync(item.Id.IsNullorEmpty() ? item.City : item.Id, resstr, settings.Preferences.DataSource);
                                var index = Array.FindIndex(settings.Cities.SavedCities, x =>
                                {
                                    return x.Id == item.Id;
                                });
                                if (index != -1)
                                {
                                    settings.Cities.SavedCities[index].Update();
                                }
                                else if (item.Locate == "")
                                {
                                    settings.Cities.LocatedCity.Update();
                                }
                                settings.Cities.Save();
                                await Complete(item);
                            }
                        });
                    }
                }
            }
            else
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(() =>
                {
                    foreach (var item in Cities)
                    {
                        if (!item.Updated)
                        {
                            item.Succeed = false;
                        }
                    }
                }));
            }
        }

        internal void UpdateLocation(City cityInfo)
        {
            if (settings.Cities.LocatedCity != null && cityInfo.Id == settings.Cities.LocatedCity.Id)
            {
                return;
            }
            else
            {
                var c = new CitySettingsModel(cityInfo);
                if (settings.Cities.LocatedCity != null)
                {
                    Cities.RemoveAt(0);
                    Cities.Insert(0, new CityViewModel(c, true));
                }
                else
                {
                    Cities.Insert(0, new CityViewModel(c, false));
                }
                settings.Cities.LocatedCity = c;
                UpdateLocated();
            }
        }

        internal void Refresh()
        {
            foreach (var item in Cities)
            {
                item.Updated = false;
                item.Succeed = true;
            }
            var task = ThreadPool.RunAsync(async (work) =>
            {
                await SearchExistingDataAsync();

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(async () =>
                {
                    await Init();
                    var t = ThreadPool.RunAsync((x) =>
                    {
                        RequireLocationUpdate();
                        Update();
                    });
                }));
            });
        }


        internal void Delete(IList<object> selectedItems)
        {
            var its = new object[selectedItems.Count];
            selectedItems.CopyTo(its, 0);
            if (its.IsNullorEmpty())
            {
                return;
            }
            foreach (CityViewModel item in its)
            {
                Cities.Remove(item);
            }

            var t = ThreadPool.RunAsync((work) =>
            {
                var citys = new List<CitySettingsModel>();
                citys.AddRange(settings.Cities.SavedCities);
                foreach (CityViewModel c in its)
                {
                    var index = citys.FindIndex(x =>
                    {
                        return x.Id == c.Id;
                    });
                    if (index == -1 && settings.Cities.LocatedCity != null)
                    {
                        settings.Cities.LocatedCity = null;
                    }
                    else
                    {
                        citys.RemoveAt(index);
                    }
                }
                settings.Cities.Save(citys.ToArray());
            });
        }

        internal async void Pin(IList<object> selectedItems)
        {
            if (selectedItems == null || selectedItems.Count == 0)
            {
                return;
            }

            foreach (CityViewModel item in selectedItems)
            {
                SecondaryTile s = new SecondaryTile(item.Id, item.City, item.Id, new Uri("ms-appx:///Assets/Square150x150Logo.png"), TileSize.Default);
                var elem = s.VisualElements;
                elem.ShowNameOnSquare150x150Logo = true;
                elem.ShowNameOnSquare310x310Logo = true;
                elem.ShowNameOnWide310x150Logo = true;
                elem.Square310x310Logo = new Uri("ms-appx:///Assets/Square310x310Logo.png");
                elem.Square44x44Logo = new Uri("ms-appx:///Assets/Square44x44Logo.png");
                elem.Square71x71Logo = new Uri("ms-appx:///Assets/Square71x71Logo.png");
                elem.Wide310x150Logo = new Uri("ms-appx:///Assets/Wide310x150Logo.png");
                await s.RequestCreateAsync();
                var index = Array.FindIndex(settings.Cities.SavedCities, x =>
                {
                    return x.Id == item.Id;
                });
                CitySettingsModel currentCity;
                if (index == -1 && settings.Cities.LocatedCity != null)
                {
                    settings.Cities.LocatedCity.IsPinned = true;
                    currentCity = settings.Cities.LocatedCity;
                }
                else
                {
                    settings.Cities.SavedCities[index].IsPinned = true;
                    currentCity = settings.Cities.SavedCities[index];
                }
                var t = ThreadPool.RunAsync(async (work) =>
                {
                    string resstr;
                    try
                    {
                        resstr = await settings.Cities.ReadDataAsync(item.Id.IsNullorEmpty() ? item.City : item.Id, settings.Preferences.DataSource);
                    }
                    catch (Exception)
                    {
                        resstr = await Request.GetRequestAsync(settings, item.Id, item.City, item.longitude, item.latitude, item.zmw);
                    }
                    if (!resstr.IsNullorEmpty())
                    {
                        var weather = HeWeatherModel.Generate(resstr, settings.Preferences.DataSource);
                        if (weather == null || weather.DailyForecast == null || weather.HourlyForecast == null)
                        {
                            return;
                        }
                        var utcOffset = weather.Location.UpdateTime - weather.Location.UtcTime;
                        var current = DateTimeHelper.ReviseLoc(utcOffset);
                        var todayIndex = Array.FindIndex(weather.DailyForecast, x =>
                        {
                            return x.Date.Date == current.Date;
                        });
                        var hourIndex = Array.FindIndex(weather.HourlyForecast, x =>
                        {
                            return (x.DateTime - current).TotalSeconds > 0;
                        });
                        if (todayIndex < 0)
                        {
                            todayIndex = 0;
                        }
                        if (hourIndex < 0)
                        {
                            hourIndex = 0;
                        }
                        var isNight = Generator.CalcIsNight(weather.Location.UpdateTime, weather.DailyForecast[todayIndex].SunRise, weather.DailyForecast[todayIndex].SunSet, new Models.Location(currentCity.Latitude, currentCity.Longitude));
                        var glanceFull = Glance.GenerateGlanceDescription(weather, isNight, settings.Preferences.TemperatureParameter, DateTime.Now);
                        var glance = Glance.GenerateShortDescription(weather, isNight);
                        var uri = await settings.Immersive.GetCurrentBackgroundAsync(weather.NowWeather.Now.Condition, isNight);
                        Sender.CreateSubTileNotification(await Generator.CreateAll(currentCity, weather, current), item.Id);
                    }
                });
            }
        }

        private void UpdateLocated()
        {
            var t = ThreadPool.RunAsync(async (work) =>
            {
                string resstr = await Request.GetRequestAsync(settings, settings.Cities.LocatedCity);
                if (!resstr.IsNullorEmpty())
                {
                    Cities[0].data = resstr;
                    await settings.Cities.SaveDataAsync(Cities[0].Id.IsNullorEmpty() ? Cities[0].City : Cities[0].Id, resstr, settings.Preferences.DataSource);
                    settings.Cities.LocatedCity.Update();
                    settings.Cities.Save();
                    await Complete(Cities[0]);
                }
            });
        }

        internal void ChangeCurrent(int selectedIndex)
        {
            if (settings.Cities.LocatedCity != null && settings.Cities.EnableLocate)
            {
                settings.Cities.CurrentIndex = selectedIndex - 1;
            }
            else
            {
                settings.Cities.CurrentIndex = selectedIndex;
            }
            settings.Cities.Save();
        }

        private async Task Complete(CityViewModel item)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(async () =>
            {
                if (item.data != null && item.data.Length > 31)
                {
                    var weather = HeWeatherModel.Generate(item.data, settings.Preferences.DataSource);
                    if (weather == null || weather.DailyForecast == null || weather.HourlyForecast == null)
                        return;
                    await itemInit(item, weather);
                    item.Updated = true;
                    item.Succeed = true;
                    item.LastUpdate = DateTime.Now;
                }
            }));
        }

        private async Task itemInit(CityViewModel item, HeWeatherModel weather)
        {
            item.NowCondition = weather.NowWeather.Now.Condition;
            item.Temperature = weather.NowWeather.Temprature;
            string p = TemperatureDecoratorConverter.GetCurrentDeco();
            item.Decorator = p;
            var utcOffset = weather.Location.UpdateTime - weather.Location.UtcTime;
            var current = DateTimeHelper.ReviseLoc(utcOffset);
            var todayIndex = Array.FindIndex(weather.DailyForecast, x =>
            {
                return x.Date.Date == current.Date;
            });
            var hourIndex = Array.FindIndex(weather.HourlyForecast, x =>
            {
                return (x.DateTime - current).TotalSeconds > 0;
            });
            if (todayIndex < 0)
            {
                todayIndex = 0;
            }
            if (hourIndex < 0)
            {
                hourIndex = 0;
            }
            item.High = weather.DailyForecast[todayIndex].HighTemp;
            item.Low = weather.DailyForecast[todayIndex].LowTemp;
            var isNight = Generator.CalcIsNight(weather.Location.UpdateTime, weather.DailyForecast[todayIndex].SunRise, weather.DailyForecast[todayIndex].SunSet, new Models.Location(item.latitude, item.longitude));
            item.Glance = Glance.GenerateGlanceDescription(weather, isNight, settings.Preferences.TemperatureParameter, DateTime.Now);
            var uri = await settings.Immersive.GetCurrentBackgroundAsync(weather.NowWeather.Now.Condition, isNight);
            if (uri != null)
            {
                try
                {
                    item.Background = new BitmapImage(uri);
                }
                catch (Exception)
                {
                }
            }

            item.data = null;
        }

        private async Task Init()
        {
            foreach (var item in Cities)
            {
                if (item.data != null && item.data.Length > 31)
                {
                    var weather = HeWeatherModel.Generate(item.data, settings.Preferences.DataSource);
                    if (weather == null || weather.DailyForecast == null || weather.HourlyForecast == null)
                    {
                        return;
                    }
                    await itemInit(item, weather);
                    item.Updated = true;
                    item.Succeed = true;
                }
            }
            if (settings.Cities.EnableLocate && settings.Cities.LocatedCity != null)
            {
                CurrentIndex = settings.Cities.CurrentIndex + 1;
            }
            else
            {
                CurrentIndex = settings.Cities.CurrentIndex;
            }
        }

        private async Task SearchExistingDataAsync()
        {
            foreach (var item in Cities)
            {
                var currentTime = DateTime.Now;
                if ((currentTime - item.LastUpdate).TotalMinutes < 60)
                {
                    try
                    {
                        var data = await settings.Cities.ReadDataAsync(item.Id.IsNullorEmpty() ? item.City : item.Id, settings.Preferences.DataSource);
                        item.data = data;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
        }

        internal void SetPanelWidth(double v)
        {
            foreach (var item in Cities)
            {
                item.PanelWidth = v;
            }
        }

        private void RequireLocationUpdate()
        {
            this.LocationUpdate?.Invoke(this, new LocationUpdateEventArgs());
        }

        private void OnFetchDataFailed()
        {
            this.FetchDataFailed?.Invoke(this, new FetchDataFailedEventArgs("Cities_Null"));
        }

        internal void Pin(string v)
        {
            throw new NotImplementedException();
        }

        internal void ReloadCity()
        {
            settings = SettingsModel.Current;
            Cities.Clear();
            if (settings.Cities.EnableLocate)
            {
                if (settings.Cities.LocatedCity != null)
                    Cities.Add(new CityViewModel(settings.Cities.LocatedCity, true));
            }
            foreach (var city in settings.Cities.SavedCities)
            {
                Cities.Add(new CityViewModel(city, false));
            }
            Cities.Add(new CityViewModel());
            if (Cities.IsNullorEmpty())
            {
                var t = ThreadPool.RunAsync(async (work) =>
                {
                    await Task.Delay(1000);
                    this.OnFetchDataFailed();
                });
                return;
            }
            var task = ThreadPool.RunAsync(async (work) =>
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(async () =>
                {
                    await Init();
                    var t = ThreadPool.RunAsync((x) =>
                    {
                        Update();
                    });
                }));
            });
        }
    }

    public class Cities : ObservableCollection<CityViewModel>
    {

    }

    public class CityViewModel : ViewModelBase
    {
        private string city;
        private string id;
        private WeatherCondition nowCondition;
        private bool updated;
        private bool succeed = true;
        private DateTime lastUpdate;
        internal string data;
        private BitmapImage background;
        private string glance;
        public float longitude;
        public float latitude;
        internal string zmw;
        private Temperature temp;

        private bool empty = false;

        public bool Empty
        {
            get { return empty; }
            set { SetProperty(ref empty, value); }
        }


        private string locate = " ";

        public string Locate
        {
            get { return locate; }
            set { SetProperty(ref locate, value); }
        }


        private Temperature high;

        public Temperature High
        {
            get { return high; }
            set { SetProperty(ref high, value); }
        }

        private Temperature low;

        public Temperature Low
        {
            get { return low; }
            set { SetProperty(ref low, value); }
        }


        public Temperature Temperature
        {
            get { return temp; }
            set { SetProperty(ref temp, value); }
        }

        private string deco;

        public string Decorator
        {
            get { return deco; }
            set { SetProperty(ref deco, value); }
        }

        private double panelWidth = 320d;

        public double PanelWidth
        {
            get { return panelWidth; }
            set { SetProperty(ref panelWidth, value); }
        }


        public CityViewModel(CitySettingsModel city, bool isLocate)
        {
            Id = city.Id;
            City = city.City;
            LastUpdate = city.LastUpdate;
            longitude = city.Longitude;
            latitude = city.Latitude;
            zmw = city.ZMW;
            if (isLocate)
                Locate = "";
            Empty = false;
        }

        public CityViewModel()
        {
            Empty = true;
        }

        public string City
        {
            get
            {
                return city;
            }
            set
            {
                SetProperty(ref city, value);
            }
        }
        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                SetProperty(ref id, value);
            }
        }
        public WeatherCondition NowCondition
        {
            get
            {
                return nowCondition;
            }
            set
            {
                SetProperty(ref nowCondition, value);
            }
        }
        public bool Updated
        {
            get
            {
                return updated;
            }
            set
            {
                SetProperty(ref updated, value);
            }
        }
        public bool Succeed
        {
            get
            {
                return succeed;
            }
            set
            {
                SetProperty(ref succeed, value);
            }
        }
        public DateTime LastUpdate
        {
            get
            {
                return lastUpdate;
            }

            set
            {
                SetProperty(ref lastUpdate, value);
            }
        }

        public BitmapImage Background
        {
            get
            {
                return background;
            }

            set
            {
                SetProperty(ref background, value);
            }
        }

        public string Glance
        {
            get
            {
                return glance;
            }

            set
            {
                SetProperty(ref glance, value);
            }
        }
    }
}
