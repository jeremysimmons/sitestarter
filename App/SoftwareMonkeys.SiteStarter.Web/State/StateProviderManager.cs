using System;
using System.Configuration;
using System.Configuration.Provider;
using System.Web.Configuration;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using NLog;

namespace SoftwareMonkeys.SiteStarter.Web.State
{
    public class StateProviderManager
    {
        //Initialization related variables and logic
        private static bool isInitialized = false;
        private static Exception initializationException;

        private static object initializationLock = new object();

        static StateProviderManager()
        {
                Initialize();
        }

        public static void Initialize()
        {
            if (!isInitialized)
            {
                // TODO: Clean up
                //Logger logger = LogManager.GetLogger("StateProviderManager");
                //logger.Info("StateProviderManager.Initialize()");

                using (LogGroup logGroup = AppLogger.StartGroup("Initializes the state provider manager to hold all application data while in memory."))
                {
                    if (!isInitialized)
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
                            ProvidersHelper.InstantiateProviders(qc.Providers, providerCollection, typeof(StateProvider));
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
                }
            }
        }

        //Public feature API
        private static StateProvider defaultProvider;
        private static StateProviderCollection providerCollection;

        public static StateProvider Provider
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

       /* public static string DoWork()
        {
            return Provider.DoWork();
        }*/
    }
}
