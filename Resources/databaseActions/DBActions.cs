using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using sms_android.Resources.model;
using SQLite;

namespace sms_android.Resources.databaseActions
{
    public class DBActions
    {

        private SQLiteConnection db;
        private string dbPath = Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
                "telefonos.db3"
            );

        public Boolean createDB()
        {
            try
            {
                this.db = new SQLiteConnection(dbPath);
                if (db.GetTableInfo("Telefonos").Count > 0)
                {
                    return false;
                }
                this.db.CreateTable<Telefonos>();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public Boolean connectDB()
        {
            try
            {
                this.db = new SQLiteConnection(dbPath);

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
            
        }

  

        public Boolean createRegister(String telefono)
        {
            if (telefono.Length != 10 || this.db == null)
            {
                return false;
            }

            try
            {
                var newTelefono = new Telefonos();
                newTelefono.telefono = telefono;
                this.db.Insert(newTelefono);
                return true;
            }catch(SQLiteException ex)
            {
                return false;
            }
            
        }

        public List<string> getData()
        {
           if (this.db == null || this.db.Table<Telefonos>().Count() == 0)
            {
                return null;
            }
            var telefonos = this.db.Table<Telefonos>();
            List<string> listaTelefonos = new List<string>();
           foreach(var telefono in telefonos)
            {
                listaTelefonos.Add(telefono.telefono);
            }

            return listaTelefonos;
        }
    }
}