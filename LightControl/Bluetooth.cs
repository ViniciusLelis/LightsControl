using Android.Bluetooth;
using Android.Content;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LightControl
{
    class Bluetooth
    {

        private static BluetoothAdapter btConnection = null;
        private static BroadcastBluetooth bdBluetooth = null;
        private static BluetoothSocket btSocket = null;

        public static void SetBluetoothAdapter(BluetoothAdapter bt)
        {
            btConnection = bt;
        }

        public static BluetoothAdapter GetBluetoothAdapter()
        {
            return btConnection;
        }

        public static BroadcastBluetooth GetBroadcastBluetooth()
        { 
            if (bdBluetooth == null)
            {
                bdBluetooth = new BroadcastBluetooth();
            }

            return bdBluetooth;
        }

        public static List<String> GetDevicesFound()
        {
            return GetBroadcastBluetooth().GetDevices();
        }

        public static List<BluetoothDevice> GetListOfDevicesFound()
        {
            return GetBroadcastBluetooth().GetBluetoothDevices();
            //return this..GetBluetoothDevices();
        }

        public static void SetBluetoothSocket(BluetoothSocket socket)
        {
            btSocket = socket;
        }

        public static BluetoothSocket GetBluetoothSocket()
        {
            return btSocket;
        }

        public static void CloseSocket()
        {
            if (btSocket != null)
                btSocket.Close();
        }

        public static void SendMessage(string message)
        {
            if (btSocket.IsConnected)
            {
                var outputStream = btSocket.OutputStream;
                var byteArray = Encoding.ASCII.GetBytes(message + "#");
                outputStream.Write(byteArray, 0, byteArray.Length);
            }
            else
            {
                throw new ConnectionClosedException();
            }

        }

        public static void RetryConnection()
        {
            var socket = (BluetoothSocket)btSocket.RemoteDevice.Class.GetMethod("createRfcommSocket", new Java.Lang.Class[] { Java.Lang.Integer.Type }).Invoke(btSocket.RemoteDevice, 1);

            btSocket = socket;

            if (!socket.IsConnected)
                socket.Connect();
        }
    }

    class BroadcastBluetooth : BroadcastReceiver
    {

        List<String> devices;
        Boolean finished;
        List<BluetoothDevice> devicesList;

        public BroadcastBluetooth()
        {
            devices = new List<string>();
            devicesList = new List<BluetoothDevice>();
            finished = false;
        }

        public Boolean IsFinished()
        {
            return finished;
        }

        public void SetFinished(bool finished)
        {
            this.finished = finished;
        }

        public List<String> GetDevices()
        {
            return devices;
        }

        public List<BluetoothDevice> GetBluetoothDevices()
        {
            return devicesList;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            Task refreshMessage = new Task(() => { CountTime(); });
            refreshMessage.Start();
            this.GoAsync();
            String action = intent.Action;

            if (BluetoothAdapter.ActionDiscoveryStarted.Equals(action))
            {
                finished = false;
            }
            else if (BluetoothAdapter.ActionDiscoveryFinished.Equals(action))
            {
                finished = true;
            }
            else if (BluetoothDevice.ActionFound.Equals(action))
            {
                BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                devicesList.Add(device);
                devices.Add(String.Format("Nome: {0}\nEndereço: {1}", device.Name, device.Address));
            }
        }

        public async void CountTime()
        {
            int i = 0;
            while (i<45)
            {
                await Task.Delay(1000);
                i++;
            }
            finished = true;
        }
    }

}