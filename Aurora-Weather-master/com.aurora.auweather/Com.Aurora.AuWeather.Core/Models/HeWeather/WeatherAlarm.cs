﻿// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
namespace Com.Aurora.AuWeather.Models.HeWeather
{
    public class WeatherAlarm
    {
        private static readonly string[] TypeParseString = { "台风", "暴雨", "暴雪", "寒潮", "大风", "沙尘暴", "高温",
            "干旱", "雷电", "冰雹", "霜冻", "大雾", "霾", "道路结冰", "森林火险", "雷雨大风" };
        private static readonly string[] LevelParseString = { "蓝", "黄", "橙", "红" };
        private static WeatherAlarmType ParseType(string alarm_s)
        {
            byte i = 0;
            foreach (var s in TypeParseString)
            {
                if (alarm_s.Contains(s))
                {
                    return WeatherAlarmType.typhoon + i;
                }
                i++;
            }
            throw new ArgumentException("Value can't be null.");
        }
        private static WeatherAlarmLevel ParseLevel(string alarm_s)
        {
            byte i = 0;
            foreach (var s in LevelParseString)
            {
                if (alarm_s.Contains(s))
                {
                    return WeatherAlarmLevel.blue + i;
                }
                i++;
            }
            throw new ArgumentException("Value can't be null.");
        }

        public WeatherAlarmLevel Level
        {
            get; private set;
        }

        public WeatherAlarmType Type
        {
            get; private set;
        }

        public string Title
        {
            get; private set;
        }

        public string Text
        {
            get; private set;
        }

        public WeatherAlarm(JsonContract.WeatherAlarmContract alarm)
        {
            if (alarm == null)
            {
                return;
            }
            try
            {
                Level = ParseLevel(alarm.level);
                Type = ParseType(alarm.type);
                Title = alarm.title;
                Text = alarm.txt;
            }
            catch (Exception)
            {
                return;
            }

        }
    }
}