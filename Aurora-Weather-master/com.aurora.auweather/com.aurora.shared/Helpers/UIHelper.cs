﻿// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Com.Aurora.Shared.Helpers
{
    public static class UIHelper
    {
        public static void Change_Row_Column(FrameworkElement d, int row, int column)
        {
            d.SetValue(Grid.RowProperty, row);
            d.SetValue(Grid.ColumnProperty, column);
        }

        public static void ReverseVisibility(FrameworkElement e)
        {
            if (e.Visibility == Visibility.Collapsed)
            {
                e.Visibility = Visibility.Visible;
            }
            else
            {
                e.Visibility = Visibility.Collapsed;
            }
        }

        public static void ChangeTitlebarButtonColor(Color backGround,Color foreGround)
        {
            var view = ApplicationView.GetForCurrentView();
            var otherB = backGround;
            var otherF = foreGround;
            view.TitleBar.ButtonBackgroundColor = backGround;
            view.TitleBar.ButtonForegroundColor = foreGround;
        }
    }
}
