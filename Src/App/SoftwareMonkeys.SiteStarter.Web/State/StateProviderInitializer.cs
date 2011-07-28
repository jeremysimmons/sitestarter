using System;
using System.Configuration;
using System.Configuration.Provider;
using System.Web;
using System.Web.Configuration;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Web.State
{
	public class StateProviderInitializer
	{
		//Initialization related variables and logic
		private static bool isInitialized = false;
		private static Exception initializationException;

		private static object initializationLock = new object();

		static StateProviderInitializer()
		{
			Initialize();
		}

		/// <summary>
		/// Initializes the application state provider, including virtual server state.
		/// </summary>
		public static void Initialize()
		{
			if (!isInitialized)
			{
						InitializeApplicationState();

						isInitialized = true;

			}
		}
		
		/// <summary>
		/// Initializes the managed application and session state.
		/// </summary>
		private static void InitializeApplicationState()
		{
			
			try
			{
				//Get the feature's configuration info
				StateProviderConfiguration qc =
					(StateProviderConfiguration)ConfigurationManager.GetSection("StateProvider");

				if (qc.DefaultProvider == null || qc.Providers == null || qc.Providers.Count < 1)
					throw new ProviderException("You must specify a valid default provider.");

				//Instantiate the providers
				providerCollection = new StateProviderCollection();
				ProvidersHelper.InstantiateProviders(qc.Providers, providerCollection, typeof(BaseStateProvider));
				providerCollection.SetReadOnly();
				defaultProvider = providerCollection[qc.DefaultProvider];
				
				defaultProvider.ApplicationPath = HttpContext.Current.Request.ApplicationPath;
				defaultProvider.PhysicalApplicationPath = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath);
				
				if (defaultProvider == null)
				{
					throw new ConfigurationErrorsException(
						"You must specify a default provider for the feature.",
						qc.ElementInformation.Properties["defaultProvider"].Source,
						qc.ElementInformation.Properties["defaultProvider"].LineNumber);
				}
				
				StateAccess.State = defaultProvider;
			}
			catch (Exception ex)
			{
				initializationException = ex;
				isInitialized = false;
				throw ex;
			}
		}

		//Public feature API
		private static BaseStateProvider defaultProvider;
		private static StateProviderCollection providerCollection;

		public static BaseStateProvider Provider
		{
			get
			{
				return defaultProvider;
			}
		}

		public static StateProviderCollection Providers
		{
			get
			{
				return providerCollection;
			}
		}

	}
}
