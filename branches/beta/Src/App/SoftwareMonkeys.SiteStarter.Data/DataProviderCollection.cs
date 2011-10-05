using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;

namespace SoftwareMonkeys.SiteStarter.Data
{
    public class DataProviderCollection : ProviderCollection
    {
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("The provider parameter cannot be null.");

            if (!(provider is DataProvider))
                throw new ArgumentException("The provider parameter must be of type DataProvider.");

            base.Add(provider);
        }

        new public DataProvider this[string name]
        {
            get { return (DataProvider)base[name]; }
        }

        public void CopyTo(DataProvider[] array, int index)
        {
            base.CopyTo(array, index);
        }
    }
}
