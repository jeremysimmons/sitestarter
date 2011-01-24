using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;

namespace SoftwareMonkeys.SiteStarter.State
{
    public class StateProviderCollection : ProviderCollection//, ICollection<StateProvider>
    { 
    	
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider", "The provider parameter cannot be null.");

            if (!(provider is BaseStateProvider))
                throw new ArgumentException("provider", "The provider parameter must be of type StateProvider.");

            base.Add(provider);
        }

        new public BaseStateProvider this[string name]
        {
            get { return (BaseStateProvider)base[name]; }
        }

        public void CopyTo(BaseStateProvider[] array, int index)
        {
            base.CopyTo(array, index);
        }
    	
		/*public void Add(StateProvider item)
		{
			Add(item);
		}
    	
		public bool Contains(StateProvider item)
		{
			foreach (StateProvider provider in this)
			{
				if (item.Name == provider.Name)
					return true;
			}
				
			return false;
		}
    	
		public bool Remove(StateProvider item)
		{
			Remove(item.Name);
			// TODO: Check if this should return something else.
			return true;
		}
    	
		IEnumerator<StateProvider> GetEnumerator()
		{
			return (IEnumerator<StateProvider>)base.GetEnumerator();
		}
		
		public bool IsReadOnly
		{
			get { return false; }
		}*/
    }
}
