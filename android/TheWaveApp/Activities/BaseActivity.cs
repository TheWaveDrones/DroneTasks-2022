using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheWaveApp.Activities
{
    [Activity(Label = "BaseActivity")]
    public abstract class BaseActivity : AppCompatActivity
    {
        protected abstract void InitializeViews();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
    }
}