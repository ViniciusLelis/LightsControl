
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;

namespace LightControl
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : ActionBarActivity
    {

        private bool processingEnabling;

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);

            processingEnabling = false;
        }

        protected override void OnResume()
        {
            base.OnResume();
            StartUp();
        }

        private void StartUp()
        {
            Bluetooth.SetBluetoothAdapter(BluetoothAdapter.DefaultAdapter);
            if (Bluetooth.GetBluetoothAdapter() != null)
            {
                enableBluetooth();
                StartActivity(new Intent(Application.Context, typeof(InitialActivity)));
            }
        }

        private void enableBluetooth()
        {
            while (!Bluetooth.GetBluetoothAdapter().IsEnabled)
            {
                if (!processingEnabling)
                {
                    processingEnabling = true;
                    Bluetooth.GetBluetoothAdapter().Enable();  
                }
            }
        }


    }
}