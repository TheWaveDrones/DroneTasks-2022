using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheWaveApp.Activities;

namespace TheWaveApp.Services
{
    [Service]
    public class RecieveDataService : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            base.OnStartCommand(intent, flags, startId);
            Task.Run(() => 
            {
                while(true)
                {
                    Thread.Sleep(500);
                    if (MainActivity.inputStream.Available() > 0)
                    {
                        int legnth = MainActivity.inputStream.Available();
                        MainActivity.buf = new byte[1024];
                        var nn = MainActivity.inputStream.Read(MainActivity.buf, 0, legnth);
                        System.Array.Resize(ref MainActivity.buf, nn);
                        string msg = Encoding.ASCII.GetString(MainActivity.buf);
                        MainActivity.txtDistance.Visibility = Android.Views.ViewStates.Visible;
                        MainActivity.txtDistance.Text = msg;
                    }
                }
            });

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            // closes the bluetooth connection
            MainActivity.btSocket.Close();
        }
    }
}