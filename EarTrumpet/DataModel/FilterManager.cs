using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

using EarTrumpet.DataModel.Audio;
using EarTrumpet.UI.ViewModels;

namespace EarTrumpet
{
    public class FilterManager
    {
        public static FilterManager Current { get; private set; }

        public ObservableCollection<FilterData> DeviceStatus { get; }
        private HashSet<string> _filteredIDs;
        private readonly IAudioDeviceManager _deviceManager;
        private AppSettings _settings;
        public event EventHandler<FilterData> FilterChanged;

        public FilterManager(IAudioDeviceManager deviceManager, AppSettings settings)
        {
            _deviceManager = deviceManager;
            _deviceManager.Devices.CollectionChanged += OnCollectionChanged;
            _settings = settings;
            _filteredIDs = new HashSet<string>();
            DeviceStatus = new ObservableCollection<FilterData>();
            Current = this;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var added = ((IAudioDevice)e.NewItems[0]);
                    AddDevice(added);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var removed = ((IAudioDevice)e.OldItems[0]).Id;
                    var filteredExisting = DeviceStatus.FirstOrDefault(d => d.Device.Id == removed);
                    if (filteredExisting != null)
                    {
                        DeviceStatus.Remove(filteredExisting);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    DeviceStatus.Clear();
                    foreach(var device in _deviceManager.Devices)
                    {
                        AddDevice(device);
                    }
                    break;
            }
        }

        private void AddDevice(IAudioDevice device)
        {
            FilterData newFilter = new FilterData() { Device = device, IsShown = !_filteredIDs.Contains(device.Id) };
            DeviceStatus.Add(newFilter);
            FilterChanged(this, newFilter);
        }

        public void ApplyDeviceStatus(FilterData data)
        {
            bool didChange = false;

            if(data.IsShown)
            {
                didChange = _filteredIDs.Remove(data.Device.Id);
            }
            else
            {
                didChange = _filteredIDs.Add(data.Device.Id);
            }

            FilterChanged(this, data);

            if (didChange)
            {
                Save();
            }
        }

        public void Save()
        {
            string filterList = string.Empty;

            var it = _filteredIDs.GetEnumerator();
            while(it.MoveNext())
            {
                filterList += it.Current + ",";
            }
            if(_filteredIDs.Count > 0) { filterList = filterList.Substring(0, filterList.Length - 1); }

            _settings.FilterDevicesList = filterList;
        }

        public void Load()
        {
            string[] filterIDs = _settings.FilterDevicesList.Split(',');
            foreach(var filterID in filterIDs)
            {
                _filteredIDs.Add(filterID);
            }

            OnCollectionChanged(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
public class FilterData
{
    public IAudioDevice Device { get; set; }
    public bool IsShown { get; set; } = true;
}