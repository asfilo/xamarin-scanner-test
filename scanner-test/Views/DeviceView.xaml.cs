using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.BLE.Abstractions.EventArgs;
using scannertest.Models;

namespace scannertest.Views
{
    public partial class DeviceView : ContentPage
    {
        IBluetoothLE ble;
        IAdapter adapter;
        ObservableCollection<IDevice> deviceList;
        IDevice device;

        private const string SCANNER_DEVICE_ID = "00000000-0000-0000-0000-aaa8ac167d9f";
        private const string SCAN_SERVICE_ID = "0000feea-0000-1000-8000-00805f9b34fb";
        private const string SCAN_CHARACTERISTIC_ID = "00002aa1-0000-1000-8000-00805f9b34fb";

        public DeviceView()
        {
            InitializeComponent();
            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;
            deviceList = new ObservableCollection<IDevice>();
            lv.ItemsSource = deviceList;

            //adapter.DeviceDiscovered += _bleAdapterDeviceDiscovered;
        }

        //private void _bleAdapterDeviceDiscovered(object sender, DeviceEventArgs e)
        //{
        //    var msg = string.Format("DeviceFound {0}", e.Device.Name);
        //    var devicesss = msg;
        //}

        /// <summary>
        /// Define the status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStatus_Clicked(object sender, EventArgs e)
        {
            var state = ble.State;

            DisplayAlert("Notice", state.ToString(), "OK !");
            if (state == BluetoothState.Off)
            {
                txtErrorBle.BackgroundColor = Color.Red;
                txtErrorBle.TextColor = Color.White;
                txtErrorBle.Text = "Your Bluetooth is off ! Turn it on !";
            }
        }

        /// <summary>
        /// Scan the list of Devices
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnScan_Clicked(object sender, EventArgs e)
        {
            try
            {
                deviceList.Clear();
                adapter.DeviceDiscovered += (s, a) =>
                {
                    deviceList.Add(a.Device);
                };

                if (ble.Adapter.IsScanning)
                    await adapter.StopScanningForDevicesAsync();

                //We have to test if the device is scanning 
                if (!ble.Adapter.IsScanning)
                {
                    await adapter.StartScanningForDevicesAsync();

                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Notice", ex.Message.ToString(), "Error !");
            }

        }

        /// <summary>
        /// Connect to a specific device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnConnect_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (device != null)
                {
                    await adapter.ConnectToDeviceAsync(device);
                    if (ble.Adapter.IsScanning)
                        await adapter.StopScanningForDevicesAsync();
                }
                else
                {
                    DisplayAlert("Notice", "No Device selected !", "OK");
                }
            }
            catch (DeviceConnectionException ex)
            {
                //Could not connect to the device
                DisplayAlert("Notice", ex.Message.ToString(), "OK");
            }
        }

        private async void btnKnowConnect_Clicked(object sender, EventArgs e)
        {

            try
            {
                device = await adapter.ConnectToKnownDeviceAsync(new Guid(SCANNER_DEVICE_ID));

            }
            catch (DeviceConnectionException ex)
            {
                //Could not connect to the device
                DisplayAlert("Notice", ex.Message.ToString(), "OK");
            }
        }

        IList<IService> Services;
        IService Service;
        IList<ServiceCharacteristicModel> ServiceCharacteristics;
        ICharacteristic Characteristic;

        /// <summary>
        /// Get list of services
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnGetServices_Clicked(object sender, EventArgs e)
        {
            //Services = (System.Collections.Generic.IList<Plugin.BLE.Abstractions.Contracts.IService>)await device.GetServicesAsync();


            Service = await device.GetServiceAsync(new Guid(SCAN_SERVICE_ID));
            Characteristic = await Service.GetCharacteristicAsync(new Guid(SCAN_CHARACTERISTIC_ID));
            if (Characteristic.CanUpdate)
            {
                Characteristic.ValueUpdated += (s, a) =>
                {
                    string converted = Encoding.UTF8.GetString(a.Characteristic.Value, 0, a.Characteristic.Value.Length);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ScannedLabel.Text = converted;
                    });
                };
                try
                {
                    var desc = await Characteristic.GetDescriptorsAsync();
                    await Characteristic.StartUpdatesAsync();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                }
            }

            // Service = await device.GetServiceAsync(Guid.Parse("guid")); 
            //or we call the Guid of selected Device
            //ServiceCharacteristics = new List<ServiceCharacteristicModel>();

            //foreach (var service in Services)
            //{

            //    var characteristics = (IList<ICharacteristic>)await service.GetCharacteristicsAsync();

            //    foreach (var c in characteristics)
            //    {

            //        if (c.Id == new Guid(SCAN_CHARACTERISTIC_ID))
            //        {
            //            await DisplayAlert("Service", $"{service.Id}, {service.Name}", "OK");

            //            if (c.CanUpdate)
            //            {
            //                c.ValueUpdated += (se, a) =>
            //                {
            //                    string converted = Encoding.UTF8.GetString(a.Characteristic.Value, 0, a.Characteristic.Value.Length);
            //                    Device.BeginInvokeOnMainThread(() =>
            //                    {
            //                        ScannedLabel.Text = converted;
            //                    });

            //                };

            //                await c.StartUpdatesAsync();
            //            }
            //        }
            //    }


            //}

        }

        IList<ICharacteristic> Characteristics;
        //ICharacteristic Characteristic;
        /// <summary>
        /// Get Characteristics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnGetcharacters_Clicked(object sender, EventArgs e)
        {
            var characteristics = await Service.GetCharacteristicsAsync();
            Guid idGuid = Guid.Parse("guid");
            Characteristic = await Service.GetCharacteristicAsync(idGuid);
            //  Characteristic.CanRead
        }

        IDescriptor descriptor;
        IList<IDescriptor> descriptors;

        private async void btnDescriptors_Clicked(object sender, EventArgs e)
        {
            descriptors = (System.Collections.Generic.IList<Plugin.BLE.Abstractions.Contracts.IDescriptor>)await Characteristic.GetDescriptorsAsync();
            descriptor = await Characteristic.GetDescriptorAsync(Guid.Parse("guid"));

        }

        private async void btnDescRW_Clicked(object sender, EventArgs e)
        {
            var bytes = await descriptor.ReadAsync();
            await descriptor.WriteAsync(bytes);
        }

        private async void btnGetRW_Clicked(object sender, EventArgs e)
        {
            var bytes = await Characteristic.ReadAsync();
            await Characteristic.WriteAsync(bytes);
        }

        private async void btnUpdate_Clicked(object sender, EventArgs e)
        {
            Characteristic.ValueUpdated += (o, args) =>
            {
                var bytes = args.Characteristic.Value;
            };
            await Characteristic.StartUpdatesAsync();
        }

        /// <summary>
        /// Select Items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lv_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (lv.SelectedItem == null)
            {
                return;
            }
            device = lv.SelectedItem as IDevice;
        }

        private void txtErrorBle_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }
    }
}
