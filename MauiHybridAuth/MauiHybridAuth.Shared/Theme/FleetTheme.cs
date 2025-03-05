﻿using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiHybridAuth.Shared.Theme
{
    public class FleetTheme: CustomAppTheme
    {
       public FleetTheme() {
            PaletteLight = new PaletteLight()
            {
                Black = "rgba(75,143,226,1)",
                White = "rgba(255,255,255,1)",
                Primary = "rgba(69,134,224,1)",
                PrimaryContrastText = "rgba(255,255,255,1)",
                Secondary = "rgba(255,64,129,1)",
                SecondaryContrastText = "rgba(255,255,255,1)",
                Tertiary = "rgba(30,200,165,1)",
                TertiaryContrastText = "rgba(255,255,255,1)",
                Info = "rgba(33,150,243,1)",
                InfoContrastText = "rgba(255,255,255,1)",
                Success = "rgba(0,200,83,1)",
                SuccessContrastText = "rgba(255,255,255,1)",
                Warning = "rgba(255,152,0,1)",
                WarningContrastText = "rgba(255,255,255,1)",
                Error = "rgba(244,67,54,1)",
                ErrorContrastText = "rgba(255,255,255,1)",
                Dark = "rgba(66,66,66,1)",
                DarkContrastText = "rgba(255,255,255,1)",
                TextPrimary = "rgba(66,66,66,1)",
                TextSecondary = "rgba(0,0,0,0.5372549019607843)",
                TextDisabled = "rgba(0,0,0,0.3764705882352941)",
                ActionDefault = "rgba(0,0,0,0.5372549019607843)",
                ActionDisabled = "rgba(0,0,0,0.25882352941176473)",
                ActionDisabledBackground = "rgba(0,0,0,0.11764705882352941)",
                Background = "rgba(255,255,255,1)",
                BackgroundGray = "rgba(245,245,245,1)",
                Surface = "rgba(255,255,255,1)",
                DrawerBackground = "rgba(255,255,255,1)",
                DrawerText = "rgba(66,66,66,1)",
                DrawerIcon = "rgba(97,97,97,1)",
                AppbarBackground = "rgba(75,125,226,1)",
                AppbarText = "rgba(255,255,255,1)",
                LinesDefault = "rgba(0,0,0,0.11764705882352941)",
                LinesInputs = "rgba(189,189,189,1)",
                TableLines = "rgba(224,224,224,1)",
                TableStriped = "rgba(0,0,0,0.0196078431372549)",
                TableHover = "rgba(0,0,0,0.0392156862745098)",
                Divider = "rgba(224,224,224,1)",
                DividerLight = "rgba(0,0,0,0.8)",
                PrimaryDarken = "rgba(27,105,220,1)",
                PrimaryLighten = "rgba(106,168,231,1)",
                SecondaryDarken = "rgb(255,31,105)",
                SecondaryLighten = "rgb(255,102,153)",
                TertiaryDarken = "rgb(25,169,140)",
                TertiaryLighten = "rgb(42,223,187)",
                InfoDarken = "rgb(12,128,223)",
                InfoLighten = "rgb(71,167,245)",
                SuccessDarken = "rgb(0,163,68)",
                SuccessLighten = "rgb(0,235,98)",
                WarningDarken = "rgb(214,129,0)",
                WarningLighten = "rgb(255,167,36)",
                ErrorDarken = "rgb(242,28,13)",
                ErrorLighten = "rgb(246,96,85)",
                DarkDarken = "rgb(46,46,46)",
                DarkLighten = "rgb(87,87,87)",
                HoverOpacity = 0.06,
                RippleOpacity = 0.1,
                RippleOpacitySecondary = 0.2,
                GrayDefault = "#9E9E9E",
                GrayLight = "#BDBDBD",
                GrayLighter = "#E0E0E0",
                GrayDark = "#757575",
                GrayDarker = "#616161",
                OverlayDark = "rgba(33,33,33,0.4980392156862745)",
                OverlayLight = "rgba(255,255,255,0.4980392156862745)",
            };
            PaletteDark = new PaletteDark()
            {
                Black = "rgba(39,39,47,1)",
                Primary = "rgba(106,162,231,1)",
                Info = "rgba(50,153,255,1)",
                Success = "rgba(11,186,131,1)",
                Warning = "rgba(255,168,0,1)",
                Error = "rgba(246,78,98,1)",
                Dark = "rgba(39,39,47,1)",
                TextPrimary = "rgba(255,255,255,0.6980392156862745)",
                TextSecondary = "rgba(255,255,255,0.4980392156862745)",
                TextDisabled = "rgba(255,255,255,0.2)",
                ActionDefault = "rgba(173,173,177,1)",
                ActionDisabled = "rgba(255,255,255,0.25882352941176473)",
                ActionDisabledBackground = "rgba(255,255,255,0.11764705882352941)",
                Background = "rgba(50,51,61,1)",
                BackgroundGray = "rgba(39,39,47,1)",
                Surface = "rgba(55,55,64,1)",
                DrawerBackground = "rgba(39,39,47,1)",
                DrawerText = "rgba(255,255,255,0.4980392156862745)",
                DrawerIcon = "rgba(255,255,255,0.4980392156862745)",
                AppbarBackground = "rgba(39,39,47,1)",
                AppbarText = "rgba(255,255,255,0.6980392156862745)",
                LinesDefault = "rgba(255,255,255,0.11764705882352941)",
                LinesInputs = "rgba(255,255,255,0.2980392156862745)",
                TableLines = "rgba(255,255,255,0.11764705882352941)",
                TableStriped = "rgba(255,255,255,0.2)",
                Divider = "rgba(255,255,255,0.11764705882352941)",
                DividerLight = "rgba(255,255,255,0.058823529411764705)",
                PrimaryDarken = "rgba(59,125,225,1)",
                PrimaryLighten = "rgba(118,166,236,1)",
                InfoDarken = "rgb(10,133,255)",
                InfoLighten = "rgb(92,173,255)",
                SuccessDarken = "rgb(9,154,108)",
                SuccessLighten = "rgb(13,222,156)",
                WarningDarken = "rgb(214,143,0)",
                WarningLighten = "rgb(255,182,36)",
                ErrorDarken = "rgb(244,47,70)",
                ErrorLighten = "rgb(248,119,134)",
                DarkDarken = "rgb(23,23,28)",
                DarkLighten = "rgb(56,56,67)",
            };
            LayoutProperties = new LayoutProperties()
            {
                DefaultBorderRadius = "4px",
                DrawerMiniWidthLeft = "56px",
                DrawerMiniWidthRight = "56px",
                DrawerWidthLeft = "240px",
                DrawerWidthRight = "240px",
                AppbarHeight = "64px",
            };
            Typography = new Typography()
            {
                Default = new DefaultTypography
                {
                    FontFamily = ["Roboto", "Helvetica", "Arial", "sans-serif"],
                    FontWeight = "400",
                    FontSize = ".875rem",
                    LineHeight = "1.43",
                    LetterSpacing = ".01071em",
                    TextTransform = "none",
                },
                H1 = new H1Typography
                {
                    FontWeight = "300",
                    FontSize = "6rem",
                    LineHeight = "1.167",
                    LetterSpacing = "-.01562em",
                    TextTransform = "none",
                },
                H2 = new H2Typography
                {
                    FontWeight = "300",
                    FontSize = "3.75rem",
                    LineHeight = "1.2",
                    LetterSpacing = "-.00833em",
                    TextTransform = "none",
                },
                H3 = new H3Typography
                {
                    FontWeight = "400",
                    FontSize = "3rem",
                    LineHeight = "1.167",
                    LetterSpacing = "0",
                    TextTransform = "none",
                },
                H4 = new H4Typography
                {
                    FontWeight = "400",
                    FontSize = "2.125rem",
                    LineHeight = "1.235",
                    LetterSpacing = ".00735em",
                    TextTransform = "none",
                },
                H5 = new H5Typography
                {
                    FontWeight = "400",
                    FontSize = "1.5rem",
                    LineHeight = "1.334",
                    LetterSpacing = "0",
                    TextTransform = "none",
                },
                H6 = new H6Typography
                {
                    FontWeight = "500",
                    FontSize = "1.25rem",
                    LineHeight = "1.6",
                    LetterSpacing = ".0075em",
                    TextTransform = "none",
                },
                Subtitle1 = new Subtitle1Typography
                {
                    FontWeight = "400",
                    FontSize = "1rem",
                    LineHeight = "1.75",
                    LetterSpacing = ".00938em",
                    TextTransform = "none",
                },
                Subtitle2 = new Subtitle2Typography
                {
                    FontWeight = "500",
                    FontSize = ".875rem",
                    LineHeight = "1.57",
                    LetterSpacing = ".00714em",
                    TextTransform = "none",
                },
                Body1 = new Body1Typography
                {
                    FontWeight = "400",
                    FontSize = "1rem",
                    LineHeight = "1.5",
                    LetterSpacing = ".00938em",
                    TextTransform = "none",
                },
                Body2 = new Body2Typography
                {
                    FontWeight = "400",
                    FontSize = ".875rem",
                    LineHeight = "1.43",
                    LetterSpacing = ".01071em",
                    TextTransform = "none",
                },
                Button = new ButtonTypography
                {
                    FontWeight = "500",
                    FontSize = ".875rem",
                    LineHeight = "1.75",
                    LetterSpacing = ".02857em",
                    TextTransform = "uppercase",
                },
                Caption = new CaptionTypography
                {
                    FontWeight = "400",
                    FontSize = ".75rem",
                    LineHeight = "1.66",
                    LetterSpacing = ".03333em",
                    TextTransform = "none",
                },
                Overline = new OverlineTypography
                {
                    FontWeight = "400",
                    FontSize = ".75rem",
                    LineHeight = "2.66",
                    LetterSpacing = ".08333em",
                    TextTransform = "none",
                },
            };
            ZIndex = new ZIndex()
            {
                Drawer = 1100,
                Popover = 1200,
                AppBar = 1300,
                Dialog = 1400,
                Snackbar = 1500,
                Tooltip = 1600,
            };
        }
    }
}
