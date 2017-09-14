using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Widget;
using System;
using System.Threading.Tasks;

namespace LightControl
{
    [Activity(Label = "Lights Control", Icon = "@drawable/icon")]
    public class InitialActivity : Activity
    {

        private Button buttonRefresh;
        private Button buttonConnect;
        private ListView listViewDevices;
        private Boolean updating;
        private BluetoothDevice selectedDevice = null;
        private int timeConnecting = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            updating = false;
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.InitialLayout);

            buttonRefresh = FindViewById<Button>(Resource.Id.buttonRefresh);
            buttonRefresh.Click += ButtonRefresh_Click;
            buttonConnect = FindViewById<Button>(Resource.Id.buttonConnect);
            buttonConnect.Click += ButtonConnect_Click;

            listViewDevices = FindViewById<ListView>(Resource.Id.listViewDevices);
            listViewDevices.ItemClick += ListViewDevices_ItemClick;

            Typeface font = Typeface.CreateFromAsset(this.Assets, "NexaLight.otf");
            buttonRefresh.Typeface = font;
            buttonConnect.Typeface = font;
        }

        private void ListViewDevices_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            selectedDevice = (BluetoothDevice)listViewDevices.Adapter.GetItem(e.Position);
            listViewDevices.GetChildAt(e.Position).SetBackgroundColor(Color.Rgb(229, 113, 101));
        }

        protected override void OnPause()
        {
            base.OnPause();
            updating = false;
        }

        protected override void OnResume()
        {
            base.OnResume();
            Bluetooth.CloseSocket();
            updating = false;
        }

        public async void CountTime()
        {
            await Task.Delay(1000);
            timeConnecting++;
        }

        private void ButtonConnect_Click(object sender, EventArgs e)
        {
            updating = false;
            Bluetooth.CloseSocket();
            timeConnecting = 0;
            if (selectedDevice != null)
            {
                
                if (Bluetooth.GetListOfDevicesFound().Count > 0)
                {
                    if (Bluetooth.GetBluetoothAdapter().IsDiscovering)
                        Bluetooth.GetBluetoothAdapter().CancelDiscovery();

                    try
                    {
                        BluetoothDevice device = selectedDevice;

                        if (device.CreateBond())
                        {
                            /*Task connectionTimeout = new Task(() => { CountTime(); });
                            connectionTimeout.Start();*/
                            while (!Bluetooth.GetBluetoothAdapter().BondedDevices.Contains(device)) ;
                        }

                        if (!Bluetooth.GetBluetoothAdapter().BondedDevices.Contains(device))
                            throw new FailedToBondException();

                        ParcelUuid[] uuids = device.GetUuids();

                        // Obrigado StackOverflow por existir. Agradecimentos ao brother que comentou a linha de c�digo abaixo como solu��o para um problema filha da puta que eu estava tendo. Pareceu muito gambiarra a solu��o, mas funcionou que foi uma beleza
                        var socket = (BluetoothSocket)device.Class.GetMethod("createRfcommSocket", new Java.Lang.Class[] { Java.Lang.Integer.Type }).Invoke(device, 1);

                        Bluetooth.SetBluetoothSocket(socket);

                        if (!socket.IsConnected)
                            socket.Connect();

                        Toast toast = Toast.MakeText(this, "Conectando...", ToastLength.Long);
                        toast.Show();

                        if (Bluetooth.GetBluetoothSocket().IsConnected)
                            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                    }
                    catch (Exception exception)
                    {
                        Toast toast = Toast.MakeText(this, exception.Message, ToastLength.Long);
                        toast.Show();
                    }
                }
            } else
            {
                Toast.MakeText(this, "Você precisa selecionar um dispositivo antes", ToastLength.Long).Show();
            }
        }

        public async void RefreshDevices()
        {
            while (updating)
            {
                buttonRefresh.Text = "ATUALIZAR .";
                await Task.Delay(650);
                buttonRefresh.Text = "ATUALIZAR ..";
                await Task.Delay(650);
                buttonRefresh.Text = "ATUALIZAR ...";
                await Task.Delay(650);
                buttonRefresh.Text = "ATUALIZAR";
                await Task.Delay(650);
            }
        }

        public async void PrintDevicesFound()
        {
            int tam = 0;
            while (!Bluetooth.GetBroadcastBluetooth().IsFinished())
            {
                if (Bluetooth.GetDevicesFound().Count > 0 && Bluetooth.GetDevicesFound().Count!=tam)
                {
                    tam = Bluetooth.GetDevicesFound().Count;
                    listViewDevices.Adapter = new BluetoothListAdapter(this, Bluetooth.GetListOfDevicesFound());
                    //buttonConnect.Text = Bluetooth.GetDevicesFound()[Bluetooth.GetDevicesFound().Count - 1];
                }
                    
                await Task.Delay(500);
            }
            updating = false;
        }

        private void ButtonRefresh_Click(object sender, EventArgs e)
        {
            if (!updating)
            {
                Bluetooth.GetListOfDevicesFound().Clear();
                Bluetooth.GetDevicesFound().Clear();
                listViewDevices.Adapter = new BluetoothListAdapter(this, Bluetooth.GetListOfDevicesFound());
                updating = true;

                RefreshDevices();

                IntentFilter filter = new IntentFilter();

                filter.AddAction(BluetoothDevice.ActionFound);
                filter.AddAction(BluetoothAdapter.ActionDiscoveryStarted);
                filter.AddAction(BluetoothAdapter.ActionDiscoveryFinished);

                Bluetooth.GetBroadcastBluetooth().SetFinished(false);
                RegisterReceiver(Bluetooth.GetBroadcastBluetooth(), filter);
                Bluetooth.GetBluetoothAdapter().StartDiscovery();

                PrintDevicesFound();
            }
        }
    }
}