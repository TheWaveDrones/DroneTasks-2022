using Android;
using Android.App;
using Android.Bluetooth;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Google.Android.Material.Button;
using Java.IO;
using Java.Util;
using Model;
using System;
using System.Text;

namespace TheWaveApp.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : BaseActivity
    {
        private string[] perms = new string[] { Manifest.Permission.Bluetooth, Manifest.Permission.BluetoothAdmin, Manifest.Permission.BluetoothPrivileged, Manifest.Permission.AccessFineLocation, Manifest.Permission.AccessCoarseLocation };
        // --- //

        private static UUID mUUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");

        public static BluetoothAdapter bAdapter;
        private BluetoothDevice commDevice;
        private BluetoothSocket btSocket;

        // ----------- //
        private MaterialButton btnConnectionToggle;
        private MaterialButton btnStartRelay;
        private TextView txtDistance;


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
            txtDistance = FindViewById<TextView>(Resource.Id.txtDistance);
        }

        private void BtnStartRelay_Click(object sender, EventArgs e)
        {
            // check if char was inputed

            OutputStream outputStream = ((Android.Runtime.OutputStreamInvoker)btSocket.OutputStream).BaseOutputStream;
            InputStream inputStream = ((Android.Runtime.InputStreamInvoker)btSocket.InputStream).BaseInputStream;
            try
            {
                outputStream.Write((byte)'Z');
            }
            catch (IOException ex)
            {
            }

            // flush stream in case there was residual data left from previous connections
            inputStream.Skip(inputStream.Available());

            for (int i = 0; i < 100; i++)
            {
                byte b = (byte)inputStream.Read();
                txtDistance.Text = b.ToString();
                txtDistance.Visibility = Android.Views.ViewStates.Visible;
            }

        }

        private void BtnConnectionToggle_Click(object sender, EventArgs e)
        {
            ConfigureAdapter();
            if (btSocket.IsConnected)
            {
                btnConnectionToggle.Text = $"Connected to\n{commDevice.Name}";
                btnConnectionToggle.SetBackgroundColor(Android.Graphics.Color.Green);
                btnStartRelay.Visibility = Android.Views.ViewStates.Visible;
            }
        }


    }
}