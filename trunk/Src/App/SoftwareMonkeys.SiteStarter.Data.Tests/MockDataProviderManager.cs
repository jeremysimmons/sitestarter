using System;
using System.Configuration;
using System.Configuration.Provider;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
    public class MockDataProviderManager
    {
        //Initialization related variables and logic
        private static bool isInitialized = false;
        private static Exception initializationException;

        private static object initializationLock = new object();

        static MockDataProviderManager()
        {
                //Initialize();
        }

        public static void Initialize()
        {
        	throw new NotImplementedException();
        }

        //Public feature API
        private static DataProvider defaultProvider;
        private static DataProviderCollection providerCollection;

        public static DataProvider Provider
        {
            get
            {
                return defaultProvider;
            }
        }

        public static DataProviderCollection Providers
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
