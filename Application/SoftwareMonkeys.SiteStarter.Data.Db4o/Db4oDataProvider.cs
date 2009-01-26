using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Entities;
using System.IO;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o
{
    public class Db4oDataProvider : DataProvider 
    {
        private DataStoreCollection stores;
        public override DataStoreCollection Stores
        {
            get {
                if (stores == null)
                    stores = new DataStoreCollection();
                return stores; }
        }

        public void Initialize()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Dispose()
        {
            foreach (Db4oDataStore store in Stores)
            {
                store.Close();
            }


            foreach (Db4oDataStore store in Stores)
            {
                store.Dispose();
            }
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            DataAccess.Data = this;

            base.Initialize(name, config);

            /*if ((config == null) || (config.Count == 0))
                throw new ArgumentNullException("You must supply a valid configuration dictionary.");

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Put a localized description here.");
            }

            //Let ProviderBase perform the basic initialization
            base.Initialize(name, config);

            //Perform feature-specific provider initialization here

            //Get the connection string
            string connectionStringName = config["connectionStringName"];
            if (String.IsNullOrEmpty(connectionStringName))
                throw new ProviderException("You must specify a connectionStringName attribute.");

            ConnectionStringsSection cs =
                (ConnectionStringsSection)ConfigurationManager.GetSection("connectionStrings");
            if (cs == null)
                throw new ProviderException("An error occurred retrieving the connection strings section.");

            if (cs.ConnectionStrings[connectionStringName] == null)
                throw new ProviderException("The connection string could not be found in the connection strings section.");
            else
                connectionString = cs.ConnectionStrings[connectionStringName].ConnectionString;

            if (String.IsNullOrEmpty(connectionString))
                throw new ProviderException("The connection string is invalid.");
            config.Remove("connectionStringName");

            //Check to see if unexpected attributes were set in configuration
            if (config.Count > 0)
            {
                string extraAttribute = config.GetKey(0);
                if (!String.IsNullOrEmpty(extraAttribute))
                    throw new ProviderException("The following unrecognized attribute was found in " + Name + "'s configuration: '" +
                                                extraAttribute + "'");
                else
                    throw new ProviderException("An unrecognized attribute was found in the provider's configuration.");
            }*/
        }

        public override IDataStore InitializeDataStore(string dataStoreName)
        {
            return Db4oDataStoreFactory.InitializeDataStore(dataStoreName);
        }

        /// <summary>
        /// Gets the name of the data store that the provided entity is stored in.
        /// </summary>
        /// <param name="type">The type of entity to get the data store name of.</param>
        /// <param name="throwErrorIfNotFound">A flag indicating whether an error should be thrown when no data store attribute is found.</param>
        /// <returns>The data store that the provided entity is stored in.</returns>
        public string GetDataStoreName(Type type, bool throwErrorIfNotFound)
        {
            object[] attributes = (object[])type.GetCustomAttributes(true);
            foreach (object attribute in attributes)
            {
                if (attribute is DataStoreAttribute)
                    return ((DataStoreAttribute)attribute).DataStoreName;
            }
            if (throwErrorIfNotFound)
            {
                throw new Exception("No data store name was found for the entity '" + type.ToString() + "'");
            }
            return String.Empty;
        }


        /// <summary>
        /// Gets the name of the data store that the provided entity is stored in.
        /// </summary>
        /// <param name="type">The type of entity to get the data store name of.</param>
        /// <returns>The data store that the provided entity is stored in.</returns>
        public override string GetDataStoreName(Type type)
        {
            return GetDataStoreName(type, true);
        }

        /// <summary>
        /// Gets the names of the data stores in the data directory.
        /// </summary>
        /// <returns>The names of the data stores found.</returns>
        override public string[] GetDataStoreNames()
        {
            List<String> names = new List<String>();

            foreach (string file in Directory.GetFiles(Config.Application.PhysicalPath + @"\App_Data\", "*.yap"))
            {
                names.Add(Path.GetFileNameWithoutExtension(file));
            }

            return names.ToArray();
        }
    }
}
