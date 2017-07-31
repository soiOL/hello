﻿// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Com.Aurora.AuWeather.Core.Models.Caiyun.JsonContract;

namespace Com.Aurora.AuWeather.Models
{

    public class AQI
    {

        public AQI(HeWeather.JsonContract.AQIContract aqi)
        {
            if (aqi == null)
            {
                return;
            }
            try
            {
                uint m;
                if (uint.TryParse(aqi.city.aqi, out m))
                {
                    Aqi = m;
                }
                if (uint.TryParse(aqi.city.co, out m))
                {
                    Co = m;
                }
                if (uint.TryParse(aqi.city.no2, out m))
                {
                    No2 = m;
                }
                if (uint.TryParse(aqi.city.o3, out m))
                {
                    O3 = m;
                }
                if (uint.TryParse(aqi.city.pm10, out m))
                {
                    Pm10 = m;
                }
                if (uint.TryParse(aqi.city.pm25, out m))
                {
                    Pm25 = m;
                }
                if (uint.TryParse(aqi.city.so2, out m))
                {
                    So2 = m;
                }

            }
            catch (Exception)
            {
                
            }
            finally
            {
                Qlty = ParseQlty(aqi.city.qlty);
                if (Qlty == AQIQuality.unknown && Aqi != default(uint))
                {
                    Qlty = CalcQlty(Aqi);
                }
            }
        }

        public AQI(Value aqi, Value pm25)
        {
            if (aqi == null)
                return;
            Aqi = (uint)aqi.value;
            Pm25 = (uint)pm25.value;
            Qlty = CalcQlty((uint)aqi.value);
        }

        private AQIQuality CalcQlty(uint value)
        {
            if (value <= 50 && value >= 0)
            {
                return AQIQuality.one;
            }
            else if (value <= 100)
            {
                return AQIQuality.two;
            }
            else if (value <= 150)
            {
                return AQIQuality.three;
            }
            else if (value <= 200)
            {
                return AQIQuality.four;
            }
            else if (value <= 250)
            {
                return AQIQuality.five;
            }
            else
            {
                return AQIQuality.six;
            }
        }

        private static AQIQuality ParseQlty(string qlty)
        {
            switch (qlty)
            {
                default: return AQIQuality.unknown;
                case "优": return AQIQuality.one;
                case "良": return AQIQuality.two;
                case "轻度污染": return AQIQuality.three;
                case "中度污染": return AQIQuality.four;
                case "重度污染": return AQIQuality.five;
                case "严重污染": return AQIQuality.six;
            }
        }

        public uint Aqi
        {
            get; private set;
        }

        public uint Co
        {
            get; private set;
        }

        public uint No2
        {
            get; private set;
        }

        public uint O3
        {
            get; private set;
        }

        public uint Pm10
        {
            get; private set;
        }

        public uint Pm25
        {
            get; private set;
        }

        public uint So2
        {
            get; private set;
        }

        public AQIQuality Qlty
        {
            get; private set;
        }
    }
}