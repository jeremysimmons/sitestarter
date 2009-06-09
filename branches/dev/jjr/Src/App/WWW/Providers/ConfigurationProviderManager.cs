using System;
using System.Configuration;
using System.Configuration.Provider;
using System.Web.Configuration;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Web.Providers
{
    public class ConfigurationProviderManager
    {
        //Initialization related variables and logic
        private static bool isInitialized = false;
        private static Exception initializationException;

        private static object initializationLock = new object();

        static ConfigurationProviderManager()
        {
            Initialize();
        }

        private static void Initialize()
        {

            try
            {
                //Get the feature's configuration info
                ConfigurationProviderConfiguration qc =
                    (ConfigurationProviderConfiguration)ConfigurationManager.GetSection("ConfigurationProvider");

                if (qc.DefaultProvider == null || qc.Providers == null || qc.Providers.Count < 1)
                    throw new ProviderException("You must specify a valid default provider.");

                //Instantiate the providers
                providerCollection = new ConfigurationProviderCollection();
                ProvidersHelper.InstantiateProviders(qc.Providers, providerCollection, typeof(ConfigurationProvider));
                providerCollection.SetReadOnly();
                defaultProvider = providerCollection[qc.DefaultProvider];
                if (defaultProvider == null)
                {
                    throw new ConfigurationErrorsException(
                        "You must specify a default provider for the feature.",
                        qc.ElementInformation.Properties["defaultProvider"].Source,
                        qc.ElementInformation.Properties["defaultProvider"].LineNumber);
                }
            }
            catch (Exception ex)
            {
                initializationException = ex;
                isInitialized = true;
                throw ex;
            }

            isInitialized = true; //error-free initialization
        }

        //Public feature API
        private static ConfigurationProvider defaultProvider;
        private static ConfigurationProviderCollection providerCollection;

        public static ConfigurationProvider Provider
        {
            get
            {
                return defaultProvider;
            }
        }

        public static ConfigurationProviderCollection Providers
        {
            get
            {
                return providerCollection;
            }
        }

       /* public static string DoWork()
        {
            return Provider.DoWork();
        }*/
    }
}
