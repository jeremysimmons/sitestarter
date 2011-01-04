using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Configuration.Provider;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.State
{
    /// <summary>
    /// Defines the interface required for all data providers.
    /// </summary>
    public abstract class StateProvider : ProviderBase, IStateProvider
    {
        public abstract bool ContainsApplication(string key);
        public abstract void SetApplication(string key, object value);
        public abstract object GetApplication(string key);
        public abstract void RemoveApplication(string key);
        
        public abstract bool ContainsSession(string key);
        public abstract void SetSession(string key, object value);
        public abstract object GetSession(string key);
        public abstract void RemoveSession(string key);
        
        public abstract bool ContainsRequest(string key);
        public abstract void SetRequest(string key, object value);
        public abstract object GetRequest(string key);
        public abstract void RemoveRequest(string key);
        
        public abstract bool ContainsUser(string key);
        public abstract void SetUser(string key, object value);
        public abstract void SetUser(string key, object value, DateTime expirationDate);
        public abstract object GetUser(string key);
        public abstract void RemoveUser(string key);
        
        public abstract string[] GetKeys(StateScope scope);
        
        public abstract string PhysicalApplicationPath { get;set; }
    }
}
