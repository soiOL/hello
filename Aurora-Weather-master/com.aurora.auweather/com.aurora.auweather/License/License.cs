﻿// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;

namespace Com.Aurora.AuWeather.License
{
    internal class License
    {
#if DEBUG
        public static readonly string[] DonationPack = new string[]
        {
            "1","2","1","2","1","2","1","1","2","1","2","1","2","1"
        };
#else
        public static readonly string[] DonationPack = new string[]
        {
            "Donation0","Donation1","Donation2","Donation3","Donation4","Donation5","Donation6", "Donation7","Donation8","Donation9","Donation10","Donation11","Donation12","Donation13"
        };
#endif
        private LicenseInformation licenseInformation;
        private LicenseChangedEventHandler licenseChangeHandler;

        public bool IsPurchased
        {
            get
            {
#if BETA
                return true;
#else
                foreach (var item in DonationPack)
                {
                    if (licenseInformation.ProductLicenses[item].IsActive)
                    {
                        return true;
                    }
                    else
                    {
                        continue;
                    }
                }
                return false;
            
#endif
            }
        }

        internal async void GetPrice()
        {
            try
            {
#if DEBUG
                ListingInformation listing = await CurrentAppSimulator.LoadListingInformationAsync();
#else
                ListingInformation listing = await CurrentApp.LoadListingInformationAsync();
#endif
                List<string> ps = new List<string>();
                foreach (var item in DonationPack)
                {
                    var product = listing.ProductListings[item];
                    ps.Add(product.FormattedPrice);
                }
                Price = ps.ToArray();

                licenseChangeHandler = new LicenseChangedEventHandler(licenseChangedEventHandler);
#if DEBUG
                CurrentAppSimulator.LicenseInformation.LicenseChanged += licenseChangeHandler;
#else
                CurrentApp.LicenseInformation.LicenseChanged += licenseChangeHandler;
#endif
            }
            catch (Exception)
            {
                Price = null;
            }
        }

        public string[] Price
        {
            get;
            private set;
        }
        public string Name { get; private set; }

        public event LicenseChangedEventHandler LicenseChanged;

        public License()
        {
            Reload();
        }

        private void Reload()
        {
#if DEBUG
            licenseInformation = CurrentAppSimulator.LicenseInformation;
#else
            licenseInformation = CurrentApp.LicenseInformation;
#endif

        }

        private void licenseChangedEventHandler()
        {
            this.OnLicenseChanged();
        }

        private void OnLicenseChanged()
        {
            this.LicenseChanged?.Invoke();
        }

        internal async Task TryPurchaseAsync(string donationPack)
        {
#if DEBUG
            await CurrentAppSimulator.RequestProductPurchaseAsync(donationPack);
#else
            await CurrentApp.RequestProductPurchaseAsync(donationPack);
#endif
        }
    }
}
