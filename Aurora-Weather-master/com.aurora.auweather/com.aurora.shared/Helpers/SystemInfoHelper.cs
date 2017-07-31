﻿using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources.Core;
using Windows.System.Profile;
using Windows.UI.ViewManagement;

namespace Com.Aurora.Shared.Helpers
{
    public static class SystemInfoHelper
    {
        public static IEnumerable<string> GetSystemInfo()
        {
            var sv = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong v = ulong.Parse(sv);
            ulong v1 = (v & 0xFFFF000000000000L) >> 48;
            ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
            ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
            ulong v4 = (v & 0x000000000000FFFFL);
            string sysVer = $"{v1}.{v2}.{v3}.{v4}";
            var eascdi = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();
            var resources = ResourceContext.GetForCurrentView();
            var lang = resources.QualifierValues["Language"];
            var region = resources.QualifierValues["HomeRegion"];
            var ver = Package.Current.Id.Version.Major.ToString("0") + "." +
               Package.Current.Id.Version.Minor.ToString("0") + "." +
               Package.Current.Id.Version.Build.ToString("0");
            return new string[] { "Version = " + sysVer, "Package = " + ver, "Language = " + lang, "HomeRegion = " + region,
            "EAS ID = " + eascdi.Id, "Friendly Name = " + eascdi.FriendlyName, "OS = " + eascdi.OperatingSystem, "Firmware = " + eascdi.SystemFirmwareVersion,
            "Hardware = " + eascdi.SystemHardwareVersion, "Manufacturer = " + eascdi.SystemManufacturer, "Product Name = " + eascdi.SystemProductName,
            "SKU = " + eascdi.SystemSku };

        }

        public static string GetPackageVer()
        {
#if BETA
            return Package.Current.Id.Version.Major.ToString("0") + "." +
               Package.Current.Id.Version.Minor.ToString("0") + "." +
               Package.Current.Id.Version.Build.ToString("0")+" - BETA";
#else
            return Package.Current.Id.Version.Major.ToString("0") + "." +
               Package.Current.Id.Version.Minor.ToString("0") + "." +
               Package.Current.Id.Version.Build.ToString("0");
#endif
        }

        public static ulong GetPackageVersionNum()
        {
            return ((ulong)Package.Current.Id.Version.Major << 32) + ((ulong)Package.Current.Id.Version.Minor << 16) + Package.Current.Id.Version.Build;
        }


        public static DeviceFormFactorType GetDeviceFormFactorType()

        {

            switch (AnalyticsInfo.VersionInfo.DeviceFamily)

            {

                case "Windows.Mobile":

                    return DeviceFormFactorType.Phone;

                case "Windows.Desktop":

                    return UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Mouse

                        ? DeviceFormFactorType.Desktop

                        : DeviceFormFactorType.Tablet;

                case "Windows.Universal":

                    return DeviceFormFactorType.IoT;

                case "Windows.Team":

                    return DeviceFormFactorType.SurfaceHub;

                default:

                    return DeviceFormFactorType.Other;

            }
        }
    }

    public enum DeviceFormFactorType

    {

        Phone,

        Desktop,

        Tablet,

        IoT,

        SurfaceHub,

        Other

    }
}
