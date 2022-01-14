using Android.Bluetooth;
using Android.Content;
using Android.Widget;
using Model;
using TheWaveApp.Activities;

namespace TheWaveApp.Boradcast
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class BluetoothReciver : BroadcastReceiver
    {
        private Devices foundDevices = new Devices();

        public override void OnReceive(Context context, Intent intent)
        {

        }


        /*
           string action = intent.Action;
           if (action.Equals(BluetoothDevice.ActionFound))
           {
               BluetoothDevice device = intent.GetParcelableExtra(BluetoothDevice.ExtraDevice) as BluetoothDevice;

               string check = $"{device.Name}\n{device.Address}";
               if (!MainActivity.pairedDevices.Exists(item => item.Address.Equals(device.Address)))
               {
                   MainActivity.pairedDevices.Add(device);
                   foreach (BluetoothDevice bd in MainActivity.pairedDevices)
                   {
                       if (!foundDevices.Exists(check))
                       {
                           foundDevices.Add($"{bd.Name}\n{bd.Address}");
                       }
                   }
                   MainActivity.lvDevices.Adapter = new ArrayAdapter(context, Android.Resource.Layout.SimpleListItem1, foundDevices);
               }
           */

    }
}