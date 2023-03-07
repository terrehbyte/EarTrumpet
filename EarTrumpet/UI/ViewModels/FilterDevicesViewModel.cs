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
        private readonly DeviceCollectionViewModel _mainViewModel;

        public string FilterDevicesList
        {
            get { return _settings.FilterDevicesList; }
            set { _settings.FilterDevicesList = value; }
        }

        public FilterDevicesViewModel(DeviceCollectionViewModel mainViewModel, AppSettings settings) : base(null)
        {
            _mainViewModel = mainViewModel;
            _settings = settings;
            Glyph = "\xE946";
            Title = Properties.Resources.FilterDevicesTitle;
        }

        public void OnLostFocus(object sender, RoutedEventArgs e)
        {
            var box = (sender as TextBox);
            FilterDevicesList = box.Text;
            _mainViewModel.ReloadDevices();
        }
    }
}
