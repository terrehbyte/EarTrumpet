using EarTrumpet.DataModel.Audio;
using EarTrumpet.Interop.Helpers;
using EarTrumpet.UI.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EarTrumpet.UI.ViewModels
{
    public class FilterDevicesViewModel : SettingsPageViewModel
    {
        private readonly FilterManager _filterManager;

        public ObservableCollection<FilterDataViewModel> Devices { get; private set; }

        public FilterDevicesViewModel(FilterManager filters) : base(null)
        {
            _filterManager = filters;
            _filterManager.DeviceStatus.CollectionChanged += OnFilteredCollectionChanged;
            Devices = new ObservableCollection<FilterDataViewModel>();
            Glyph = "\xE946";
            Title = Properties.Resources.FilterDevicesTitle;
            OnFilteredCollectionChanged(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void AddDevice(FilterData filter)
        {
            var added = filter;
            var allExistingAdded = Devices.FirstOrDefault(d => d.Id == added.Device.Id);
            if (allExistingAdded == null)
            {
                Devices.Add(new FilterDataViewModel(this, added, (x) => { ApplyFilterStatus(x); }));
            }
        }

        private void OnFilteredCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var added = (FilterData)e.NewItems[0];
                    AddDevice(added);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    var removed = (FilterData)e.OldItems[0];
                    var allExistingRemoved = Devices.FirstOrDefault(d => d.Id == removed.Device.Id);
                    if (allExistingRemoved != null)
                    {
                        Devices.Remove(allExistingRemoved);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    Devices.Clear();

                    foreach (var device in _filterManager.DeviceStatus)
                    {
                        AddDevice(device);
                    }

                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public void ApplyFilterStatus(FilterData filterData)
        {
            FilterManager.Current.ApplyDeviceStatus(filterData);
        }
    }
}
