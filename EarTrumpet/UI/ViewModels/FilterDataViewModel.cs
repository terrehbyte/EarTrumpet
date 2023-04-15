using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EarTrumpet.DataModel.Audio;
using EarTrumpet.DataModel.WindowsAudio;
using EarTrumpet.Interop.Helpers;
using EarTrumpet.UI.ViewModels;

namespace EarTrumpet.UI
{
    public class FilterDataViewModel
    {
        public string DisplayName => _data.Device.DisplayName;
        public string AccessibleName => Properties.Resources.AppOrDeviceMutedFormatAccessibleText.Replace("{Name}", DisplayName);
        public string DeviceDescription => ((IAudioDeviceWindowsAudio)_data.Device).DeviceDescription;
        public string InterfaceName => ((IAudioDeviceWindowsAudio)_data.Device).InterfaceName;
        public string Id => _data.Device.Id;
        public bool IsShown
        {
            get => _data.IsShown;
            set
            {
                _data.IsShown = value;
                _apply(_data);
            }
        }

        private FilterData _data;
        private readonly Action<FilterData> _apply;

        public FilterDataViewModel(FilterData filter, Action<FilterData> apply)
        {
            _apply = apply;
            _data = filter;
        }
    }
}
