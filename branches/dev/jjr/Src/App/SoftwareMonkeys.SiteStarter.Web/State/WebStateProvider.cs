using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Entities;
using System.IO;
using SoftwareMonkeys.SiteStarter.Configuration;
using System.Web;

namespace SoftwareMonkeys.SiteStarter.Web.State
{
    public class WebStateProvider : StateProvider 
    {
       /* private DataStoreCollection stores;
        public override DataStoreCollection Stores
        {
            get {
                if (stores == null)
                    stores = new DataStoreCollection();
                return stores; }
        }*/

        public void Initialize()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }
        
        #region Application state
        public override bool ContainsApplication(string key)
        {
        	if (key == String.Empty)
        		throw new ArgumentException("The provided key cannot be null or String.Empty.");
        	
       		if (key != String.Empty
        	    && HttpContext.Current != null &&
        	    HttpContext.Current.Application[key] != null)
	            return true;
	        else
	        	return false;
        }

        public override void SetApplication(string key, object value)
        {
        	if (key == String.Empty)
        		throw new ArgumentException("The provided key cannot be null or String.Empty.");
        	
            if (key != String.Empty && HttpContext.Current != null)
            {
                HttpContext.Current.Application[key] = value;
            }
        }

        public override object GetApplication(string key)
        {
        	if (key == String.Empty)
        		throw new ArgumentException("The provided key cannot be null or String.Empty.");
        	
       	if (key != String.Empty && HttpContext.Current != null)
	            return HttpContext.Current.Application[key];
	        else
	        	return null;
        }
        #endregion
        
        #region Session state
        public override bool ContainsSession(string key)
        {
        	if (key == String.Empty)
        		throw new ArgumentException("The provided key cannot be null or String.Empty.");
        	
       		if (key != String.Empty
        	    && HttpContext.Current != null &&
        	    HttpContext.Current.Session[key] != null)
	            return true;
	        else
	        	return false;
        }
        
        public override void SetSession(string key, object value)
        {
        	if (key == String.Empty)
        		throw new ArgumentException("The provided key cannot be null or String.Empty.");
        	
            if (key != String.Empty && HttpContext.Current != null)
            {
                HttpContext.Current.Session[key] = value;
            }
        }

        public override object GetSession(string key)
        {
        	if (key == String.Empty)
        		throw new ArgumentException("The provided key cannot be null or String.Empty.");
        	
        	if (key != String.Empty && HttpContext.Current != null && HttpContext.Current.Session != null)
	            return HttpContext.Current.Session[key];
	        else
	        	return null;
        }
        #endregion

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {

        //    throw new Exception("The method or operation is not implemented.");
            SoftwareMonkeys.SiteStarter.State.StateAccess.State = this;

            base.Initialize(name, config);
        }

       /* public override IDataStore InitializeDataStore(string dataStoreName)
        {
            return WebStateStoreFactory.InitializeDataStore(dataStoreName);
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
        }*/
    }
}
