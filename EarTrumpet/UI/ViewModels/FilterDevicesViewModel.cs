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

        private void AddDevice(int index, FilterData filter)
        {
            var allExistingAdded = Devices.FirstOrDefault(d => d.Id == filter.Device.Id);
            if (allExistingAdded == null)
            {
                Devices.Insert(index, new FilterDataViewModel(filter, (x) => { ApplyFilterStatus(x); }));
            }
        }

        private void OnFilteredCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var added = (FilterData)e.NewItems[0];
                    AddDevice(e.NewStartingIndex, added);
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
                        AddDevice(Devices.Count, device);
                    }

                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public void ApplyFilterStatus(FilterData filterData)
        {
            _filterManager.ApplyDeviceStatus(filterData);
        }
    }
}
