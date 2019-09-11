using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.RecyclerView.Extensions;
using Android.Views;
using Android.Widget;

namespace sms_android
{
    [Activity(Label = "Numeros")]
    public class Numeros : ListActivity
    {
        public IList<String> numeros;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            this.numeros = Intent.GetStringArrayListExtra("numeros");

            ListAdapter = new ArrayAdapter<string>(this, Resource.Layout.numeros, this.numeros);

            

           
        }
    }
}