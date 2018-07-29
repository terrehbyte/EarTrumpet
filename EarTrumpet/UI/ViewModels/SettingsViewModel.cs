﻿using EarTrumpet.Extensions;
using EarTrumpet.Interop.Helpers;
using EarTrumpet.UI.Helpers;
using EarTrumpet.UI.Services;
using System;
using System.Windows.Input;
using Windows.ApplicationModel;

namespace EarTrumpet.UI.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        private HotkeyData _hotkey;

        internal HotkeyData Hotkey
        {
            get => _hotkey;
            set
            {
                _hotkey = value;
                SettingsService.Hotkey = _hotkey;
                RaisePropertyChanged(nameof(Hotkey));
                RaisePropertyChanged(nameof(HotkeyText));
            }
        }

        public event Func<HotkeyData, HotkeyData> RequestHotkey;

        public string Title => Properties.Resources.SettingsWindowText;
        public string HotkeyText => _hotkey.ToString();
        public string DefaultHotKey => SettingsService.s_defaultHotkey.ToString();
        public RelayCommand OpenDiagnosticsCommand { get; }
        public RelayCommand OpenAboutCommand { get; }
        public RelayCommand OpenFeedbackCommand { get; }
        public RelayCommand SelectHotkey { get; }
        public RelayCommand OpenAddonManager { get; set; }

        public bool UseLegacyIcon
        {
            get => SettingsService.UseLegacyIcon;
            set => SettingsService.UseLegacyIcon = value;
        }

        public string AboutText { get; private set; }

        internal SettingsViewModel()
        {
            Hotkey = SettingsService.Hotkey;
            OpenAboutCommand = new RelayCommand(OpenAbout);
            OpenDiagnosticsCommand = new RelayCommand(OpenDiagnostics);
            OpenFeedbackCommand = new RelayCommand(FeedbackService.OpenFeedbackHub);
            SelectHotkey = new RelayCommand(OnSelectHotkey);

            string aboutFormat = "EarTrumpet {0}";
            if (App.Current.HasIdentity())
            {
                AboutText = string.Format(aboutFormat, Package.Current.Id.Version.ToVersionString());
            }
            else
            {
                AboutText = string.Format(aboutFormat, "0.0.0.0");
            }
        }

        private void OpenDiagnostics()
        {
            if(Keyboard.IsKeyDown(Key.LeftShift) && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                throw new Exception("This is an intentional crash.");
            }

            DiagnosticsService.DumpAndShowData();
        }

        private void OpenAbout()
        {
            ProcessHelper.StartNoThrow("https://github.com/File-New-Project/EarTrumpet");
        }

        private void OnSelectHotkey()
        {
            var ret = RequestHotkey.Invoke(Hotkey);
            if (ret != null)
            {
                HotkeyManager.Current.Unregister(Hotkey);
                Hotkey = ret;
                HotkeyManager.Current.Register(Hotkey);
            }
        }
    }
}