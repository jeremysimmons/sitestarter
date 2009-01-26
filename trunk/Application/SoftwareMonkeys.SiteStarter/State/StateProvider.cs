using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Configuration.Provider;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.State
{
    /// <summary>
    /// Defines the interface required for all data providers.
    /// </summary>
    public abstract class StateProvider : ProviderBase
    {
     //   { get; }

       // public abstract void Initialize(string name, NameValueCollection settings);

       // void Dispose();

     //   public abstract IDataStore InitializeDataStore(string dataStoreName);
     //   public abstract string GetDataStoreName(Type objectType);
     //   public abstract string[] GetDataStoreNames();

        public abstract void SetApplication(string key, object value);
        public abstract object GetApplication(string key);
    }
}
