using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;

namespace SoftwareMonkeys.SiteStarter.State
{
    public class StateProviderCollection : ProviderCollection
    {
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("The provider parameter cannot be null.");

            if (!(provider is StateProvider))
                throw new ArgumentException("The provider parameter must be of type StateProvider.");

            base.Add(provider);
        }

        new public StateProvider this[string name]
        {
            get { return (StateProvider)base[name]; }
        }

        public void CopyTo(StateProvider[] array, int index)
        {
            base.CopyTo(array, index);
        }
    }
}
