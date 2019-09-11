using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using sms_android.Resources.databaseActions;

namespace sms_android
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]

    public class MainActivity : AppCompatActivity, BroadcastReveiverOTP.OnReceiveSMSListener
    {

        // Se declara una nueva instancia de la clase BroadcastReceiverOTP que 
        // hace la función de estár a la escucha de los mensajes
        private BroadcastReveiverOTP _receiver;

        // Se declara una nueva lista de cadenas que se encargará de almacenar los números agregados a la aplicación

        List<string> numeros;

        // Se declara un objeto de la clase HttpClient que permite hacer la petición http
        // Es decir permite mandar la información a la página solicitada

        private HttpClient client;

        private DBActions db;

        // Se crean instancias de los objetos de la interfaz de usuario

        private Button agregarNumero;

        private EditText numero;

        private Button listaNumeros;

       

        // Método principal que se ejecuta al iniciar la aplicación
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            // Se espera la aplicación para saber si la aplicación tiene permisos de lectura de sms
            // si no es así se piden los permisos
            await TryToGetPermissions();

            // Se crear las instancias de las clases declaradas

            this.client = new HttpClient();
            
            this._receiver = new BroadcastReveiverOTP();

            // Método que pone a la esucha al receiver

            this._receiver.setOnReceiveListener(this);

            // método que crea la instancia de la aplicación

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            
            // se agrega la interfaz a la aplicación

            SetContentView(Resource.Layout.activity_main);

            // Se registra el receiver a utilizar, así como se le pasa el "intent" que en este caso
            // es SMS_RECEIVED el cúal permite leer los mensajes recibidos

            RegisterReceiver(_receiver, new IntentFilter("android.provider.Telephony.SMS_RECEIVED"));


            // Se vincula los objetos de la interfaz con los recursos de la misma

            this.agregarNumero = (Button)FindViewById(Resource.Id.button_agregar);

            this.numero = (EditText)FindViewById(Resource.Id.lista_numero);

            this.listaNumeros = (Button)FindViewById(Resource.Id.button_numeros);

            // Se agregan los eventos a los botones

            this.agregarNumero.Click += this.agregarHandler;

            this.listaNumeros.Click += abrirNumerosHandler;

            // Se conecta a la base de datos

            this.connect();
        }

        private void connect()
        {
            // Se crea una nueva instancia de DBActions, que es una clase auxiliar para manejar la base de datos
            this.db = new DBActions();
            try { 
                // Se conecta a la base de datos
                this.db.connectDB();
                // Se recuperan los teléfonos registrados en la base de datos
                this.numeros = this.db.getData();

            }
            catch (Exception ex)
            {
                // Si surge un problema es que no se ha creado la base de datos
                // Se procede a crear la base de datos
                this.db.createDB();
                // Se agrega un registro a la tabla de teléfonos
                this.db.createRegister("9212830763");
                // Se recuperan los teléfonos registrados en la base de datos
                this.numeros = this.db.getData();
            }
        }
        // método dónde se agrega un número a la tabla de teléfonos
        // Solamente de este listado de números se registrarán los mensajes
        public void agregarHandler(object sender, EventArgs e)
        {
            try
            {
                this.db.connectDB();
                this.db.createRegister(this.numero.Text);
                this.numeros = this.db.getData();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            // Se borra el texto capturado en el textfield
            this.numero.Text = "";
        }

        // Método que se dispara para listar los números registrados
        private void abrirNumerosHandler(object sender, EventArgs e)
        {
            Intent nextActivity = new Intent(this, typeof(Numeros));
            IList<String> numbers = this.numeros;
            nextActivity.PutStringArrayListExtra("numeros", numbers);
            StartActivity(nextActivity);
            
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        // Método que se ejecuta al recibir un nuevo mensaje
        public void onReceived(string message, string number)
        {
            // Se busca entre todos los números registrados
            foreach(var numero in this.numeros)
            {
                // si el número coincide con el teléfono que mandó el mensaje
                if (number == numero)
                {
                    // Se llama a la función que hace una consulta get a la página, es decir
                    // Crea un nuevo registro a la base de datos de la página
                    getMessage(message, number);
                }
            }
        }

        // Método que se ejecuta al recibir un mensaje de algún teléfono registrado
        private async void getMessage(string message,string telefono)
        {
            try
            {
                // Se asigna la url de la página con los parámetros recibidos
                var url = "http://lr.salvallanos.com/default.aspx?numero="+telefono+"&mensaje="+message;
                // se hace la petición HTTP
                var response = await this.client.GetAsync(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine("MyStackTrace: {0}", ex.ToString());
            }
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        // método que se ejecuta al iniciar la aplicación para saber si se cuenta con los permisos adecuados
        async Task TryToGetPermissions()
        {
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                await GetPermissionsAsync();
                return;
            }


        }
        const int RequestLocationId = 0;

        // Lista de permisos requeridos para la aplicación

        readonly string[] PermissionsGroupLocation =
            {
                            Manifest.Permission.ReadSms,
                            Manifest.Permission.Internet
             };

        // Método que se usa para obtener los permisos
        async Task GetPermissionsAsync()
        {
            const string permission = Manifest.Permission.AccessFineLocation;

            if (CheckSelfPermission(permission) == (int)Android.Content.PM.Permission.Granted)
            {
                Toast.MakeText(this, "Leer mensajes", ToastLength.Short).Show();
                return;
            }

            if (ShouldShowRequestPermissionRationale(permission))
            {
                //set alert for executing the task
                Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                alert.SetTitle("Se requieren permisos");
                alert.SetMessage("Para que funcione la aplicación se requiere de permisos");
                alert.SetPositiveButton("Permisos requeridos", (senderAlert, args) =>
                {
                    // se piden los permisos
                    RequestPermissions(PermissionsGroupLocation, RequestLocationId);
                });

                alert.SetNegativeButton("Cancel", (senderAlert, args) =>
                {
                    Toast.MakeText(this, "Cancelled!", ToastLength.Short).Show();
                });

                // se muestra el dialog dónde el usuario acepta los permisos
                Dialog dialog = alert.Create();
                dialog.Show();


                return;
            }

            RequestPermissions(PermissionsGroupLocation, RequestLocationId);

        }
        public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RequestLocationId:
                    {
                        // Si la aplicación tiene permisos se manda un Toast
                        if (grantResults[0] == (int)Android.Content.PM.Permission.Granted)
                        {
                            Toast.MakeText(this, "Permisos otorgados", ToastLength.Short).Show();

                        }
                        else
                        {
                            //En caso de no tener los permisos
                            Toast.MakeText(this, "Permisos denegados", ToastLength.Short).Show();

                        }
                    }
                    break;
            }
        }

    }
}

