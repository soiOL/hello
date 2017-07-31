﻿// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Com.Aurora.AuWeather.Models
{
    public class Speed
    {
        private double kmph;

        public double KMPH
        {
            get
            {
                return kmph;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                kmph = value;
            }
        }

        public double MPS
        {
            get
            {
                return kmph / 3.6;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                kmph = value * 3.6;
            }
        }

        public double Knot
        {
            get
            {
                return kmph / 1.852;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                kmph = value * 1.852;
            }
        }

        public static Speed FromKMPH(double kmph)
        {
            var speed = new Speed();
            speed.KMPH = kmph;
            return speed;
        }

        public static Speed FromMPS(double mps)
        {
            var speed = new Speed();
            speed.MPS = mps;
            return speed;
        }

        public static Speed FromKnot(double knot)
        {
            var speed = new Speed();
            speed.Knot = knot;
            return speed;
        }

        internal string Actual(SpeedParameter windParameter)
        {
            switch (windParameter)
            {
                case SpeedParameter.KMPH:
                    return KMPH.ToString("0.#");
                case SpeedParameter.MPS:
                    return MPS.ToString("0.#");
                case SpeedParameter.Knot:
                    return Knot.ToString("0.#");
                default:
                    return KMPH.ToString("0.#");
            }
        }

        internal string DanWei(SpeedParameter speedParameter)
        {
            switch (speedParameter)
            {
                case SpeedParameter.KMPH:
                    return "km/h";
                case SpeedParameter.MPS:
                    return "m/s";
                case SpeedParameter.Knot:
                    return "knot";
                default:
                    return "km/h";
            }
        }

        public double ActualDouble(SpeedParameter speedParameter)
        {
            switch (speedParameter)
            {
                case SpeedParameter.KMPH:
                    return KMPH;
                case SpeedParameter.MPS:
                    return MPS;
                case SpeedParameter.Knot:
                    return Knot;
                default:
                    return 0;
            }
        }
    }
}
