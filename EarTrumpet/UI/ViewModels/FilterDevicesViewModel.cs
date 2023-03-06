using EarTrumpet.Interop.Helpers;
using EarTrumpet.UI.Helpers;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EarTrumpet.UI.ViewModels
{
    class FilterDevicesViewModel : SettingsPageViewModel
    {
        private readonly AppSettings _settings;

        public string FilterDevicesList
        {
            get { return _settings.FilterDevicesList; }
            set { _settings.FilterDevicesList = value; }
        }

        public FilterDevicesViewModel(AppSettings settings) : base(null)
        {
            _settings = settings;
            Glyph = "\xE946";
            Title = Properties.Resources.FilterDevicesTitle;
        }

        public void OnLostFocus(object sender, RoutedEventArgs e)
        {
            var box = (sender as TextBox);
            FilterDevicesList = box.Text;
        }
    }
}
