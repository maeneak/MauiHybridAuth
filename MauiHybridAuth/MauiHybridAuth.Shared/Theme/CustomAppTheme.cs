using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiHybridAuth.Shared.Theme
{
    public class CustomAppTheme : MudBlazor.MudTheme, INotifyPropertyChanged
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
        public event PropertyChangedEventHandler? PropertyChanged;
    }
    public enum ThemeColorMode
    {
        Light,
        Dark,
        System = -1
    }
}
