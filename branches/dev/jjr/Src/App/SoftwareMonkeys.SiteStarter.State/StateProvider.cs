using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Configuration.Provider;

namespace SoftwareMonkeys.SiteStarter.State
{
    /// <summary>
    /// Defines the interface required for all data providers.
    /// </summary>
    public abstract class StateProvider : ProviderBase
    {
        public abstract bool ContainsApplication(string key);
        public abstract void SetApplication(string key, object value);
        public abstract object GetApplication(string key);
        
        public abstract bool ContainsSession(string key);
        public abstract void SetSession(string key, object value);
        public abstract object GetSession(string key);
    }
}
