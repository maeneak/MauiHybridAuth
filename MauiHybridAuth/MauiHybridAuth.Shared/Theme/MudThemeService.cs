using MudBlazor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiHybridAuth.Shared.Theme
{
    public class MudThemeService : INotifyPropertyChanged
    {
        ThemeColorMode _currentMode = ThemeColorMode.System;
        bool _systemInDarkMode;
        public bool InDarkMode { 
            get {
                return CurrentMode == ThemeColorMode.System ? SystemInDarkMode : CurrentMode == ThemeColorMode.Dark;
            }
            set
            {
                _currentMode = value ? ThemeColorMode.Dark : ThemeColorMode.Light;
            }
        }
        public MudTheme CurrentTheme { get; set; } = new ();
        public bool SystemInDarkMode
        {
            get { return _systemInDarkMode; }
            set
            {
                _systemInDarkMode = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InDarkMode)));
            }
        }
        public ThemeColorMode CurrentMode {
            get { return _currentMode; }
            set {
                _currentMode = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InDarkMode)));
            } 
        }
        public string Icon { 
            get {
                return CurrentMode == ThemeColorMode.System ? MudBlazor.Icons.Material.Filled.AutoMode :
                    InDarkMode ? MudBlazor.Icons.Material.Filled.DarkMode : MudBlazor.Icons.Material.Filled.LightMode;
            } 
        }

        public void ToggleMode()
        {
            CurrentMode = CurrentMode == ThemeColorMode.System ? ThemeColorMode.Light : CurrentMode == ThemeColorMode.Light ? ThemeColorMode.Dark : ThemeColorMode.System;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

    }
    public enum ThemeColorMode
    {
        Light,
        Dark,
        System = -1
    }


}
