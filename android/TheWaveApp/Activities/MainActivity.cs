using Android;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Google.Android.Material.Button;
using Java.IO;
using Java.Util;
using Model;
using System;
using System.Text;
using TheWaveApp.Services;

namespace TheWaveApp.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : BaseActivity
    {
        private string[] perms = new string[] { Manifest.Permission.Bluetooth, Manifest.Permission.BluetoothAdmin, Manifest.Permission.BluetoothPrivileged, Manifest.Permission.AccessFineLocation, Manifest.Permission.AccessCoarseLocation };
        // --- //

        public static byte[] buf;
        private static UUID mUUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
        private static int counter = 0;
        public static OutputStream outputStream;
        public static InputStream inputStream;

        public static BluetoothAdapter bAdapter;
        private BluetoothDevice commDevice;
        public static BluetoothSocket btSocket;

        // ----------- //
        private MaterialButton btnConnectionToggle;
        private MaterialButton btnStartRelay;
        private MaterialButton btnToggleContinuous;
        public static TextView txtDistance;

        private object padlock = new object();


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            RequestPermissions(perms, 1);
            InitializeViews();
            SetVisiblity();
        }

        private void SetVisiblity()
        {
            txtDistance.Visibility = Android.Views.ViewStates.Invisible;
            btnStartRelay.Visibility = Android.Views.ViewStates.Invisible;
            btnToggleContinuous.Visibility = Android.Views.ViewStates.Invisible;
        }

        private void ConfigureAdapter()
        {
            bAdapter = BluetoothAdapter.DefaultAdapter;
            commDevice = bAdapter.GetRemoteDevice(BluetoothMacCodes.Sensor1);
            try
            {
                btSocket = commDevice.CreateRfcommSocketToServiceRecord(mUUID);
                btSocket.Connect();
                bAdapter.CancelDiscovery();
                outputStream = ((Android.Runtime.OutputStreamInvoker)btSocket.OutputStream).BaseOutputStream;
                inputStream = ((Android.Runtime.InputStreamInvoker)btSocket.InputStream).BaseInputStream;
            }
            catch (IOException e)
            {
                Toast.MakeText(this, "Failed to create socket", ToastLength.Short).Show();
                e.PrintStackTrace();
            }
            
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void InitializeViews()
        {
            btnConnectionToggle = FindViewById<MaterialButton>(Resource.Id.btnConnectionToggle);
            btnConnectionToggle.Click += BtnConnectionToggle_Click;

            btnStartRelay = FindViewById<MaterialButton>(Resource.Id.btnStartRelay);
            btnStartRelay.Click += BtnStartRelay_Click;

            btnToggleContinuous = FindViewById<MaterialButton>(Resource.Id.btnToggleContinuous);
            btnToggleContinuous.Click += BtnToggleContinuous_Click;

            txtDistance = FindViewById<TextView>(Resource.Id.txtDistance);
            txtDistance.Text = string.Empty;
        }

        private void BtnToggleContinuous_Click(object sender, EventArgs e)
        {
            // if toggled color will be swtiched to green.
            if(counter == 0)
            {
                // stop sending continous data

                btnToggleContinuous.SetBackgroundColor(Android.Graphics.Color.Green);
                btnToggleContinuous.Text = "Disable Continuous Data Streaming";
                counter++;

                
                outputStream.Write((byte)'c');
                System.Threading.Thread.Sleep(500);

                Intent intent = new Intent(this, typeof(RecieveDataService));
                StartService(intent);

            }
            else if(counter == 1)
            {
                // now the sensor should send in continous stream
                btnToggleContinuous.SetBackgroundColor(Android.Graphics.Color.Blue);
                btnToggleContinuous.Text = "Enable Continuous Data Streaming";
                counter--;
                outputStream.Write((byte)'s');

                Intent intent = new Intent(this, typeof(RecieveDataService));
                StopService(intent);
            }
        }

        private void BtnStartRelay_Click(object sender, EventArgs e)
        {
            // check if char was inputed

            lock (padlock)
            {
                try
                {
                    outputStream.Write((byte)'d');
                    System.Threading.Thread.Sleep(1000);
                    if (inputStream.Available() > 0)
                    {
                        int legnth = inputStream.Available();
                        buf = new byte[1024];
                        var nn = inputStream.Read(buf, 0, legnth);
                        System.Array.Resize(ref buf, nn);
                        string msg = Encoding.ASCII.GetString(buf);
                        txtDistance.Visibility = Android.Views.ViewStates.Visible;
                        txtDistance.Text = msg;
                    }
                }
                catch (IOException ex)
                {
                }
            }

            // flush stream in case there was residual data left from previous connections
            //inputStream.Skip(inputStream.Available());


        }



        private void BtnConnectionToggle_Click(object sender, EventArgs e)
        {
            ConfigureAdapter();
            if (btSocket.IsConnected)
            {
                outputStream.Write((byte)'s');
                btnConnectionToggle.Text = $"Connected to\n{commDevice.Name}";
                btnConnectionToggle.SetBackgroundColor(Android.Graphics.Color.Green);
                btnStartRelay.Visibility = Android.Views.ViewStates.Visible;
                btnToggleContinuous.Visibility = Android.Views.ViewStates.Visible;
            }
        }


    }
}