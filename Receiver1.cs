using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Telephony;
using Android.Views;
using Android.Widget;


namespace sms_android

{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { "android.provider.Telephony.SMS_RECEIVED" }, Priority = (int)IntentFilterPriority.HighPriority)]

    // Se crea una nueva clase que extiende de BroadcastReceiver la cúal hace la función de tener permiso de escuchar
    // los mensajes entrantes
    public class BroadcastReveiverOTP : BroadcastReceiver
    {


        public static readonly string INTENT_ACTION = "android.provider.Telephony.SMS_RECEIVED";

        protected string message, address = string.Empty;

        private OnReceiveSMSListener _onReceiveSMSListener;

        public BroadcastReveiverOTP() { }

        [Obsolete]
        public override void OnReceive(Context context, Intent intent)
        {

            if (intent.HasExtra("pdus"))
            {
                var smsArray = (Java.Lang.Object[])intent.Extras.Get("pdus");
                foreach (var item in smsArray)
                {
                    var sms = SmsMessage.CreateFromPdu((byte[])item);
                    var address = sms.OriginatingAddress;
                    var message = sms.MessageBody;
                    //Toast.MakeText(context, "Numero" + " " + address + " Mensaje: " + message, ToastLength.Long).Show();

                    if (_onReceiveSMSListener != null)
                    {
                        _onReceiveSMSListener.onReceived(message, address);
                    }

                }
            }
        }

        public interface OnReceiveSMSListener
        {
            void onReceived(string message, string number);
        }

        public void setOnReceiveListener(OnReceiveSMSListener onReceiveSMSListener)
        {
            _onReceiveSMSListener = onReceiveSMSListener;
        }

    }

}